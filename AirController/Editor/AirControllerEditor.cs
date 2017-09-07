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

        public override void OnInspectorGUI()
        {
            controller = (AirController)target;

            DrawSettings();
            DrawStats();

            foreach (Device d in controller.Devices.Values)
            {
                DrawDevice(d);
            }


            EditorUtility.SetDirty(controller);
        }

        private void DrawSettings()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            controller.heroMode = (AirController.HEROMODE)EditorGUILayout.EnumPopup("Hero Mode", controller.heroMode);

            controller.joinMode = (AirController.JOINMODE)EditorGUILayout.EnumPopup("Join Mode", controller.joinMode);
            controller.maxPlayersMode = (AirController.MAXPLAYERSMODE)EditorGUILayout.EnumPopup("Max players mode", controller.maxPlayersMode);

            if(controller.maxPlayersMode == AirController.MAXPLAYERSMODE.LIMITED)
                controller.maxPlayers = EditorGUILayout.IntField("Max players", controller.maxPlayers);

            controller.debug = EditorGUILayout.Toggle("Debug", controller.debug);

            EditorGUILayout.EndVertical();
        }

        private void DrawStats ()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);

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
            EditorGUILayout.LabelField("" + device.NickName, EditorStyles.boldLabel);

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
            EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);

            foreach (string key in device.Input.Axis.Keys)
            {
                EditorGUILayout.LabelField(key + ":", "(" + device.Input.Axis[key].x + ", " + device.Input.Axis[key].y + ")");
            }

            foreach (string key in device.Input.Keys.Keys)
            {
                EditorGUILayout.LabelField(key + ":", device.Input.Keys[key].value.ToString());
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawPlayer(Player player)
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.LabelField("Player " + player.PlayerId, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("state: ", "" + player.state);

            EditorGUILayout.EndVertical();
        }
    }
}