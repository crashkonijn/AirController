using NDream.AirConsole;
using SwordGC.AirController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.Examples.Pong
{
    public class PongAirController : AirController
    {

        protected override Device GetNewDevice(int deviceId)
        {
            return new PongDevice(deviceId);
        }
    }
}