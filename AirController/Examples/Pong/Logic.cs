using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SwordGC.AirController.Examples.Pong
{
    public class Logic : MonoBehaviour
    {

        public Racket leftRacket;
        public Racket rightRacket;

        public Text ui;

        public Rigidbody2D ball;
        public float ballSpeed = 10f;

        private void Start()
        {
            leftRacket.player = AirController.Instance.GetPlayer(0);
            rightRacket.player = AirController.Instance.GetPlayer(1);
        }

        private void OnEnable()
        {
            AirController.Instance.onPlayerClaimed += OnPlayerClaimed;
            AirController.Instance.onPlayerUnclaimed += OnPlayerUnclaimed;
        }

        private void OnDisable()
        {
            AirController.Instance.onPlayerClaimed -= OnPlayerClaimed;
            AirController.Instance.onPlayerUnclaimed -= OnPlayerUnclaimed;
        }

        private void OnPlayerClaimed (Player player)
        {
            if (AirController.Instance.PlayersAvailable == 0)
            {
                StartGame();
            }
            AirController.Instance.UpdateDeviceStates();
        }

        private void OnPlayerUnclaimed (Player player)
        {
            if (AirController.Instance.PlayersAvailable != 0)
            {
                StopGame();
            }
            AirController.Instance.UpdateDeviceStates();
        }

        private void StartGame ()
        {
            ui.text = "GO!";
            Debug.Log("Start game");
            ResetBall(true);
        }

        private void StopGame()
        {
            ui.text = string.Format("NEED {0} MORE PLAYERS", AirController.Instance.PlayersAvailable);
            ResetBall(false);
        }

        void ResetBall(bool move)
        {

            // place ball at center
            ball.position = Vector3.zero;

            // push the ball in a random direction
            if (move)
            {
                Vector3 startDir = new Vector3(Random.Range(-1, 1f), Random.Range(-0.1f, 0.1f), 0);
                ball.velocity = startDir.normalized * ballSpeed;
            }
            else
            {
                ball.velocity = Vector3.zero;
            }
        }

        void UpdateScoreUI()
        {
            // update text canvas
            ui.text = leftRacket.score + ":" + rightRacket.score;
        }

        void FixedUpdate()
        {

            // check if ball reached one of the ends
            if (ball.position.x < -9f)
            {
                rightRacket.score++;
                UpdateScoreUI();
                ResetBall(true);
            }

            if (ball.position.x > 9f)
            {
                leftRacket.score++;
                UpdateScoreUI();
                ResetBall(true);
            }
        }

    }
}