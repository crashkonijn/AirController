using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.Examples.Pong
{
    public class Racket : MonoBehaviour
    {

        PongPlayer pongPlayer;

        void OnCollisionEnter2D(Collision2D col)
        {

            if (col.gameObject.GetComponent<Rigidbody2D>() != null)
            {

                float hitPos = (col.transform.position.y - transform.position.y) / (GetComponent<Collider2D>().bounds.size.y / 2);
                float hitDir = 1f;

                if (col.relativeVelocity.x > 0)
                {
                    hitDir = -1f;
                }

                Vector2 dir = new Vector2(hitDir, hitPos).normalized;
                col.gameObject.GetComponent<Rigidbody2D>().velocity = dir * 5f;

            }
        }
    }
}
