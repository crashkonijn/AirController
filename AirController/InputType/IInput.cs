using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AirController.InputTypes
{
    interface IInput
    {
        void HandleData(JSONObject data);
        void Update();
    }
}
