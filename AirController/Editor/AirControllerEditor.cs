using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace SwordGC.AirController
{
    [CustomEditor(typeof(AirController), true)]
    public class AirControllerEditor : Editor
    {
        AirController controller;

        private bool showUnclaimedPlayers = true;
        private bool showDevices = true;
        private Dictionary<string, Dictionary<Device, bool>> showData = new Dictionary<string, Dictionary<Device, bool>>();

        public override void OnInspectorGUI()
        {
            controller = (AirController)target;

            DrawSettings();

            if (!EditorApplication.isPlaying) return;

            DrawStats();

            if (controller.Players.Count > 0)
            {
                showUnclaimedPlayers = EditorGUILayout.Foldout(showUnclaimedPlayers, "Unclaimed Players");

                if (showUnclaimedPlayers)
                {
                    foreach (Player p in controller.Players.Values)
                    {
                        if (p.state != Player.STATE.CLAIMED) DrawPlayer(p);
                    }
                }
            }

            showDevices = EditorGUILayout.Foldout(showDevices, "Devices");
            if (showDevices)
            {
                foreach (Device d in controller.Devices.Values)
                {
                    DrawDevice(d);
                }
            }

            EditorUtility.SetDirty(controller);
        }

        private void DrawSettings()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("Box");

            controller.heroMode = (AirController.HEROMODE)EditorGUILayout.EnumPopup("Hero Mode", controller.heroMode);

            controller.joinMode = (AirController.JOINMODE)EditorGUILayout.EnumPopup("Join Mode", controller.joinMode);
            controller.maxPlayersMode = (AirController.MAXPLAYERSMODE)EditorGUILayout.EnumPopup("Max players mode", controller.maxPlayersMode);

            if(controller.maxPlayersMode == AirController.MAXPLAYERSMODE.LIMITED)
                controller.maxPlayers = EditorGUILayout.IntField("Max players", controller.maxPlayers);

            controller.debug = EditorGUILayout.Toggle("Debug", controller.debug);
            controller.autoLoadSavedata = EditorGUILayout.Toggle("Auto load savedata", controller.autoLoadSavedata);

            EditorGUILayout.EndVertical();
        }

        private void DrawStats ()
        {
            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("Box");

            if (controller.heroMode == AirController.HEROMODE.TOGETHER)
                EditorGUILayout.LabelField("Hero: ", "" + controller.HasHero, EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Player objects: ", "" + controller.Players.Count, EditorStyles.boldLabel);

            if (controller.maxPlayersMode == AirController.MAXPLAYERSMODE.LIMITED)
            {
                EditorGUILayout.LabelField("Max Players: ", controller.maxPlayers.ToString(), EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Players available: ", "" + controller.PlayersAvailable, EditorStyles.boldLabel);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawDevice(Device device)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("" + device.Nickname, EditorStyles.boldLabel);

            EditorGUILayout.LabelField("deviceId: ", "" + device.DeviceId);
            EditorGUILayout.LabelField("view: ", "" + device.View);
            EditorGUILayout.LabelField("classes: ", "" + device.Classes);

            if (controller.heroMode == AirController.HEROMODE.SEPERATE) EditorGUILayout.LabelField("hero: ", "" + device.IsHero);

            DrawInput(device);

            if (device.PlayerId != -1) DrawPlayer(controller.GetPlayer(device.PlayerId));

            EditorGUILayout.EndVertical();
        }

        private void DrawInput (Device device)
        {
            EditorGUILayout.BeginVertical("Box");
            SetShowInput("input", device, EditorGUILayout.Foldout(ShouldShowInput("input", device), "Input"));

            if (ShouldShowInput("input", device))
            {

                foreach (string key in device.Input.Axis.Keys)
                {
                    EditorGUILayout.LabelField(key + ":", "(" + device.Input.Axis[key].x + ", " + device.Input.Axis[key].y + ")");
                }

                foreach (string key in device.Input.Keys.Keys)
                {
                    if (device.Input.Keys[key].type == InputTypes.Key.TYPE.HOLD)
                    {
                        EditorGUILayout.LabelField(key + ":", "Hold (" + device.Input.Keys[key].active + ", " + device.Input.Keys[key].value.ToString() + ")");
                    }
                    else
                    {
                        EditorGUILayout.LabelField(key + ":", "Tap (" + device.Input.Keys[key].value.ToString() + ")");
                    }
                }
            }

            SetShowInput("motion", device, EditorGUILayout.Foldout(ShouldShowInput("motion", device), "Motion"));

            if (ShouldShowInput("motion", device))
            {
                EditorGUILayout.LabelField("State: ", device.Input.Orientation.State.ToString());
                EditorGUILayout.LabelField("Orientation: ", device.Input.Orientation.EulerAngles.ToString());
                //EditorGUILayout.LabelField("Orientation: ", "(" + Mathf.RoundToInt(device.Input.orientation.EulerAngles.x / 90) + ", " + Mathf.RoundToInt(device.Input.orientation.EulerAngles.y / 90) + ", " + Mathf.RoundToInt(device.Input.orientation.EulerAngles.z / 90) + ")");
                EditorGUILayout.LabelField("Motion: ", device.Input.Motion.GravityAcceleration.ToString());
                EditorGUILayout.LabelField("Roll: ", device.Input.Motion.GetRoll(device.Input.Orientation.State).ToString());
                EditorGUILayout.LabelField("Tilt: ", device.Input.Motion.GetTilt(device.Input.Orientation.State).ToString());
            }

            SetShowInput("savedata", device, EditorGUILayout.Foldout(ShouldShowInput("savedata", device), "Save Data"));
            if (ShouldShowInput("savedata", device))
            {
                EditorGUILayout.LabelField("Int Store", EditorStyles.boldLabel);
                foreach (KeyValuePair<string, int> data in device.SaveData.IntStore)
                {
                    EditorGUILayout.LabelField(data.Key, data.Value.ToString());
                }

                EditorGUILayout.LabelField("String Store", EditorStyles.boldLabel);
                foreach (KeyValuePair<string, string> data in device.SaveData.StringStore)
                {
                    EditorGUILayout.LabelField(data.Key, data.Value);
                }

                if (GUILayout.Button("Save"))
                {
                    controller.SaveData(device);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawPlayer(Player player)
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("Player " + player.PlayerId, EditorStyles.boldLabel);

            if (player.state != Player.STATE.CLAIMED)
            EditorGUILayout.LabelField("state: ", "" + player.state);

            EditorGUILayout.EndVertical();
        }

        private bool ShouldShowInput (string key, Device device)
        {
            if (!GetSettings(key).ContainsKey(device)) showData[key].Add(device, false);

            return showData[key][device];
        }

        private void SetShowInput (string key, Device device, bool value)
        {
            if (!GetSettings(key).ContainsKey(device)) showData[key].Add(device, false);

            showData[key][device] = value;
        }

        private Dictionary<Device, bool> GetSettings(string key)
        {
            if (!showData.ContainsKey(key)) showData.Add(key, new Dictionary<Device, bool>());

            return showData[key];
        }
    }
}