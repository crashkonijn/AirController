using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController
{
    public class Player
    {
        /// <summary>
        /// Empty input object that gets returned when there's no device connected to this player to prevent errors
        /// </summary>
        private static Input emptyInput = new Input();

        /// <summary>
        /// Cached reference to the controller
        /// </summary>
        AirController airController;

        public enum STATE { UNCLAIMED, CLAIMED, DISCONNECTED }
        /// <summary>
        /// The current state of this player
        /// </summary>
        public STATE state = STATE.UNCLAIMED;

        /// <summary>
        /// Wrapper for the device nickname
        /// </summary>
        public string Nickname
        {
            get { return Device.Nickname; }
        }

        /// <summary>
        /// Wrapper for the device profile picture
        /// </summary>
        public Texture2D ProfilePicture
        {
            get { return Device.ProfilePicture; }
        }

        /// <summary>
        /// The id of this player
        /// </summary>
        public int PlayerId { get; private set; }
        
        /// <summary>
        /// The id of the connected device
        /// </summary>
        public int DeviceId { get; private set; }

        /// <summary>
        /// Returns the device of this player
        /// </summary>
        public Device Device
        {
            get { return airController.GetDevice(DeviceId); }
        }

        /// <summary>
        /// Return true if this player has a device
        /// </summary>
        public bool HasDevice
        {
            get { return Device != null; }
        }

        /// <summary>
        /// Returns the input of this player
        /// </summary>
        public Input Input
        {
            get
            {
                return Device != null ? Device.Input : emptyInput;
            }
        }

        public Player(int playerId)
        {
            DeviceId = -1;
            PlayerId = playerId;

            airController = AirController.Instance;
        }

        #region PLAYER_FUNCTIONS
        /// <summary>
        /// Claims this player for a device
        /// </summary>
        public Player Claim(int deviceId)
        {
            DeviceId = deviceId;
            state = STATE.CLAIMED;

            airController.OnPlayerClaimed(this);
            return this;
        }

        /// <summary>
        /// Unclaims this player from it's device
        /// </summary>
        public void UnClaim()
        {
            DeviceId = -1;
            state = STATE.UNCLAIMED;

            airController.OnPlayerUnclaimed(this);
        }

        /// <summary>
        /// Sets it's state to disconnected
        /// </summary>
        public void Disconnect()
        {
            state = STATE.DISCONNECTED;
        }
        #endregion
    }
}
