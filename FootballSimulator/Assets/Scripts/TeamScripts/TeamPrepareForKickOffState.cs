using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPrepareForKickOffState : State<TeamScript>
{
    GameObject[] Teams;

    public override void Enter(TeamScript team)
    {
        Teams = GameObject.FindGameObjectsWithTag("Team");

        // Reset the key players
        team.ClosestToBall = null;
        team.ControllingPlayer = null;
        team.ReceivingPlayer = null;
        team.SupportingPlayer = null;

        // Send the players in their home regions
        team.ReturnAllPlayersHome();
    }

    public override void Execute(TeamScript team)
    {
        if (Teams[0].GetComponent<TeamScript>().Team_AllPlayersHome()
            && Teams[1].GetComponent<TeamScript>().Team_AllPlayersHome()
            && team.FootBall.transform.position == Vector3.zero)
        {
            Teams[0].GetComponent<TeamScript>().ChangeState(Teams[0].GetComponent<TeamScript>().state_Defending);
            Teams[1].GetComponent<TeamScript>().ChangeState(Teams[1].GetComponent<TeamScript>().state_Defending);

            GameObject.FindGameObjectWithTag("Pitch").GetComponent<SoccerPitchScript>().GameOn = true;
        }
    }

    public override void Exit(TeamScript team)
    {
        // No exit for this state
    }
}
