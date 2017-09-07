using SwordGC.AirController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleDevice : Device {

	public ExampleDevice (int deviceId) : base(deviceId)
    {

    }

    public override string View
    {
        get
        {
            if (Player == null) return "NotJoined";
            else return "Joined";
        }
    }
}
