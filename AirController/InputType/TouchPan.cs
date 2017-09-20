using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.InputTypes
{
    public class TouchPan : IInput
    {
        private bool active;
        private bool prevActive;

        public Vector2 touchStart;
        public Vector2 touchCurrent;

        /// <summary>
        /// Returns true on the first frame of this event
        /// </summary>
        public bool IsTouchStart
        {
            get
            {
                return active && !prevActive;
            }
        }

        /// <summary>
        /// Returns true during all frames of this event
        /// </summary>
        public bool IsTouching
        {
            get
            {
                return active;
            }
        }


        /// <summary>
        /// Returns true on the last frame of this event
        /// </summary>
        public bool IsTouchEnd
        {
            get
            {
                return !active && prevActive;
            }
        }

        public void HandleData (JSONObject data)
        {
            if (data["value"].HasField("vector"))
            {
                Vector2 v = Input.VectorFromJSON(data["value"]["vector"]);

                if (!IsTouching)
                {
                    touchStart = v;
                    active = true;
                }

                touchCurrent = v;

                if (data["value"]["end"].b)
                {
                    active = false;
                }
            }
        }

        public void Update ()
        {
            prevActive = active;
        }

        /// <summary>
        /// Calls the input action when IsTouchStart
        /// </summary>
        public void TouchStart (Action<Vector2> callback)
        {
            if (IsTouchStart)
            {
                callback(touchStart);
            }
        }
        
        /// <summary>
        /// Calls the input action when IsTouching
        /// </summary>
        public void Touching (Action<Vector2, Vector2> callback)
        {
            if (IsTouching)
            {
                callback(touchStart, touchCurrent);
            }
        }
        
        /// <summary>
        /// Calls the input action when IsTouchEnd
        /// </summary>
        public void TouchEnd (Action<Vector2, Vector2> callback)
        {
            if (IsTouchEnd)
            {
                callback(touchStart, touchCurrent);
            }
        }
    }
}