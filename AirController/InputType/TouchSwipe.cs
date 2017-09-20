using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.InputTypes
{
    public class TouchSwipe : IInput
    {
        public enum DIRECTION { UP, DOWN, LEFT, RIGHT }
        public DIRECTION Direction { get; private set; }

        private bool active;

        public bool IsSwiped
        {
            get
            {
                return active;
            }
        }

        public void HandleData(JSONObject data)
        {
            if (data["value"])
            {
                switch (data["value"].str)
                {
                    case "swipeleft":
                        Direction = DIRECTION.LEFT;
                        break;
                    case "swiperight":
                        Direction = DIRECTION.RIGHT;
                        break;
                    case "swipeup":
                        Direction = DIRECTION.UP;
                        break;
                    case "swipedown":
                        Direction = DIRECTION.DOWN;
                        break;
                }
            }
            active = true;

            Debug.Log("SWIPE: " + Direction);
        }

        public void Update()
        {
            active = false;
        }

        public void Swiped (Action<DIRECTION> callback)
        {
            if (IsSwiped)
            {
                callback(Direction);
            }
        }
        
        public void SwipedUp (Action callback)
        {
            if (IsSwiped && Direction == DIRECTION.UP)
            {
                callback();
            }
        }

        public void SwipedDown(Action callback)
        {
            if (IsSwiped && Direction == DIRECTION.DOWN)
            {
                callback();
            }
        }

        public void SwipedLeft(Action callback)
        {
            if (IsSwiped && Direction == DIRECTION.LEFT)
            {
                callback();
            }
        }

        public void SwipedRight(Action callback)
        {
            if (IsSwiped && Direction == DIRECTION.RIGHT)
            {
                callback();
            }
        }
    }
}