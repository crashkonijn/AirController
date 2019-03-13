using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.Examples.Pong
{
    public class Racket : MonoBehaviour
    {
        public Logic logic;
        public Player player;
        private Rigidbody2D rb2d;
        public int score;
        public float moveSpeed = 5f;

        private void Awake()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            rb2d.velocity = player.Input.GetVector("move") * moveSpeed;
        }

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
                col.gameObject.GetComponent<Rigidbody2D>().velocity = dir * logic.ballSpeed;

            }
        }
    }
}
