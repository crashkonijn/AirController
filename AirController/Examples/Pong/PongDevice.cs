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

        public override string View
        {
            get
            {
                return "Loading";
            }
        }
    }
}