using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{

    public static MatchManager i;

    int gameCounter;
    public int goalsCounter;

    // Create a singleton

    void Awake()
    {
        if(!i)
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
