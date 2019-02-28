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
                    return "claim";
                }

                if (PlayerId != 0)
                {
                    return "full";
                }

                switch (menu.state)
                {
                    case MenuController.STATE.MENU: return "menu";
                    case MenuController.STATE.BUTTONS: return "buttons";
                    case MenuController.STATE.SWIPE: return "swipe";
                    case MenuController.STATE.PAN: return "pan";
                    case MenuController.STATE.MOTION: return "motion";
                    case MenuController.STATE.CUSTOM_DATA: return "custom-data";
                    case MenuController.STATE.PROFILE: return "profile";
                    case MenuController.STATE.AXIS: return "axis";
                    case MenuController.STATE.JOYSTICK: return "joystick";
                    default: return "loading";
                }
            }
        }
    }
}