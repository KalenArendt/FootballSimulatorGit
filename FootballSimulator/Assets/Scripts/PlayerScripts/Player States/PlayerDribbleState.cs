using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDribbleState : State<PlayerController>
{
    public override void Enter(PlayerController player)
    {
        player.playerTeam.ControllingPlayer = player.gameObject;
    }

    public override void Execute(PlayerController player)
    {
        // See if the player is facing its own goal
        float dot = Vector3.Dot(player.transform.up, player.GoalTarget.transform.position);

        if(dot < 0)
        {
            if (player.GetFootball().GetComponent<Rigidbody2D>().velocity.magnitude < 1f)
            {
                // Rotate the player facing direction
                // Then kick the ball in that direction
                float kickAngle = 10f * Time.deltaTime * 400;

                Vector2 kickDir = player.transform.up + new Vector3(kickAngle * Mathf.Sign(player.transform.up.x * -1), kickAngle * Mathf.Sign(player.transform.up.y), 0.0f);

                // Kick the ball
                float kickingForce = 0.25f * Time.deltaTime * 400;

                player.GetFootball().GetComponent<FootballScript>().KickBall(kickDir.normalized, kickingForce);
            }
        }
        else
        {
            if (player.GetFootball().GetComponent<Rigidbody2D>().velocity.magnitude < 1f)
            {
                Vector2 kickDir = player.GoalTarget.transform.position - player.transform.position;

                // Kick the ball
                float kickingForce = 4f * Time.deltaTime * 400;

                player.GetFootball().GetComponent<FootballScript>().KickBall(kickDir.normalized, kickingForce);
            }
        }

        // Chase the ball after kicking it
        player.ChangeState(player.state_PlayerChase);
    }

    public override void Exit(PlayerController player)
    {
        
    }
}
