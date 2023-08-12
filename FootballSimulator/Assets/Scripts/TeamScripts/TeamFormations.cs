using UnityEngine;

public class TeamFormations : MonoBehaviour
{
   public bool defensive;
   public TeamScript team;
   public Tactic newTactic;

   public enum Tactic
   {
      SquareDefending,
      CrossDefending,
      VDefending,
      DiamondAttacking,
      ArrowAttacking,
      SoloPoacherAttacking,
   }

   public class Formation
   {
      public int[] lhs_regions;
      public int[] rhs_regions;

      public Formation (int[] left, int[] right)
      {
         lhs_regions = left;
         rhs_regions = right;
      }
   }

   private Tactic myTactics;

   // Defensive formations
   public Formation f_SquareDefending = new Formation(new int[5] { 20, 11, 31, 13, 33 }, new int[5] { 29, 18, 38, 16, 36 });

   public Formation f_CrossDefending = new Formation(new int[5] { 20, 11, 31, 21, 22 }, new int[5] { 29, 18, 38, 28, 27 });

   public Formation f_VDefending = new Formation(new int[5] { 20, 11, 31, 2, 42 }, new int[5] { 29, 18, 38, 7, 47 });

   // Attacking Formations
   public Formation f_SoloPoacherAttacking = new Formation(new int[5] { 20, 12, 32, 16, 27 }, new int[5] { 29, 17, 37, 13, 22 });

   public Formation f_DiamondAttacking = new Formation(new int[5] { 20, 22, 4, 44, 27 }, new int[5] { 29, 27, 6, 46, 22 });

   public Formation f_ArrowAttacking = new Formation(new int[5] { 20, 16, 36, 27, 28 }, new int[5] { 29, 13, 33, 22, 21 });

   public void ChangeTeamFormation ()
   {
      Formation newFormation = f_SquareDefending;

      newFormation = newTactic switch
      {
         Tactic.SquareDefending => f_SquareDefending,
         Tactic.SoloPoacherAttacking => f_SoloPoacherAttacking,
         Tactic.CrossDefending => f_CrossDefending,
         Tactic.VDefending => f_VDefending,
         Tactic.DiamondAttacking => f_DiamondAttacking,
         Tactic.ArrowAttacking => f_ArrowAttacking,
         _ => f_SquareDefending,
      };

      // Is the team on lhs?
      if (team.lhs_regions)
      {
         if (defensive)
         {
            team.DefendingRegions = newFormation.lhs_regions;

            if (team.GetCurrentState() == team.state_Defending || GameObject.FindGameObjectWithTag("Pitch").GetComponent<SoccerPitchScript>().GameOn == false)
            {
               team.ChangePlayerHomeRegions(team.DefendingRegions);
            }
         }
         else
         {
            team.AttackingRegions = newFormation.lhs_regions;

            if (team.GetCurrentState() == team.state_Attacking)
            {
               team.ChangePlayerHomeRegions(team.AttackingRegions);
            }
         }
      }
      else
      {
         if (defensive)
         {
            team.DefendingRegions = newFormation.rhs_regions;

            if (team.GetCurrentState() == team.state_Defending || GameObject.FindGameObjectWithTag("Pitch").GetComponent<SoccerPitchScript>().GameOn == false)
            {
               team.ChangePlayerHomeRegions(team.DefendingRegions);
            }
         }
         else
         {
            team.AttackingRegions = newFormation.rhs_regions;

            if (team.GetCurrentState() == team.state_Attacking)
            {
               team.ChangePlayerHomeRegions(team.AttackingRegions);
            }
         }
      }
   }
}
