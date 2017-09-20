using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.InputTypes
{
    public class DeviceMotion : IInput
    {
        /// <summary>
        /// Holds the acceleration of this device, including gravity
        /// </summary>
        public Vector3 GravityAcceleration { get; private set; }

        public void HandleData(JSONObject data)
        {
            if (data.keys.Contains("x") && data.keys.Contains("y") && data.keys.Contains("z"))
                GravityAcceleration = new Vector3(data["x"].f, data["y"].f, data["z"].f);
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
                case DeviceOrientation.STATE.PORTRAIT: return -GravityAcceleration.x;
                case DeviceOrientation.STATE.LEFT: return GravityAcceleration.y;
                case DeviceOrientation.STATE.RIGHT: return -GravityAcceleration.y;
                case DeviceOrientation.STATE.FLAT: return -GravityAcceleration.x;
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
                case DeviceOrientation.STATE.PORTRAIT: return GravityAcceleration.z;
                case DeviceOrientation.STATE.LEFT: return GravityAcceleration.z;
                case DeviceOrientation.STATE.RIGHT: return GravityAcceleration.z;
                case DeviceOrientation.STATE.FLAT: return -GravityAcceleration.y;
                default: return 0f;
            }
        }
    }
}