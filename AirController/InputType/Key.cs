using System;
using System.Collections;
using UnityEngine;

namespace SwordGC.AirController.InputTypes
{
    public class Key : IInput
    {
        public enum TYPE { TAP, HOLD }
        public TYPE type;

        public bool active = false;
        public bool prevActive = false;

        public int value;
        public float cooldown;

        public void HandleData(JSONObject data)
        {
            // data["position"] has the info

            if (data["type"].str == "tap-button")
            {
                OnDown(int.Parse(data["value"].str != null ? data["value"].str : "0"), TYPE.TAP);
                return;
            }

            if (data["type"].str == "hold-button")
            {
                if (data["event"].str == "down")
                {
                    OnDown(int.Parse(data["value"].str != null ? data["value"].str : "0"), TYPE.HOLD);
                    return;
                }

                if (!prevActive)
                {
                    AirController.Instance.StartCoroutine(Tap(int.Parse(data["value"].str != null ? data["value"].str : "0"), TYPE.HOLD));
                    return;
                }

                OnUp();
            }
        }

        public void Update()
        {
            prevActive = active;

            if (type == TYPE.TAP)
            {
                cooldown -= Time.deltaTime;
                active = false;
            }
        }

        private void OnDown(int value, TYPE type)
        {
            if (cooldown > 0)
            {
                return;
            }

            active = true;
            this.value = value;
            this.type = type;

            if (type == TYPE.TAP)
            {
                cooldown = Input.BUTTON_COOLDOWN;
            }
        }

        private void OnUp()
        {
            active = false;
        }

        private IEnumerator Tap(int value, TYPE type)
        {
            OnDown(value, type);

            yield return new WaitForEndOfFrame();

            OnUp();
        }
    }
}