using UnityEngine;

public class PlayerReturnHomeScript : State<PlayerController>
{
   public override void Enter (PlayerController player)
   {

   }

   public override void Execute (PlayerController player)
   {
      if (player.Pitch.GetComponent<SoccerPitchScript>().GameOn)
      {
         if ((player.gameObject == player.playerTeam.ClosestToBall
             && player.playerTeam.ReceivingPlayer == null
             && !player.Pitch.GetComponent<SoccerPitchScript>().GoalkeeperHasBall)
              || player.playerTeam.ReceivingPlayer == player.gameObject)
         {
            player.ChangeState(player.state_PlayerChase);
            return;
         }
      }

      if (player == player.GetInterceptor()
          && (player.playerTeam.ReceivingPlayer == null)
          && !player.Pitch.GetComponent<SoccerPitchScript>().GoalkeeperHasBall)
      {
         player.ChangeState(player.state_PlayerIntercept);
      }

      if (player.playerTeam.GetCurrentState() != player.playerTeam.state_PrepareForKickOff)
      {
         if (player.isPlayerHome())
         {
            player.ChangeState(player.state_PlayerWait);
            return;
         }
      }

      // Move to home region
      player.transform.position = Vector3.MoveTowards(player.transform.position, player.playerHomeRegion, Time.deltaTime);

      //rotate to look at the home region
      player.transform.up = Vector2.Lerp(player.transform.up, player.playerHomeRegion - new Vector2(player.transform.position.x, player.transform.position.y), 0.025f * Time.deltaTime * 400);
   }

   public override void Exit (PlayerController player)
   {

   }
}
