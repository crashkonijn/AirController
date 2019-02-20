using NDream.AirConsole;
using SwordGC.AirController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SwordGC.AirController.Examples.Controller
{
    public class ExampleAirController : AirController
    {
        protected override void OnReady(string code)
        {
            base.OnReady(code);

            //SceneManager.LoadScene("Controller", LoadSceneMode.Additive);
        }

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