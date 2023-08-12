using UnityEngine;

public class PlayerWaitState : State<PlayerController>
{
   public override void Enter (PlayerController player)
   {
      //
   }

   public override void Execute (PlayerController player)
   {
      player.PlayerRigidBody.velocity = Vector2.zero;

      // Look at the ball
      player.TrackBall();

      // Check if we can request a pass
      if (player.playerTeam.Team_HasBall()
          && player.gameObject != player.playerTeam.ControllingPlayer
          && player.IsAheadOfAttacker())
      {
         player.playerTeam.RequestPass(player);

         return;
      }

      if (player.Pitch.GetComponent<SoccerPitchScript>().GameOn)
      {
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

         if (player != player.playerTeam.ReceivingPlayer
             && player != player.playerTeam.SupportingPlayer
             && player != player.playerTeam.InterceptingPlayer
             && player != player.playerTeam.ClosestToBall)
         {
            if (!player.isPlayerHome())
            {
               player.ChangeState(player.state_PlayerReturnHome);
               return;
            }
         }
      }
   }

   public override void Exit (PlayerController player)
   {
      //
   }
}
