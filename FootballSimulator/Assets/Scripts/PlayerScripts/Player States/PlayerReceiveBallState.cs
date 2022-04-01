using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReceiveBallState : State<PlayerController>
{
    public override void Enter(PlayerController player)
    {
        player.playerTeam.ReceivingPlayer = player.gameObject;

        //player.playerTeam.ControllingPlayer = player.gameObject;

        player.ChangeState(player.state_PlayerChase);
    }

    public override void Execute(PlayerController player)
    {
        
    }

    public override void Exit(PlayerController player)
    {

    }
}
