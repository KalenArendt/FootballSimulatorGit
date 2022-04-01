using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballZAxis : MonoBehaviour
{
    Rigidbody2D Z_RigidBody;

    // Start is called before the first frame update
    void Start()
    {
        Z_RigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 0)
        {
            float bounceForce = Mathf.Abs(Z_RigidBody.velocity.y)/1.25f;

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
