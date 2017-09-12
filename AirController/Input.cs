using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController
{
    public class Input
    {
        public static float BUTTON_COOLDOWN = 0.05f;

        public DeviceMotion motion;
        public DeviceOrientation orientation;

        /// <summary>
        /// Holds all vector inputs
        /// </summary>
        public Dictionary<string, Vector2> Axis { get; private set; }

        /// <summary>
        /// Holds all key (button) inputs
        /// </summary>
        public Dictionary<string, Key> Keys { get; private set; }

        public Input ()
        {
            Axis = new Dictionary<string, Vector2>();
            Keys = new Dictionary<string, Key>();
        }
        
        /// <summary>
        /// Processes the data
        /// </summary>
        public void Process(JToken data)
        {
            JSONObject json = new JSONObject(data.ToString());

            foreach (string key in json.keys)
            {
                JSONObject j = json[key];

                string type = j["type"].str;

                if (type == "tap-button")
                {
                    GetKeyObject(key).OnDown(int.Parse(j["value"].str != null ? j["value"].str : "0"), Key.TYPE.TAP);
                }
                else if (type == "hold-button")
                {
                    if (j["event"].str == "down")
                    {
                        GetKeyObject(key).OnDown(int.Parse(j["value"].str != null ? j["value"].str : "0"), Key.TYPE.HOLD);
                    }
                    else
                    {
                        GetKeyObject(key).OnUp();
                    }
                }
                else if(type == "vector")
                {
                    string lKey = key.ToLower();
                    if (!Axis.ContainsKey(lKey))
                    {
                        Axis.Add(lKey, VectorFromJSON(j["value"]));
                    }
                    else
                    {
                        Axis[lKey] = VectorFromJSON(j["value"]);
                    }
                }
                else if (type == "gyro")
                {
                    Debug.Log(j["value"].ToString());

                    orientation.FromJSON(j["value"]);
                    motion.FromJSON(j["value"]);

                    Debug.Log(orientation);
                }
            }
        }

        /// <summary>
        /// Returns a key object for a given key
        /// </summary>
        private Key GetKeyObject(string key)
        {
            key = key.ToLower();

            if (!Keys.ContainsKey(key))
            {
                Keys.Add(key, new Key());
            }

            return Keys[key];
        }

        /// <summary>
        /// Returns true if the button was pressed this frame
        /// </summary>
        public bool GetKey (string key)
        {
            return GetKeyObject(key).active;
        }

        /// <summary>
        /// Returns true if the button was pressed down this frame
        /// </summary>
        public bool GetKeyDown (string key)
        {
            return GetKeyObject(key).active && !GetKeyObject(key).prevActive;
        }

        /// <summary>
        /// True if the button was released this frame
        /// </summary>
        public bool GetKeyUp (string key)
        {
            return GetKeyObject(key).type == Key.TYPE.HOLD ? !GetKeyObject(key).active && GetKeyObject(key).prevActive : false;
        }

        /// <summary>
        /// Returns the axis of an object
        /// 
        /// Example: The controller holds a joystick called Movement
        /// GetAxis("MovementHorizontal") will return the x value of movement
        /// </summary>
        public float GetAxis (string key)
        {
            key = key.ToLower();

            if (key.Contains("horizontal"))
            {
                key = key.Replace("horizontal", "");
                if (Axis.ContainsKey(key))
                {
                    return Axis[key].x;
                }
            }
            else if (key.Contains("vertical"))
            {
                key = key.Replace("vertical", "");
                if (Axis.ContainsKey(key))
                {
                    return Axis[key].x;
                }
            }
            return 0f;
        }

        /// <summary>
        /// Returns the vector of a given key
        /// </summary>
        public Vector2 GetVector (string key)
        {
            key = key.ToLower();

            if (Axis.ContainsKey(key))
            {
                return Axis[key];
            }

            return Vector2.zero;
        }

        /// <summary>
        /// Resets the unput, is called on LateUpdate
        /// </summary>
        public void Reset()
        {
            foreach (Key k in Keys.Values)
            {
                k.prevActive = k.active;

                if (k.type == Key.TYPE.TAP)
                {
                    k.cooldown -= Time.deltaTime;
                    k.active = false;
                }
            }
        }

        /// <summary>
        /// Returns a Vector2 from JSON input
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private Vector2 VectorFromJSON (JSONObject json)
        {
            if (json.keys.Contains("x") && json.keys.Contains("y"))
            {
                Vector2 v = new Vector2();

                v.x = json["x"].f / 50f;
                v.y = json["y"].f / 50f * -1f;

                return v;
            }
            else
            {
                return Vector2.zero;
            }
        }

        public class Key
        {
            public enum TYPE { TAP, HOLD }
            public TYPE type;

            public bool active = false;
            public bool prevActive = false;
            public int value;
            public float cooldown;

            public void OnDown (int value, TYPE type)
            {
                if (cooldown > 0) return;

                active = true;
                this.value = value;
                this.type = type;

                if (type == TYPE.TAP)
                {
                    cooldown = BUTTON_COOLDOWN;
                }
            }

            public void OnUp ()
            {
                active = false;
            }

        }

        public struct DeviceOrientation
        {
            public enum STATE { UNKNOWN, FLAT, PORTRAIT, LEFT, RIGHT}
            public STATE state;

            public float alpha;
            public float beta;
            public float gamma;

            public float rawAlpha;
            public float rawBeta;
            public float rawGamma;

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

            public void FromJSON (JSONObject data)
            {
                if(data.keys.Contains("alpha"))
                    alpha = LimitRotation(data["alpha"].f);

                if (data.keys.Contains("beta"))
                    beta = LimitRotation(data["beta"].f);

                if (data.keys.Contains("gamma"))
                    gamma = LimitRotation(data["gamma"].f);

                DetectState();
            }

            private void DetectState ()
            {
                int x = Mathf.RoundToInt(X / 90f);
                //int y = Mathf.RoundToInt(Y / 90f);
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

            private float LimitRotation (float f)
            {
                if (f > 180f)
                {
                    return f - 180;
                }
                else if (f < - 180f)
                {
                    return f + 180;
                }

                return f;
            }
        }

        public struct DeviceMotion
        {
            public Vector3 acceleration;
            public Vector3 gravityAcceleration;

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

            public void FromJSON(JSONObject data)
            {
                if (data.keys.Contains("x") && data.keys.Contains("y") && data.keys.Contains("z"))
                    acceleration = new Vector3(data["x"].f, data["y"].f - 9.81f, data["z"].f);

                if (data.keys.Contains("x") && data.keys.Contains("y") && data.keys.Contains("z"))
                    gravityAcceleration = new Vector3(data["x"].f, data["y"].f, data["z"].f);
            }
        }
    }
}