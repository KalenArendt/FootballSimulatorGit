using UnityEngine;

public class MatchManager : MonoBehaviour
{

   public static MatchManager i;
   private int gameCounter;
   public int goalsCounter;

   // Create a singleton

   private void Awake ()
   {
      if (!i)
      {
         gameCounter = 0;
         i = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         i.gameCounter++;
         //Debug.Log("Games: " + i.gameCounter + " Goals: " + i.goalsCounter);
         Destroy(gameObject);
      }
   }
}
