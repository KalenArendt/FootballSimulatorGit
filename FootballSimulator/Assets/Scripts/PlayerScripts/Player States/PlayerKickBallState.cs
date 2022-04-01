using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickBallState : State<PlayerController>
{
    public override void Enter(PlayerController player)
    {
        // Change the team that has possesion
        GameObject[] Teams = GameObject.FindGameObjectsWithTag("Team");

        foreach(GameObject item in Teams)
        {
            if(item == player.playerTeam.gameObject)
            {
                item.GetComponent<TeamScript>().m_HasBall = true;
            }
            else
            {
                if (item.GetComponent<TeamScript>().m_HasBall == true)
                {
                    item.GetComponent<TeamScript>().m_HasBall = false;

                    item.GetComponent<TeamScript>().ResetAllPlayers();

                    player.GetFootball().GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
                }
            }
        }

        // this is the controlling player
        player.playerTeam.ControllingPlayer = player.gameObject;

        //
        if (player.playerTeam.ReceivingPlayer == player.gameObject)
        {
            player.GetFootball().GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
            player.playerTeam.ReceivingPlayer = null;
        }

        // If the player is not ready for next kick, change the state
        if(!player.isReadyForNextKick() || (player.GetFootball().transform.localScale.x > 0.16f && !player.isGoalkeeper))
        {
            player.ChangeState(player.state_PlayerChase);
        }
    }

    // Method that calculates a kick error based on the current target and player attributes.
    Vector2 KickError(PlayerController player, Vector2 Target, float attribute)
    {
        // Distance to target affects accuracy
        float targetDistance = Vector2.Distance(Target, player.GetFootball().transform.position);

        // Calculate the error angle based on players' ability, distance and a random error factor
        float ErrorAngle = player.GetFootball().GetComponent<FootballScript>().CalculateErrorAngle(attribute, targetDistance);

        // Calculate the new direction based on the error angle
        Vector2 KickDir = (Target - new Vector2(player.GetFootball().transform.position.x, player.GetFootball().transform.position.y));

        KickDir = new Vector2((KickDir.x * Mathf.Cos(ErrorAngle * Mathf.Deg2Rad) - KickDir.y * Mathf.Sin(ErrorAngle * Mathf.Deg2Rad)),
                                (KickDir.x * Mathf.Sin(ErrorAngle * Mathf.Deg2Rad) + KickDir.y * Mathf.Cos(ErrorAngle * Mathf.Deg2Rad)));

        return KickDir;
    }

    public override void Execute(PlayerController player)
    {
        //calculate the dot product of the vector pointing to the ball
        //and the player's heading

        // Direction to ball
        Vector2 ToBall = player.GetFootball().transform.position - player.transform.position;

        // transform.up is the player's facing direction
        float dot = Vector2.Dot(player.transform.up, ToBall.normalized);

        // If the ball
        // has a receiver
        // is at goalkeeper
        // is behind the player
        // continue chasing the ball
        if (player.playerTeam.ReceivingPlayer != null
            || player.Pitch.GetComponent<SoccerPitchScript>().GoalkeeperHasBall
            || (dot < 0))
        {
            player.ChangeState(player.state_PlayerChase);
        }

        dot = Mathf.Clamp(dot, -1f, 1f);

        // Adjust the kicking force depending on the ball position in relation to the player's facing direction.
        float power = (player.GetComponent<PlayerAttributes>().SHO_Power * dot) ;

        // Check if a shot at goal is possible
        Vector2 BallTarget = Vector2.zero;

        /* Attempt a shot at the goal */
        #region
        //if it’s determined that the player could score a goal from this position
        //OR if he should just kick the ball anyway, the player will attempt
        //to make the shot

        float potShot = Random.Range(0.0f, 10.0f);

        float potShotLimit = 0.1f;

        potShotLimit /= (Vector3.Distance(player.transform.position, player.GoalTarget.transform.position));

        if (player.playerTeam.CanScore(player.transform.position, power, ref BallTarget)
            || potShot < 0.1f)
        {
            // Reset Receiver
            player.playerTeam.ReceivingPlayer = null;

            player.GetFootball().GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);

            player.kickCooldown = 0.5f;
            // Take the shot at goal

            // Add some error to the kick
            Vector2 kickDir = KickError(player, BallTarget, player.GetComponent<PlayerAttributes>().SHO_Finishing);

            // Kick the ball in the given direction
            player.GetFootball().GetComponent<FootballScript>().KickBall(kickDir.normalized, power);

            // Call the find support function
            player.FindSupport();

            player.ChangeState(player.state_PlayerWait);

            return;
        }
        #endregion

        /* Attempt a pass to a player */
        #region

        PlayerController Receiver = null;

        // If we can find a valid pass
        if (player.IsThreatened() && player.playerTeam.FindPass(player, ref Receiver, ref BallTarget, power, player.minPassDistance))
        {
            if(!player.OpponenetsForward() && player.GetComponent<PlayerAttributes>().DRI_Agility > 
                30 * player.transform.GetComponentInChildren<OpponentsNearby>().GetOpponnentsNearby())
            {
                // Reset Receiver
                player.playerTeam.ReceivingPlayer = null;

                // Find support
                player.FindSupport();

                // Change state to dribble
                player.ChangeState(player.state_PlayerDribble);

                return;
            }

            player.GetFootball().GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);

            player.kickCooldown = 0.5f;
            // Pass the ball

            // Add some error to the pass
            Vector2 kickDir = BallTarget - new Vector2(player.GetFootball().transform.position.x, player.GetFootball().transform.position.y);

            // Kick the ball in the given direction
            player.GetFootball().GetComponent<FootballScript>().KickBall(kickDir.normalized, power);

            // Send a message to the receiver
            Receiver.ChangeState(Receiver.state_PlayerReceiveBall);

            // Find Support
            player.FindSupport();

            // Change state to wait
            player.ChangeState(player.state_PlayerWait);

            return;
        }
        // Else just clear the ball
        else if(player.OpponenetsForward() || player.isGoalkeeper)
        {
            player.GetFootball().GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);

            player.kickCooldown = 0.5f;
            // Clear the ball

            if(Receiver == null)
            {
                BallTarget = player.GoalTarget.transform.position;
            }

            // Add some error to the kick
            Vector2 kickDir = KickError(player, BallTarget, player.GetComponent<PlayerAttributes>().SHO_Finishing);

            // Kick the ball in the given direction
            if (player.isGoalkeeper)
            {
                player.GetFootball().GetComponent<FootballScript>().KickBall(kickDir.normalized, power * 1.25f, 8.0f);
            }
            else
            {
                player.GetFootball().GetComponent<FootballScript>().KickBall(kickDir.normalized, power);
            }

            // Send a message to the receiver
            if (Receiver)
            {
                Receiver.ChangeState(Receiver.state_PlayerReceiveBall);
            }
            else
            {
                player.playerTeam.ReceivingPlayer = null;
            }

            // Find Support
            player.FindSupport();

            // Change state to wait
            player.ChangeState(player.state_PlayerWait);

            return;
        }
        #endregion
        // If the player should not shoot or pass, dribble the ball upfield
        else
        {
            // Reset Receiver
            player.playerTeam.ReceivingPlayer = null;

            // Find support
            player.FindSupport();

            // Change state to dribble
            player.ChangeState(player.state_PlayerDribble);

            return;
        }
    }

    public override void Exit(PlayerController player)
    {
        //
    }
}
