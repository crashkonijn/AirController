using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.InputTypes
{
    public class DeviceOrientation : IInput
    {
        public enum STATE { UNKNOWN, FLAT, PORTRAIT, LEFT, RIGHT }
        public STATE state;

        public float alpha;
        public float beta;
        public float gamma;

        public float X { get { return beta; } }
        public float Y { get { return alpha; } }
        public float Z { get { return gamma; } }

        public Vector3 EulerAngles
        {
            get
            {
                return new Vector3(beta, alpha, gamma);
            }
        }

        public void HandleData(JSONObject data)
        {
            if (data.keys.Contains("alpha"))
                alpha = LimitRotation(data["alpha"].f);

            if (data.keys.Contains("beta"))
                beta = LimitRotation(data["beta"].f);

            if (data.keys.Contains("gamma"))
                gamma = LimitRotation(data["gamma"].f);

            DetectState();
        }

        public void Update()
        {
            
        }

        private void DetectState()
        {
            int x = Mathf.RoundToInt(X / 90f);
            int z = Mathf.RoundToInt(Z / 90f);

            if (x == 0 && z == 0)
            {
                state = STATE.FLAT;
            }
            else if ((x == 0 && z == -1) || (x == 2 && z == 1))
            {
                state = STATE.LEFT;
            }
            else if ((x == 0 && z == 1) || (x == -2 && z == -1))
            {
                state = STATE.RIGHT;
            }
            else if (x == 1 && (z == 1 || z == 0 || z == -1))
            {
                state = STATE.PORTRAIT;
            }
        }

        private float LimitRotation(float f)
        {
            if (f > 180f)
            {
                return f - 180;
            }
            else if (f < -180f)
            {
                return f + 180;
            }

            return f;
        }
    }
}