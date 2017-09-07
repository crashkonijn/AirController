using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController
{
    public class Device
    {
        /// <summary>
        /// Cached reference to the controller
        /// </summary>
        private AirController airController;

        /// <summary>
        /// Reference to the input of this device
        /// </summary>
        public Input Input { get; protected set; }

        /// <summary>
        /// Set to true when this specific device is here
        /// </summary>
        private bool isHero;

        /// <summary>
        /// Returns true when this device is hero or heromode === TOGETHER and there's at least one hero in the party
        /// </summary>
        public bool IsHero
        {
            get
            {
                return airController.heroMode == AirController.HEROMODE.TOGETHER ? airController.HasHero : isHero;
            }
            set
            {
                isHero = value;
            }
        }

        /// <summary>
        /// The id of this device
        /// </summary>
        public int DeviceId { get; private set; }

        /// <summary>
        /// Returns the playerId of the claimed player.
        /// When no player is claimed it will return -1
        /// </summary>
        public int PlayerId
        {
            get
            {
                foreach (Player p in airController.Players.Values)
                {
                    if (p.DeviceId == DeviceId) return p.PlayerId;
                }
                return -1;
            }
        }

        /// <summary>
        /// Returns the connected player
        /// </summary>
        public Player Player
        {
            get
            {
                return AirController.Instance.GetPlayer(PlayerId);
            }
        }

        /// <summary>
        /// Returns the Nickname of this device
        /// </summary>
        public string NickName
        {
            get
            {
                if (AirController.Instance.IsReady)
                    return AirConsole.instance.GetNickname(DeviceId);
                else
                    return "Guest";
            }
        }

        /// <summary>
        /// Should return the current view of the controller
        /// </summary>
        public virtual string View
        {
            get
            {
                return "Menu";
            }
        }

        /// <summary>
        /// Should return the classes that should be inserted on the controller
        /// </summary>
        public virtual string Classes
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Holds the profile picture of the device in 512x512 px
        /// </summary>
        public Texture2D ProfilePicture { get; private set; }

        public Device(int deviceId)
        {
            airController = AirController.Instance;
            DeviceId = deviceId;
            Input = new Input();

            airController.StartCoroutine(LoadProfilePicture());
        }

        /// <summary>
        /// Returns the JSON data for this object
        /// </summary>
        public JSONObject GetJson()
        {
            JSONObject j = new JSONObject(JSONObject.Type.OBJECT);

            j.AddField("playerId", PlayerId);
            j.AddField("view", View);
            j.AddField("class", Classes);
            j.AddField("enablehero", IsHero);

            return j;
        }

        /// <summary>
        /// Loads the profile picture of this device
        /// </summary>
        private IEnumerator LoadProfilePicture()
        {
            string url = AirConsole.instance.GetProfilePicture(DeviceId, 512);
            ProfilePicture = new Texture2D(4, 4, TextureFormat.DXT1, false);
            WWW www = new WWW(url);
            yield return www;
            www.LoadImageIntoTexture(ProfilePicture);
        }
    }
}