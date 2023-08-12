using System;
using System.Collections.Generic;

using UnityEngine;

public enum SoccerMessages
{
   msg_SupportAttacker,
   msg_GoHome,
   msg_ReceiveBall,
   msg_PassToMe,
   msg_Wait
};

public class PlayerController : MonoBehaviour
{
   // Player Components
   #region
   public bool isGoalkeeper = false;

   public CircleCollider2D PlayerCollider;

   public Rigidbody2D PlayerRigidBody;

   public GameObject Pitch;

   public GameObject GoalTarget;

   public TeamScript playerTeam;

   public float minPassDistance = 1f;

   public float kickCooldown = 0.5f;

   public bool hasPossession = false;

   public GameObject ArriveTarget;

   // Home region
   public Vector2 playerHomeRegion;

   #endregion

   // FSM Components
   #region
   public string CurrentState;

   public StateMachine<PlayerController> Player_FSM;

   public State<PlayerController> state_PlayerKickBall, state_PlayerChase, state_PlayerWait, state_PlayerDribble, state_PlayerReturnHome,
                                   state_PlayerReceiveBall, state_SupportAttacker, state_PlayerIntercept;
   private Transition<PlayerController>.condition cond_kickBall, cond_chaseBall, cond_Wait, cond_dribble, cond_returnHome,
                                           cond_receiveBall, cond_SupportAttacker;
   #endregion

   // Dictionary
   #region
   [Serializable]
   public class PlayerStatesDictionary : SerializableDictionary<MyTransitionClass, float> { }

   [Tooltip("The state transition and transition time")]
   public PlayerStatesDictionary myStates;

   [Serializable]
   public class MyTransitionClass
   {
      [Tooltip("The current state")]
      public States cState;

      [Tooltip("The condition for the transition to happen")]
      public Conditions cond;

      [Tooltip("Bool type (true/false)")]
      public bool Bool;

      [Tooltip("Next state to transition to")]
      public States nState;
   }

   public enum States
   {
      //KickBall,
      //Chase,
      //Wait,
      //Dribble,
      //ReturnHome,
      //ReceiveBall,
      //SupportAttacker
   }

   public enum Conditions
   {
      //c_KickBall,
      //c_Chase,
      //c_Wait,
      //c_Dribble,
      //c_ReturnHome,
      //c_ReceiveBall,
      //c_SupportAttacker
   }

   // Go through the dictionary of transitions and set them accordingly
   public void ConfigureTransitions (PlayerStatesDictionary myDictionary)
   {
      foreach (KeyValuePair<MyTransitionClass, float> keyValuePair in myDictionary)
      {
         enumToState(keyValuePair.Key.cState).Transitions.Add(new Transition<PlayerController>(enumToCondition(keyValuePair.Key.cond), keyValuePair.Key.Bool, enumToState(keyValuePair.Key.nState)));
      }
   }

   // Enum to state function
   private State<PlayerController> enumToState (States myState)
   {
      return myState.ToString() switch
      {
         _ => null,
      };
   }

   // Enum to condition function
   private Transition<PlayerController>.condition enumToCondition (Conditions myCondition)
   {
      return myCondition.ToString() switch
      {
         _ => null,
      };
   }
   #endregion

   public SoccerMessages messages;

   private void Start ()
   {
      PlayerCollider = GetComponentInChildren<CircleCollider2D>();

      PlayerRigidBody = GetComponent<Rigidbody2D>();

      Pitch = GameObject.FindGameObjectWithTag("Pitch");
   }

   public void Awake ()
   {
      // Configure Player Components
      playerTeam = GetComponentInParent<TeamScript>();
      GoalTarget = playerTeam.goalTarget;

      // Configure FSM
      #region 

      Player_FSM = new StateMachine<PlayerController>();

      // Set up the states
      state_PlayerKickBall = new PlayerKickBallState();

      state_PlayerDribble = new PlayerDribbleState();

      state_PlayerWait = new PlayerWaitState();

      state_PlayerChase = new PlayerChaseState(GetFootball());

      state_PlayerKickBall = new PlayerKickBallState();

      state_PlayerReturnHome = new PlayerReturnHomeScript();

      state_SupportAttacker = new PlayerSupportAttackerState();

      state_PlayerReceiveBall = new PlayerReceiveBallState();

      state_PlayerIntercept = new PlayerInterceptState();

      // Set up the conditions

      // Configure the state transitions 
      ConfigureTransitions(myStates);

      Player_FSM.Configure(this, state_PlayerReturnHome);
      #endregion
   }

   public void ChangeState (State<PlayerController> e)
   {
      Player_FSM.ChangeState(e);
   }

   public void Update ()
   {
      if (Player_FSM != null)
      {
         Player_FSM.Update();
      }

      kickCooldown -= Time.deltaTime;

      CurrentState = GetCurrentState().ToString();
   }

   public State<PlayerController> GetCurrentState ()
   {
      return Player_FSM.GetCurrentState();
   }

   public void TrackBall ()
   {
      //rotate to look at the ball
      transform.up = Vector2.Lerp(transform.up, GetFootball().transform.position - transform.position, 0.25f * Time.deltaTime * 400);
   }

   public void FindSupport ()
   {
      foreach (GameObject player in playerTeam.GetTeamPlayers())
      {
         if (playerTeam.ClosestToBss == player && player != playerTeam.ReceivingPlayer && !player.GetComponent<PlayerController>().isGoalkeeper)
         {
            PlayerController support = playerTeam.ClosestToBss.GetComponent<PlayerController>();
            support.ChangeState(support.state_SupportAttacker);
         }
         else
         {
            PlayerController currentPlayer = player.GetComponent<PlayerController>();
            if (currentPlayer.Player_FSM.GetCurrentState() == currentPlayer.state_PlayerWait)
            {
               currentPlayer.ChangeState(currentPlayer.state_PlayerReturnHome);
            }
         }
      }
   }

   public bool IsThreatened ()
   {
      return transform.GetComponentInChildren<OpponentsNearby>().GetOpponnentsNearby() > 0;
   }

   public bool OpponenetsForward ()
   {
      List<Collider2D> opponents = transform.GetComponentInChildren<OpponentsNearby>().collidersList;

      foreach (Collider2D enemy in opponents)
      {
         Vector3 toOpponent = (enemy.transform.position - transform.position).normalized;
         Vector3 toTarget = (GoalTarget.transform.position - transform.position).normalized;

         var dot = Vector2.Dot(toOpponent, transform.position);

         if (Mathf.Sign(toOpponent.x) == Mathf.Sign(toTarget.x))
         {
            return true;
         }
      }

      return false;
   }

   // Returns the football gameobject
   public GameObject GetFootball ()
   {
      return GameObject.Find("Football");
   }

   public PlayerController GetChasingPlayer ()
   {
      foreach (GameObject player in playerTeam.GetTeamPlayers())
      {
         if (player != gameObject)
         {
            if (player.GetComponent<PlayerController>().Player_FSM.GetCurrentState() == player.GetComponent<PlayerController>().state_PlayerChase)
            {
               return player.GetComponent<PlayerController>();
            }
         }
      }

      return null;
   }

   private bool IsClosestToBall ()
   {
      return playerTeam.ClosestToBall == gameObject;
   }

   // Is the player in possession of the ball
   private bool IsInPossession ()
   {
      return hasPossession;
   }

   // Is the player touching the ball
   private bool IsTouchingBall ()
   {
      var touchBall = PlayerCollider.IsTouchingLayers(LayerMask.GetMask("Football")) && GetFootball().transform.localScale.x <= 0.25f;
      return touchBall;
   }

   // Check if this player is closer to ball than the current controlling player
   public bool IsAheadOfAttacker ()
   {
      if (playerTeam.ControllingPlayer != null &&
          Vector2.Distance(transform.position, GoalTarget.transform.position) <
          Vector2.Distance(playerTeam.ControllingPlayer.transform.position, GoalTarget.transform.position))
      {
         return true;
      }
      else
      {
         return false;
      }
   }

   // Check if the player is ready for the next kick
   public bool isReadyForNextKick ()
   {
      if (kickCooldown < 0)
      {
         return true;
      }

      return false;
   }

   public void PassToTarget (PlayerController target)
   {
      if (PlayerCollider.IsTouchingLayers(LayerMask.GetMask("Football")))
      {
         GetFootball().GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);

         kickCooldown = 0.5f;
         // Pass the ball

         // Add some error to the pass
         var kickDir = new Vector2(target.transform.position.x - GetFootball().transform.position.x, target.transform.position.y - GetFootball().transform.position.y);

         // Kick the ball in the given direction
         GetFootball().GetComponent<FootballScript>().KickBall(kickDir.normalized, 100.0f);

         // Send a message to the receiver
         target.ChangeState(target.state_PlayerReceiveBall);

         // Find Support
         FindSupport();

         // Change state to wait
         ChangeState(state_PlayerWait);

         return;
      }
   }

   public PlayerController GetInterceptor ()
   {
      PlayerController bestInterceptor = null;

      // If the team has the ball, we do not need to intercept it
      if (playerTeam.m_HasBall || Pitch.GetComponent<SoccerPitchScript>().GameOn == false)
      {
         return null;
      }

      GameObject[] teams = GameObject.FindGameObjectsWithTag("Team");

      if (GetChasingPlayer())
      {
         foreach (GameObject item in teams)
         {
            if (item != playerTeam)
            {
               GameObject controllingPlayer = item.GetComponent<TeamScript>().ControllingPlayer;

               if (controllingPlayer &&
                   controllingPlayer.GetComponent<PlayerController>().GetCurrentState() == controllingPlayer.GetComponent<PlayerController>().state_PlayerChase)
               {
                  PlayerController chaser = GetChasingPlayer();

                  Vector3 toTarget = controllingPlayer.transform.position - chaser.transform.position;

                  // If the chasing player is already infront of the opponent, we don't need to intercept it
                  if (playerTeam.lhs_regions && toTarget.x > 0)
                  {
                     return null;
                  }
                  else if (!playerTeam.lhs_regions && toTarget.x < 0)
                  {
                     return null;
                  }
                  // Search for the closest player that is in front of the opponent
                  else
                  {
                     var closestDistance = Mathf.Infinity;

                     foreach (GameObject currentPlayer in playerTeam.GetTeamPlayers())
                     {
                        if (currentPlayer != chaser &&
                            (!currentPlayer.GetComponent<PlayerController>().isGoalkeeper ||
                            Vector3.Distance(controllingPlayer.transform.position, controllingPlayer.GetComponent<PlayerController>().GoalTarget.transform.position) < 2.0f))
                        {
                           Vector3 l_dir = controllingPlayer.transform.position - currentPlayer.transform.position;

                           if (playerTeam.lhs_regions)
                           {
                              if (l_dir.x > 0)
                              {
                                 var dis = Vector2.Distance(currentPlayer.transform.position, controllingPlayer.transform.position);
                                 if (dis < closestDistance)
                                 {
                                    bestInterceptor = currentPlayer.GetComponent<PlayerController>();

                                    closestDistance = dis;
                                 }
                              }
                           }
                           else
                           {
                              if (l_dir.x < 0)
                              {
                                 var dis = Vector2.Distance(currentPlayer.transform.position, controllingPlayer.transform.position);
                                 if (dis < closestDistance)
                                 {
                                    bestInterceptor = currentPlayer.GetComponent<PlayerController>();

                                    closestDistance = dis;
                                 }
                              }
                           }
                        }
                     }
                  }
               }
            }
         }
      }

      if (bestInterceptor != null)
      {
         return bestInterceptor;
      }

      else
      {
         return null;
      }
   }


   // Check if the player is in his home region
   public bool isPlayerHome ()
   {
      if (Vector2.Distance(transform.position, playerHomeRegion) < 0.1f)
      {
         return true;
      }
      else
      {
         return false;
      }
   }
}
