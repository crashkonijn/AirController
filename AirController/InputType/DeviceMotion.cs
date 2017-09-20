using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.InputTypes
{
    public class DeviceMotion : IInput
    {
        public Vector3 acceleration;
        public Vector3 gravityAcceleration;

        public void HandleData(JSONObject data)
        {
            if (data.keys.Contains("x") && data.keys.Contains("y") && data.keys.Contains("z"))
                acceleration = new Vector3(data["x"].f, data["y"].f - 9.81f, data["z"].f);

            if (data.keys.Contains("x") && data.keys.Contains("y") && data.keys.Contains("z"))
                gravityAcceleration = new Vector3(data["x"].f, data["y"].f, data["z"].f);
        }

        public void Update()
        {
            
        }

        /// <summary>
        /// Returns the roll compared to the desired state
        /// </summary>
        public float GetRoll(DeviceOrientation.STATE state)
        {
            switch (state)
            {
                case DeviceOrientation.STATE.PORTRAIT: return -gravityAcceleration.x;
                case DeviceOrientation.STATE.LEFT: return gravityAcceleration.y;
                case DeviceOrientation.STATE.RIGHT: return -gravityAcceleration.y;
                case DeviceOrientation.STATE.FLAT: return -gravityAcceleration.x;
                default: return 0f;
            }
        }

        /// <summary>
        /// Returns the tilt compared to the desired state
        /// </summary>
        public float GetTilt(DeviceOrientation.STATE state)
        {
            switch (state)
            {
                case DeviceOrientation.STATE.PORTRAIT: return gravityAcceleration.z;
                case DeviceOrientation.STATE.LEFT: return gravityAcceleration.z;
                case DeviceOrientation.STATE.RIGHT: return gravityAcceleration.z;
                case DeviceOrientation.STATE.FLAT: return -gravityAcceleration.y;
                default: return 0f;
            }
        }
    }
}