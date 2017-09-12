using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SwordGC.AirController
{
    public class SaveData
    {
        /// <summary>
        /// Holds all int values, should not be accessed directly
        /// </summary>
        public Dictionary<string, int> IntStore { get; private set; }

        /// <summary>
        /// Holds all string values, should not be accessed directly
        /// </summary>
        public Dictionary<string, string> StringStore { get; private set; }

        public SaveData ()
        {
            IntStore = new Dictionary<string, int>();
            StringStore = new Dictionary<string, string>();
        }

        /// <summary>
        /// Sets an int to the savedata
        /// </summary>
        public void SetInt (string key, int i)
        {
            if (!IntStore.ContainsKey(key)) IntStore.Add(key, i);
            else IntStore[key] = i;
        }

        /// <summary>
        /// Returns an int from the savedata, returns the defaultValue when there's none
        /// </summary>
        public int GetInt (string key, int defaultValue = 0)
        {
            if (!IntStore.ContainsKey(key)) IntStore.Add(key, defaultValue);

            return IntStore[key];
        }

        /// <summary>
        /// Sets a string to the savedata
        /// </summary>
        public void SetString(string key, string s)
        {
            if (!StringStore.ContainsKey(key)) StringStore.Add(key, s);
            else StringStore[key] = s;
        }

        /// <summary>
        /// Returns a string from the savedata, returns the defaultValue when there's none
        /// </summary>
        public string GetString (string key, string defaultValue = "")
        {
            if (!StringStore.ContainsKey(key)) StringStore.Add(key, defaultValue);

            return StringStore[key];
        }

        /// <summary>
        /// Internal function to load data that's loaded by AirConsole
        /// </summary>
        public void FromJSON (JSONObject data)
        {
            Debug.Log("Parsing savedata " + data.ToString());

            if (data.HasField("AirControllerData"))
            {
                JSONObject json = new JSONObject(Decode(data["AirControllerData"].str));

                if (json.HasField("intStore"))
                {
                    foreach (string key in json["intStore"].keys)
                    {
                        IntStore[key] = (int)json["intStore"][key].i;
                    }
                }

                if (json.HasField("stringStore"))
                {
                    foreach (string key in json["stringStore"].keys)
                    {
                        IntStore[key] = (int)json["stringStore"][key].i;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all data and converts it to a string
        /// </summary>
        /// <returns>The base64 encoded string of the savedata</returns>
        public string ToJSON ()
        {
            JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

            JSONObject iStore = new JSONObject(JSONObject.Type.OBJECT);
            foreach (KeyValuePair<string, int> data in IntStore)
            {
                iStore.AddField(data.Key, data.Value);
            }

            JSONObject sStore = new JSONObject(JSONObject.Type.OBJECT);
            foreach (KeyValuePair<string, string> data in StringStore)
            {
                sStore.AddField(data.Key, data.Value);
            }

            json.AddField("intStore", iStore);
            json.AddField("stringStore", sStore);

            return Encode(json.ToString());
        }

        /// <summary>
        /// Encodes a string to Base64
        /// </summary>
        private string Encode (string encode)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(encode);
            return Convert.ToBase64String(bytesToEncode);
        }

        /// <summary>
        /// Decodes a string from Base64
        /// </summary>
        private string Decode (string decode)
        {
            byte[] decodedBytes = Convert.FromBase64String(decode);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}