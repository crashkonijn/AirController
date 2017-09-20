using SwordGC.AirController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.Examples.Controller
{
    public class ExampleDevice : Device
    {
        MenuController menu;

        public ExampleDevice(int deviceId) : base(deviceId)
        {
            menu = GameObject.FindObjectOfType<MenuController>();
        }

        public override string View
        {
            get
            {
                if (airController.PlayersAvailable == 1)
                {
                    return "Claim";
                }
                else if(PlayerId != 0)
                {
                    return "Full";
                }
                else
                {
                    switch (menu.state)
                    {
                        case MenuController.STATE.MENU: return "Menu";
                        case MenuController.STATE.BUTTONS: return "Buttons";
                        case MenuController.STATE.SWIPE: return "Swipe";
                        case MenuController.STATE.PAN: return "Pan";
                        case MenuController.STATE.MOTION: return "Motion";
                        case MenuController.STATE.JOYSTICK: return "Joystick";
                        case MenuController.STATE.PROFILE: return "Profile";
                        default: return "Loading";
                    }
                }
            }
        }
    }
}