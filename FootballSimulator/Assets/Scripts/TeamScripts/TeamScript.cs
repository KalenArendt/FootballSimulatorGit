using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TeamScript : MonoBehaviour
{

    // FSM Components
    #region
    public string CurrState;

    private StateMachine<TeamScript> Team_FSM;

    public State<TeamScript> state_Defending, state_Attacking, state_PrepareForKickOff;

    Transition<TeamScript>.condition cond_HasBall, cond_PrepareForKickOff, cond_AllPlayersHome;
    #endregion

    // Dictionary
    #region
    [Serializable]
    public class TeamStatesDictionary : SerializableDictionary<TransitionClassTeam, float> { }

    [Tooltip("The state transition and transition time")]
    public TeamStatesDictionary myTeamStates;

    [Serializable]
    public class TransitionClassTeam
    {
        [Tooltip("The current state")]
        public States cState;

        [Tooltip("The condition for the transition to happen")]
        public Conditions cond;

        [Tooltip("Bool tupe (true/false)")]
        public bool Bool;

        [Tooltip("Next state to transition to")]
        public States nState;
    }

    public enum States
    {
        PrepareForKickOff,
        Defending,
        Attacking
    }

    public enum Conditions
    {
        HasBall,
        AllPlayersHome,
        PrepareForKickOff
    }


    // Go through the dictionary of transitions and set them accordingly
    public void ConfigureTransitions(TeamStatesDictionary myDictionary)
    {
        foreach (KeyValuePair<TransitionClassTeam, float> keyValuePair in myDictionary)
        {
            enumToState(keyValuePair.Key.cState).Transitions.Add(new Transition<TeamScript>(enumToCondition(keyValuePair.Key.cond), keyValuePair.Key.Bool, enumToState(keyValuePair.Key.nState)));
        }
    }

    // Enum to state function
    State<TeamScript> enumToState(States myState)
    {
        switch (myState.ToString())
        {
            case "PrepareForKickOff":
                return state_PrepareForKickOff;
            case "Defending":
                return state_Defending;
            case "Attacking":
                return state_Attacking;
            default:
                return null;
        }
    }

    // Enum to condition function
    Transition<TeamScript>.condition enumToCondition(Conditions myCondition)
    {
        switch (myCondition.ToString())
        {
            case "HasBall":
                return cond_HasBall;
            case "AllPlayersHome":
                return cond_AllPlayersHome;
            case "PrepareForKickOff":
                return cond_PrepareForKickOff;
            default:
                return null;
        }
    }
    #endregion

    // Team Components
    #region
    GameObject[] teamPlayers = new GameObject[5];

    public bool lhs_regions = true;

    public GameObject FootBall;

    public GameObject BSS = null;
    public GameObject ClosestToBss = null;

    // Player roles
    public GameObject ClosestToBall = null;
    public GameObject ControllingPlayer = null;
    public GameObject ReceivingPlayer = null;
    public GameObject SupportingPlayer = null;
    public GameObject InterceptingPlayer = null;

    // Formations
    // Defending
    public int[] DefendingRegions;
    public int[] AttackingRegions;

    // If the team has the ball
    public bool m_HasBall = false;

    // The goal target
    public GameObject goalTarget;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if(lhs_regions)
        {
            DefendingRegions = new int[5] { 20, 11, 31, 13, 33 };
            AttackingRegions = new int[5] { 20, 12, 32, 16, 27 };
        }
        else
        {
            DefendingRegions = new int[5] { 29, 18, 38, 16, 36 };
            AttackingRegions = new int[5] { 29, 17, 37, 13, 22 };
        }

        FootBall = GameObject.Find("Football");

        BSS = null;

        for(int i = 0; i < transform.childCount; i++)
        {
            teamPlayers[i] = transform.GetChild(i).gameObject;
        }

        StartSSCalculator();

        Team_FSM.Configure(this, state_Defending);

        ChangeState(state_PrepareForKickOff);
    }

    public void ChangeState(State<TeamScript> e)
    {
        Team_FSM.ChangeState(e);
    }

    public State<TeamScript> GetCurrentState()
    {
        return Team_FSM.GetCurrentState();
    }

    private void Awake()
    {
        // Configure FSM
        #region 

        Team_FSM = new StateMachine<TeamScript>();

        // Set up the states
        state_PrepareForKickOff = new TeamPrepareForKickOffState();

        state_Defending = new TeamDefendingState();

        state_Attacking = new TeamAttackingState();

        // Set up the conditions
        cond_HasBall = Team_HasBall;

        cond_AllPlayersHome = Team_AllPlayersHome;

        cond_PrepareForKickOff = Team_prepareForKickOff;

        // Configure the state transitions 
        ConfigureTransitions(myTeamStates);

        //Team_FSM.Configure(this, state_Defending);
        #endregion
    }

    // Returns an array of all team players
    public GameObject[] GetTeamPlayers()
    {
        return teamPlayers;
    }

    // Set the home regions for the players
    public void ChangePlayerHomeRegions(int[] m_regions)
    {
        for(int i = 0; i < teamPlayers.Length; i++)
        {
            if (teamPlayers[i] != null)
            {
                teamPlayers[i].GetComponent<PlayerController>().playerHomeRegion = 
                    teamPlayers[i].GetComponent<PlayerController>().Pitch.GetComponent<SoccerPitchScript>().subRegions[m_regions[i]].center;
            }
        }
    }

    public void ResetAllPlayers()
    {
        ControllingPlayer = null;
        ReceivingPlayer = null;
        SupportingPlayer = null;
        ClosestToBall = null;
        ClosestToBss = null;

        foreach(GameObject item in teamPlayers)
        {
            PlayerController player = item.GetComponent<PlayerController>();

            if (player.GetCurrentState() != player.state_PlayerChase)
            {
                player.kickCooldown = 0.0f;
                player.ChangeState(player.state_PlayerReturnHome);
            }
        }
    }

    // Send the team players in their home regions
    public void ReturnAllPlayersHome()
    {
        foreach(GameObject player in teamPlayers)
        {
            player.GetComponent<PlayerController>().ChangeState(player.GetComponent<PlayerController>().state_PlayerReturnHome);
        }
    }

    // Finds the clossest player from a target
    public GameObject FindClosest(GameObject target, bool avoidControllingPPlayer)
    {
        float minDistance = Mathf.Infinity;

        GameObject ClosestToTarget = null;

        foreach(GameObject currentPlayer in teamPlayers)
        {
            if((avoidControllingPPlayer == true && currentPlayer == ControllingPlayer))
            {
                continue;
            }
            if (currentPlayer != null)
            {
                float distanceToTarget = Vector2.Distance(currentPlayer.transform.position, target.transform.position);

                if (distanceToTarget < minDistance && currentPlayer.activeInHierarchy)
                {
                    minDistance = distanceToTarget;
                    ClosestToTarget = currentPlayer;
                }
            }
        }

        return ClosestToTarget;
    }

    // Method that starts the support spot calculator
    public void StartSSCalculator()
    {
        InvokeRepeating("SupportSpotCalculator", 0.0f, 0.5f);
    }

    // Method that stops the support spot calculator
    public void StopSSCalculator()
    {
        CancelInvoke("SupportSpotCalculator");
    }

    public IEnumerator BSSCoroutine()
    {
        GameObject bestSupportSpot = null;
        float BestScore = 0.0f;

        GameObject[] supportSpots = GameObject.FindGameObjectsWithTag("SupportSpot");

        foreach (GameObject currentSpot in supportSpots)
        {
            // Reset the previous score
            currentSpot.GetComponent<SupportSpotScript>().SS_score = 1.0f;

            // Check if the spot is in an advenced position
            //if (ControllingPlayer != null)
            //{
            //    if (Vector2.Dot(ControllingPlayer.transform.position, currentSpot.transform.position) < 0)
            //    {
            //        currentSpot.GetComponent<SupportSpotScript>().SS_score += 1.0f;
            //    }
            //}

            if (ControllingPlayer != null)
            {
                // Check if that position is safe from all opponents
                if (IsPassSafe(ControllingPlayer.transform.position, currentSpot.transform.position, null, 100.0f))
                {
                    currentSpot.GetComponent<SupportSpotScript>().SS_score += 2.0f;
                }

                // Check if that position is a good shooting position
                if (CanScore(currentSpot.transform.position, 100.0f))
                {
                    currentSpot.GetComponent<SupportSpotScript>().SS_score += 1.0f;
                }

                //calculate how far this spot is away from the controlling player
                float optimalDistance = 3.0f;

                float distance = Vector2.Distance(ControllingPlayer.transform.position, currentSpot.transform.position);

                float temp = Mathf.Abs(optimalDistance - distance);

                if (temp < optimalDistance)
                {
                    currentSpot.GetComponent<SupportSpotScript>().SS_score += (optimalDistance - temp) / optimalDistance;
                }
            }

            if (currentSpot.GetComponent<SupportSpotScript>().SS_score > BestScore)
            {
                bestSupportSpot = currentSpot;
                BestScore = currentSpot.GetComponent<SupportSpotScript>().SS_score;
            }

            yield return new WaitForSeconds(0.0005f);
        }
        BSS = bestSupportSpot;
    }
    // Find the best support spot for this team
    // Programming Game AI by Example - Mat Buckland
    public GameObject SupportSpotCalculator()
    {
        StartCoroutine(BSSCoroutine());

        return BSS;
    }

    // Check if a pass would be safe from all opponents

    // Method that checks if a pass is safe from a specific opponent
    // This will be called for each opponent when needed
    bool isPassSafeFromOpponent(Vector2 from, Vector2 target, GameObject Receiver, GameObject opponent, float PassingForce)
    {
        // move opponent into local space
        Vector2 ToTarget = target - from;
        Vector2 ToTargetNormalized = ToTarget.normalized;
        Vector2 ToOpponent = new Vector2(opponent.transform.position.x, opponent.transform.position.y) - from;
        Vector2 ToOppNormalized = ToOpponent.normalized;

        if(Vector2.Dot(ToOppNormalized, ToTargetNormalized) < 0)
        {
            return true;
        }

        // See if the opponent is farther away than the target
        if(Vector2.Distance(from, target) < Vector2.Distance(opponent.transform.position, from))
        {
            if(Receiver != null)
            {
                if(Vector2.Distance(target, opponent.transform.position) > Vector2.Distance(target, Receiver.transform.position))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        Vector3 fromPos3 = new Vector3(from.x, from.y, 0.0f);
        Vector3 targetPos3 = new Vector3(target.x, target.y, 0.0f);

        // Projection of a point onto a line
        // https://stackoverflow.com/questions/51905268/how-to-find-closest-point-on-line
        // Dot product solution
        /* 
        //Get heading
        Vector2 heading = (target - from);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        Vector2 OpponentPos = new Vector2(opponent.transform.position.x, opponent.transform.position.y);

        //Do projection from the point but clamp it
        Vector2 lhs = OpponentPos - from;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        Vector2 proj = from + heading * dotP;
         */

        // Check if the opponent can intercept the ball in the given time
        // Unity Project solution
        Vector3 Projection = fromPos3 + Vector3.Project(opponent.transform.position - fromPos3, targetPos3 - fromPos3);

        float TimeForBall = FootBall.GetComponent<FootballScript>().TimeToCoverDistance(from, Projection, PassingForce);

        float maxDistanceTravelled = Vector3.MoveTowards(opponent.transform.position, Projection, Time.deltaTime).normalized.magnitude;

        float reach = TimeForBall + opponent.GetComponentInChildren<CircleCollider2D>().radius * 0.075f /*+ FootBall.GetComponent<CircleCollider2D>().radius*/;

        if (Vector2.Distance(opponent.transform.position, Projection) > reach)
        {
            return true;
        }

        return false;
    }

    bool IsPassSafe(Vector2 from, Vector2 TargetPos, GameObject receiver, float power)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> opponents = new List<GameObject>();

        foreach(GameObject player in players)
        {
            if(player.transform.parent != this.transform)
            {
                opponents.Add(player);
            }
        }

        foreach(GameObject opponent in opponents)
        {
            if(isPassSafeFromOpponent(from, TargetPos, receiver, opponent, power) == false)
            {
                return false;
            }
        }

        return true;
    }

    // Method that checks if we can shoot the ball at a goal target
    public bool CanScore(Vector2 from, float power, ref Vector2 ShotTarget)
    {
        int numAttempts = 10;

        for(int i = numAttempts; i > 0; i--)
        {
            ShotTarget = goalTarget.transform.GetChild(0).transform.position;

            float maxYVal = 0.5f;

            float minYVal = -0.5f;

            ShotTarget.y = UnityEngine.Random.Range(minYVal, maxYVal);

            float TimeForBall = FootBall.GetComponent<FootballScript>().TimeToCoverDistance(from, ShotTarget, power);

            if (TimeForBall > 0)
            {
                if (IsPassSafe(from, ShotTarget, null, 175.0f))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Overload the CanScore function to take in only two parameters
    public bool CanScore(Vector2 from, float power)
    {
        int numAttempts = 10;

        for (int i = numAttempts; i > 0; i--)
        {
            Vector2 ShotTarget = goalTarget.transform.GetChild(0).transform.position;

            float maxYVal = 0.5f;

            float minYVal = -0.5f;

            ShotTarget.y = UnityEngine.Random.Range(minYVal, maxYVal);

            float TimeForBall = FootBall.GetComponent<FootballScript>().TimeToCoverDistance(from, ShotTarget, power);

            if (TimeForBall > 0)
            {
                if (IsPassSafe(from, ShotTarget, null, 100.0f))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Method that checks if the player can pass to any teammate
    // and stores that reference
    public bool FindPass(PlayerController Passer,
                        ref PlayerController receiver,
                        ref Vector2 PassTarget,
                        float power,
                        float minPassDistance)
    {
        float max_Desirability = 0f;

        GameObject mostAdvancedPlayer = null;

        receiver = null;

        foreach (GameObject currentPlayer in teamPlayers)
        {
            // Passer to Receiver distance
            float thisDTP = Vector2.Distance(Passer.transform.position, currentPlayer.transform.position);

            if (currentPlayer != Passer.gameObject && currentPlayer.gameObject.activeInHierarchy == true && thisDTP >= minPassDistance)
            {
                if(currentPlayer.GetComponent<PlayerController>().isGoalkeeper)
                {
                    if(thisDTP > 5.0f)
                    {
                        continue;
                    }
                }

                float current_Desirability = 0;

                // Receiver Distance to goal
                float teammateDTG = Vector2.Distance(currentPlayer.GetComponent<PlayerController>().GoalTarget.transform.position, currentPlayer.transform.position);

                if(mostAdvancedPlayer != null && 
                    teammateDTG < Vector2.Distance(mostAdvancedPlayer.GetComponent<PlayerController>().GoalTarget.transform.position, mostAdvancedPlayer.transform.position)
                    && teammateDTG < Vector2.Distance(mostAdvancedPlayer.GetComponent<PlayerController>().GoalTarget.transform.position, Passer.transform.position) -3.0f)
                {
                    mostAdvancedPlayer = currentPlayer;
                }
                else if(mostAdvancedPlayer == null)
                {
                    mostAdvancedPlayer = currentPlayer;
                }

                // Get the best option formula
                //          Based on Ability
                //          Receiver distance to goal
                //          Distance from player to teammate

                float passPower = thisDTP < 2 ? Passer.GetComponent<PlayerAttributes>().PAS_Short : Passer.GetComponent<PlayerAttributes>().PAS_Long;

                if (IsPassSafe(Passer.transform.position, currentPlayer.transform.position, currentPlayer, passPower))
                {
                    current_Desirability = 1.0f / teammateDTG;
                }

                if (current_Desirability > max_Desirability)
                {
                    max_Desirability = current_Desirability;
                    receiver = currentPlayer.GetComponent<PlayerController>();
                    PassTarget = currentPlayer.transform.position;
                }
            }
        }

        if(receiver != null)
        {
            return true;
        }
        else
        {
            receiver = mostAdvancedPlayer.GetComponent<PlayerController>();
            PassTarget = mostAdvancedPlayer.transform.position;

            return false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Team_FSM != null)
        {
            Team_FSM.Update();
        }

        CurrState = Team_FSM.GetCurrentState().ToString();

        ClosestToBall = FindClosest(FootBall, false);

        if (BSS != null)
        {
            ClosestToBss = FindClosest(BSS, true);
        }
    }

    public bool Team_HasBall()
    {
        return m_HasBall;
    }

    public bool Team_AllPlayersHome()
    {
        foreach(GameObject player in teamPlayers)
        {
            if(player.gameObject != null && player.gameObject.activeInHierarchy && !player.GetComponent<PlayerController>().isPlayerHome())
            {
                return false;
            }
        }
        return true;
    }

    public bool Team_prepareForKickOff()
    {
        return false;
    }

    public void RequestPass(PlayerController player)
    {
        if (!player.IsThreatened() &&
            IsPassSafe(ControllingPlayer.transform.position,
            player.transform.position, player.gameObject, 100.0f))
        {
            ControllingPlayer.GetComponent<PlayerController>().PassToTarget(player);
        }
    }
}
