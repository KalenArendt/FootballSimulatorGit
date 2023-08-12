using UnityEngine;

public class PlayerInterceptState : State<PlayerController>
{
   public override void Enter (PlayerController player)
   {
      player.playerTeam.InterceptingPlayer = player.gameObject;
   }

   public override void Execute (PlayerController player)
   {

      GameObject ChaseTarget = null;

      GameObject[] Teams = GameObject.FindGameObjectsWithTag("Team");

      foreach (GameObject item in Teams)
      {
         if (item != player.playerTeam)
         {
            ChaseTarget = item.GetComponent<TeamScript>().ControllingPlayer;
         }
      }

      if (ChaseTarget != null)

      {
         Vector3 toDirection = ChaseTarget.GetComponent<PlayerController>().GoalTarget.transform.position - ChaseTarget.transform.position;

         Vector3 interceptPoint = ChaseTarget.transform.position + (toDirection.normalized * 1.2f);

         player.transform.position = Vector3.MoveTowards(player.transform.position, interceptPoint, Time.deltaTime);

         player.transform.up = Vector2.Lerp(player.transform.up,
             new Vector2(interceptPoint.x - player.transform.position.x, interceptPoint.y - player.transform.position.y), 0.025f * Time.deltaTime * 400);

         if (Vector2.Distance(ChaseTarget.transform.position, ChaseTarget.GetComponent<PlayerController>().GoalTarget.transform.position) < 5.0f)
         {
            player.ChangeState(player.state_PlayerChase);
            return;
         }
      }
      else
      {
         player.transform.position = Vector3.MoveTowards(player.transform.position, player.GetFootball().transform.position, Time.deltaTime);

         player.TrackBall();
      }

      if (player.gameObject == player.playerTeam.ClosestToBall
          && (player.playerTeam.ReceivingPlayer == null)
          && !player.Pitch.GetComponent<SoccerPitchScript>().GoalkeeperHasBall)
      {
         player.ChangeState(player.state_PlayerChase);
         return;
      }

      if (player.PlayerCollider.IsTouchingLayers(LayerMask.GetMask("Football")) && player.GetFootball().transform.localScale.x < 0.16f)
      {
         player.ChangeState(player.state_PlayerKickBall);
         return;
      }
   }

   public override void Exit (PlayerController player)
   {
      player.playerTeam.InterceptingPlayer = null;
   }
}
