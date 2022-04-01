using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSupportSpots : MonoBehaviour
{
    public GameObject SupportSpot;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSpots(); 
    }

    void GenerateSpots()
    {
        for(int i = -8; i <= 8; i++)
        {
            for(int j = -5; j <= 5; j++)
            {
                Instantiate(SupportSpot, new Vector2(i, j), new Quaternion(0, 0, 0, 1), this.transform);
            }
        }
    }
}
