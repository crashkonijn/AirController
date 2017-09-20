using NDream.AirConsole;
using SwordGC.AirController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.Examples.Controller
{
    public class ExampleAirController : AirController
    {

        public override Player GetNewPlayer(int playerId)
        {
            return new ExamplePlayer(playerId);
        }

        protected override Device GetNewDevice(int deviceId)
        {
            return new ExampleDevice(deviceId);
        }
    }
}