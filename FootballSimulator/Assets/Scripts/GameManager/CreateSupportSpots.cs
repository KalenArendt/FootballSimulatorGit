using UnityEngine;

public class CreateSupportSpots : MonoBehaviour
{
   public GameObject SupportSpot;

   // Start is called before the first frame update
   private void Start ()
   {
      GenerateSpots();
   }

   private void GenerateSpots ()
   {
      for (var i = -8; i <= 8; i++)
      {
         for (var j = -5; j <= 5; j++)
         {
            Instantiate(SupportSpot, new Vector2(i, j), new Quaternion(0, 0, 0, 1), transform);
         }
      }
   }
}
