using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamAttackingState : State<TeamScript>
{
    public override void Enter(TeamScript team)
    {
        // Declare the left and right defending regions
        // These will change for the team depending on if we are on the first or second half

        // Check if the team is on the right or on the left of the pitch.
        team.ChangePlayerHomeRegions(team.AttackingRegions);

        team.StartSSCalculator();
    }

    public override void Execute(TeamScript team)
    {
        if (!team.Team_HasBall())
        {
            team.ChangeState(team.state_Defending);
        }
    }

    public override void Exit(TeamScript team)
    {
        team.StopSSCalculator();
    }
}
