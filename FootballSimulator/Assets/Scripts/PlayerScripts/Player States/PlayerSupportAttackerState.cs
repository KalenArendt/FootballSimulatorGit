using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSupportAttackerState : State<PlayerController>
{
    public override void Enter(PlayerController player)
    {
        if (player.playerTeam.SupportingPlayer != null && player.playerTeam.SupportingPlayer != player.gameObject)
        {
            player.playerTeam.SupportingPlayer.GetComponent<PlayerController>().ChangeState(
            player.playerTeam.SupportingPlayer.GetComponent<PlayerController>().state_PlayerReturnHome);
        }

        player.playerTeam.SupportingPlayer = player.gameObject;
    }

    public override void Execute(PlayerController player)
    {
        if (player.playerTeam.BSS != null)
        {
            player.ArriveTarget = player.playerTeam.BSS;

            // Go to the best support spot
            player.transform.position = Vector3.MoveTowards(player.transform.position,
                                         player.ArriveTarget.transform.position, Time.deltaTime * 1.25f);
            //rotate to look at the best support spot
            player.transform.up = Vector2.Lerp(player.transform.up, player.ArriveTarget.transform.position - player.transform.position, 0.025f * Time.deltaTime * 400);

            // If the team loses possesion, return to the home region
            if (!player.playerTeam.Team_HasBall())
            {
                player.ChangeState(player.state_PlayerReturnHome);
                return;
            }

            // If the player is in a good position to shoot, request a pass
            if (player.playerTeam.CanScore(player.transform.position, player.GetComponent<PlayerAttributes>().SHO_Power))
            {
                player.playerTeam.RequestPass(player);
            }

            // If the player arrives at the BSS, request a pass
            if (player.transform.position == player.ArriveTarget.transform.position && player.IsAheadOfAttacker())
            {
                // Player should keep the eyes on the ball
                player.TrackBall();

                // Stop the movement
                player.PlayerRigidBody.velocity = new Vector2(0, 0);

                // Request a pass
                player.playerTeam.RequestPass(player);
            }

            if (player.gameObject == player.playerTeam.ClosestToBall
                && (player.playerTeam.ReceivingPlayer == null)
                && !player.Pitch.GetComponent<SoccerPitchScript>().GoalkeeperHasBall)
            {
                player.ChangeState(player.state_PlayerChase);
                return;
            }

            if (player == player.GetInterceptor()
                && (player.playerTeam.ReceivingPlayer == null)
                && !player.Pitch.GetComponent<SoccerPitchScript>().GoalkeeperHasBall)
            {
                player.ChangeState(player.state_PlayerIntercept);
                return;
            }
        }
    }

    public override void Exit(PlayerController player)
    {
        //throw new System.NotImplementedException();
    }
}
