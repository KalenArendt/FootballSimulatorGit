﻿using UnityEngine;

public class SupportSpotScript : MonoBehaviour
{
   public float SS_score;


   // Start is called before the first frame update
   private void Start ()
   {
      SS_score = 1.0f;
   }

   // Update is called once per frame
   private void Update ()
   {
      transform.localScale = new Vector2(SS_score, SS_score);
   }
}
