using System.Collections.Generic;

using UnityEngine;


// Calculates how many opponents are nearby based on a circle collider.
public class OpponentsNearby : MonoBehaviour
{
   private CircleCollider2D PlayerRange;
   public List<Collider2D> collidersList = new List<Collider2D>();
   private int opponentsNearby;

   // Start is called before the first frame update
   private void Start ()
   {
      PlayerRange = GetComponent<CircleCollider2D>();

      opponentsNearby = 0;
   }

   public void Update ()
   {
      opponentsNearby = collidersList.Count;
   }

   private void OnTriggerEnter2D (Collider2D collision)
   {
      if (collision.gameObject.layer == 11 && collision.gameObject.transform.parent != transform.parent.parent)
      {
         if (!collidersList.Contains(collision))
         {
            collidersList.Add(collision);
         }
      }
   }

   private void OnTriggerExit2D (Collider2D collision)
   {
      if (collision.gameObject.layer == 11 && collision.gameObject.transform.parent != transform.parent)
      {
         if (collidersList.Contains(collision))
         {
            collidersList.Remove(collision);
         }
      }
   }

   public int GetOpponnentsNearby ()
   {
      return opponentsNearby;
   }
}
