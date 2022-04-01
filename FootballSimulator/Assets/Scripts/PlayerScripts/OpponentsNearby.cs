using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Calculates how many opponents are nearby based on a circle collider.
public class OpponentsNearby : MonoBehaviour
{

    CircleCollider2D PlayerRange;
    public List<Collider2D> collidersList = new List<Collider2D>();

    int opponentsNearby;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRange = GetComponent<CircleCollider2D>();

        opponentsNearby = 0;
    }

    public void Update()
    {
        opponentsNearby = collidersList.Count;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 11 && collision.gameObject.transform.parent != this.transform.parent.parent)
        {
            if(!collidersList.Contains(collision))
            {
                collidersList.Add(collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11 && collision.gameObject.transform.parent != this.transform.parent)
        {
            if (collidersList.Contains(collision))
            {
                collidersList.Remove(collision);
            }
        }
    }

    public int GetOpponnentsNearby()
    {
        return opponentsNearby;
    }
}
