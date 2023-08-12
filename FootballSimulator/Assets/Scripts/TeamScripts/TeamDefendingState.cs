public class TeamDefendingState : State<TeamScript>
{
   public override void Enter (TeamScript team)
   {

      // Reset key players
      team.ResetAllPlayers();

      // Declare the left and right defending regions
      // These will change for the team depending on if we are on the first or second half
      team.ChangePlayerHomeRegions(team.DefendingRegions);
   }

   public override void Execute (TeamScript team)
   {
      if (team.Team_HasBall())
      {
         team.ChangeState(team.state_Attacking);
      }
   }

   public override void Exit (TeamScript team)
   {
      //
   }
}
