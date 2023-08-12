using UnityEngine;

// Go to the position of the ball
public class PlayerChaseState : State<PlayerController>
{
   protected GameObject ChaseTarget = null;

   public PlayerChaseState (GameObject target)
   {
      ChaseTarget = target;
   }

   public override void Enter (PlayerController player)
   {
      if (player.GetChasingPlayer() && player.GetChasingPlayer() != player && player.GetChasingPlayer() != player.playerTeam.ReceivingPlayer)
      {
         player.GetChasingPlayer().ChangeState(player.GetChasingPlayer().state_PlayerReturnHome);
      }
   }

   public override void Execute (PlayerController player)
   {
      GameObject[] Goals = GameObject.FindGameObjectsWithTag("Goal");

      GameObject homeGoal = null;

      if (Goals[0] == player.GoalTarget)
      {
         homeGoal = Goals[1];
      }
      else
      {
         homeGoal = Goals[0];
      }

      if (player.isGoalkeeper)
      {
         if (Vector2.Distance(player.GetFootball().transform.position, homeGoal.transform.position) < 2.0f)
         {
            player.transform.position = Vector3.MoveTowards(player.transform.position, ChaseTarget.transform.position, Time.deltaTime * 1.5f);
         }
         else
         {
            player.transform.position = Vector3.MoveTowards(player.transform.position, ChaseTarget.transform.position, Time.deltaTime);
         }
      }
      else
      {
         player.transform.position = Vector3.MoveTowards(player.transform.position, ChaseTarget.transform.position, Time.deltaTime);
      }

      player.TrackBall();

      if (player.PlayerCollider.IsTouchingLayers(LayerMask.GetMask("Football")) && player.GetFootball().transform.localScale.x < 0.16f)
      {
         player.ChangeState(player.state_PlayerKickBall);
      }
   }

   public override void Exit (PlayerController player)
   {

   }
}
