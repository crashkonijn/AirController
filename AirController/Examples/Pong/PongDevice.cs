using SwordGC.AirController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.Examples.Pong
{
    public class PongDevice : Device
    {

        public PongDevice(int deviceId) : base(deviceId)
        {
           
        }

        public override string Classes
        {
            get
            {
                if (HasPlayer)
                {
                    return "player" + PlayerId;
                }
                else
                {
                    return "";
                }
            }
        }

        public override string View
        {
            get
            {
                if (HasPlayer)
                {
                    if (airController.PlayersAvailable == 0)
                    {
                        return "Gameplay";
                    }
                    else
                    {
                        return "Waiting";
                    }
                }
                else
                {
                    return "Full";
                }
            }
        }
    }
}