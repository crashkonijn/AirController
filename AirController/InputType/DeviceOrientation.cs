using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.InputTypes
{
    public class DeviceOrientation : IInput
    {
        public enum STATE { UNKNOWN, FLAT, PORTRAIT, LEFT, RIGHT }
        public STATE State { get; private set; }

        public float Alpha { get; private set; }
        public float Beta { get; private set; }
        public float Gamma { get; private set; }

        public float X { get { return Beta; } }
        public float Y { get { return Alpha; } }
        public float Z { get { return Gamma; } }

        public Vector3 EulerAngles
        {
            get
            {
                return new Vector3(Beta, Alpha, Gamma);
            }
        }

        public void HandleData(JSONObject data)
        {
            if (data.keys.Contains("alpha"))
                Alpha = data["alpha"].f;

            if (data.keys.Contains("beta"))
                Beta = data["beta"].f;

            if (data.keys.Contains("gamma"))
                Gamma = data["gamma"].f;

            DetectState();
        }

        public void Update()
        {
            
        }

        private void DetectState()
        {
            int x = Mathf.RoundToInt(LimitRotation(X) / 90f);
            int z = Mathf.RoundToInt(LimitRotation(Z) / 90f);

            if (x == 0 && z == 0)
            {
                State = STATE.FLAT;
            }
            else if ((x == 0 && z == -1) || (x == 2 && z == 1))
            {
                State = STATE.LEFT;
            }
            else if ((x == 0 && z == 1) || (x == -2 && z == -1))
            {
                State = STATE.RIGHT;
            }
            else if (x == 1 && (z == 1 || z == 0 || z == -1))
            {
                State = STATE.PORTRAIT;
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