using UnityEngine;

public class FootballZAxis : MonoBehaviour
{
   private Rigidbody2D Z_RigidBody;

   // Start is called before the first frame update
   private void Start ()
   {
      Z_RigidBody = GetComponent<Rigidbody2D>();
   }

   // Update is called once per frame
   private void Update ()
   {
      if (transform.position.y <= 0)
      {
         var bounceForce = Mathf.Abs(Z_RigidBody.velocity.y)/1.25f;

         if (bounceForce > 0.2)
         {
            Z_RigidBody.velocity = new Vector2(0, bounceForce);
         }

         else

         {
            Z_RigidBody.velocity = new Vector2(0, 0);
            transform.position = new Vector2(transform.position.x, 0);
         }
      }
   }
}
