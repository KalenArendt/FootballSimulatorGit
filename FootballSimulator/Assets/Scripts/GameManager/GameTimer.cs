using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text TimerText;
    public Text ExtraTimeText;

    string String_Seconds;
    string String_Minutes;

    float TimerSeconds;
    int TimerMinutes;
    int HalfMinutes;

    string ET_String_Seconds;
    string ET_String_Minutes;

    float ET_Seconds;
    int ET_Minutes;
    int AddedTime;


    public bool SwitchSides = false;

    // Start is called before the first frame update
    void Start()
    {
        TimerText = GetComponent<Text>();
        ExtraTimeText = transform.GetChild(0).GetComponent<Text>();

        TimerSeconds = 0;
        TimerMinutes = 0;
        HalfMinutes = 0;

        AddedTime = -1;

        ET_Seconds = 0;
        ET_Minutes = 0;
    }

    int[] mixArray (int [] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rnd = Random.Range(0, array.Length);

            int temp = array[rnd];
            array[rnd] = array[i];
            array[i] = temp;
        }

            return array;
    }

    int GetExtraTime()
    {
        int[] numbers = {0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 5, 5, 6, 7, 8, 9, 10, };

        numbers = mixArray(numbers);

        int ExtraTime = numbers[Random.Range(0, numbers.Length - 1)];

        return ExtraTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Manage the display of minutes.
        if (TimerMinutes / 10 == 0)
        {
            String_Minutes = "0" + TimerMinutes.ToString();
        }
        else
        {
            String_Minutes = TimerMinutes.ToString();
        }

        // Manage the display of seconds
        if (Mathf.RoundToInt(TimerSeconds) / 10 == 0)
        {
            String_Seconds = "0" + TimerSeconds.ToString("F0"); ;
        }
        else
        {
            String_Seconds = TimerSeconds.ToString("F0");
        }

        // Add the minutes
        if (TimerSeconds > 59)
        {
            if (TimerMinutes < 90)
            {
                TimerMinutes++;
                HalfMinutes++;
            }
            TimerSeconds = 0;
        }

        // If we are not at the end of the half
        // Increase the seconds
        if(HalfMinutes != 45 && TimerMinutes < 90)
        {
            TimerSeconds += Time.deltaTime * 100;
        }
        else  // Show the extra time
        {
            TimerSeconds = 0;

            // Randomly generate the added time;
            if (AddedTime == -1)
            {
                ET_Minutes = 0;
                AddedTime = GetExtraTime();
                //Debug.Log(AddedTime);
                ExtraTimeText.enabled = true;
            }
            else
            {
                ExtraTimeText.text = "+" + ET_String_Minutes + ":" + ET_String_Seconds;

                if (Mathf.RoundToInt(ET_Seconds) / 10 == 0)
                {
                    ET_String_Seconds = "0" + ET_Seconds.ToString("F0");
                }
                else
                {
                    ET_String_Seconds = ET_Seconds.ToString("F0");
                }

                if (ET_Minutes / 10 == 0)
                {
                    ET_String_Minutes = "0" + ET_Minutes.ToString();
                }
                else
                {
                    ET_String_Minutes = ET_Minutes.ToString();
                }

                if (ET_Minutes != AddedTime) // Increase our timer
                {
                    ET_Seconds += Time.deltaTime * 100;

                    if (ET_Seconds > 59)
                    {
                        ET_Minutes++;
                        ET_Seconds = 0;
                    }
                }
                else if (TimerMinutes != 90 && SwitchSides)
                {
                    ET_Seconds = 0f;
                    AddedTime = -1;

                    ExtraTimeText.enabled = false;

                    HalfMinutes = 0;
                }
            }
        }

        TimerText.text = String_Minutes + ":" + String_Seconds;
    }
}
