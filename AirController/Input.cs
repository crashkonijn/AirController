using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController
{
    public class Input
    {
        public static float BUTTON_COOLDOWN = 0.05f;

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
        /// Retuyrns true if the button was pressed down this frame
        /// </summary>
        public bool GetKeyDown (string key)
        {
            return GetKeyObject(key).active && !GetKeyObject(key).prevActive;
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
    }
}