using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

   public static GameManager i;

   // Create a singleton

   //void Awake()
   //{
   //    if (!i)
   //    {
   //        i = this;
   //        DontDestroyOnLoad(gameObject);
   //    }
   //    else
   //    {
   //        Destroy(gameObject);
   //    }
   //}

   // Start is called before the first frame update
   private void Start ()
   {

   }

   public void QuitGame ()
   {
      // save any game data here
#if UNITY_EDITOR
      // Application.Quit() does not work in the editor so
      // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
      UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
   }

   public void GoToMatch ()
   {
      SceneManager.LoadScene(0);
   }
}
