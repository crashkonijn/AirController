using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SwordGC.AirController.Examples.Controller
{
    public class MenuController : MonoBehaviour
    {
        public enum STATE { MENU, BUTTONS, SWIPE, PAN, MOTION, SHAKE, JOYSTICK, PROFILE }
        public STATE state = STATE.MENU;
        public STATE prevState = STATE.MENU;

        [Header("Menu variables")]
        public CanvasGroup menuCanvas;

        [Header("Button variables")]
        public CanvasGroup buttonsCanvas; 
        public Image buttonsTap;
        public Image buttonsHold;
        public Image buttonsNumber;
        public Text buttonsNumberText;
        public Image buttonsHero;

        [Header("Swipe variables")]
        public CanvasGroup swipeCanvas;
        public Text swipeText;

        [Header("Pan variables")]
        public CanvasGroup panCanvas;
        public RectTransform panImage;

        [Header("Motion variables")]
        public CanvasGroup motionCanvas;
        public Text motionText;

        [Header("Shake variables")]
        public CanvasGroup shakeCanvas;

        [Header("Joystick variables")]
        public CanvasGroup joystickCanvas;
        public RectTransform joystickImage;

        [Header("Profile variables")]
        public CanvasGroup profileCanvas;
        public RawImage profileImage;
        public Text profileText;

        Player activePlayer;

        void Start()
        {
            SwitchState(STATE.MENU);
        }

        void Update()
        {
            CheckInput();
        }

        void CheckInput ()
        {
            if (AirController.Instance.Players.Count == 0) return;

            activePlayer = AirController.Instance.Players.Values.ToList()[0];

            if (CheckBackButton(activePlayer))
            {
                SwitchState(STATE.MENU);
                return;
            }

            switch (state)
            {
                case STATE.MENU:
                    CheckMenuInput(activePlayer);
                    break;
                case STATE.BUTTONS:
                    CheckButtonsInput(activePlayer);
                    break;
                case STATE.SWIPE:
                    CheckSwipeInput(activePlayer);
                    break;
                case STATE.PAN:
                    CheckPanInput(activePlayer);
                    break;
                case STATE.MOTION:
                    CheckMotionInput(activePlayer);
                    break;
                case STATE.SHAKE:
                    CheckShakeInput(activePlayer);
                    break;
                case STATE.JOYSTICK:
                    CheckJoystickInput(activePlayer);
                    break;
                case STATE.PROFILE:
                    CheckProfileInput(activePlayer);
                    break;
            }
        }

        bool CheckBackButton (Player p)
        {
            return p.Input.GetKeyDown("back");
        }

        void CheckMenuInput (Player p)
        {
            if(p.Input.GetKeyDown("menu"))
            {
                switch (p.Input.GetKeyValue("menu"))
                {
                    case 0:
                        SwitchState(STATE.BUTTONS);
                        break;
                    case 1:
                        SwitchState(STATE.SWIPE);
                        break;
                    case 2:
                        SwitchState(STATE.PAN);
                        break;
                    case 3:
                        SwitchState(STATE.MOTION);
                        break;
                    case 4:
                        SwitchState(STATE.SHAKE);
                        break;
                    case 5:
                        SwitchState(STATE.JOYSTICK);
                        break;
                    case 6:
                        SwitchState(STATE.PROFILE);
                        break;
                }
            }
        }

        void CheckButtonsInput(Player p)
        {
            buttonsTap.color = p.Input.GetKeyDown("tap") ? Color.red : Color.Lerp(buttonsTap.color, Color.white, 0.2f);
            buttonsHold.color = p.Input.GetKey("hold") ? Color.red : Color.Lerp(buttonsHold.color, Color.white, 0.2f);
            buttonsNumber.color = p.Input.GetKeyDown("number") ? Color.red : Color.Lerp(buttonsNumber.color, Color.white, 0.2f);
            buttonsNumberText.text = "Number " + p.Input.GetKeyValue("number");
            buttonsHero.color = p.Input.GetKeyDown("hero") ? Color.red : Color.Lerp(buttonsHero.color, Color.white, 0.2f);
        }

        void CheckSwipeInput (Player p)
        {
            p.Input.Swipe.Swiped((InputTypes.TouchSwipe.DIRECTION dir) => {
                swipeText.text = dir.ToString();
                swipeText.color = new Vector4(1f, 1f, 1f, 1f);
                swipeText.rectTransform.localScale = Vector2.one * 2f;
            });

            swipeText.color = Color.Lerp(swipeText.color, new Vector4(1f, 1f, 1f, 0f), Time.deltaTime * 5f);
            swipeText.rectTransform.localScale = Vector2.Lerp(swipeText.rectTransform.localScale, Vector2.one, Time.deltaTime * 5f);
        }

        void CheckPanInput (Player p)
        {
            p.Input.Pan.Touching((Vector2 start, Vector2 cur) => {
                panImage.localPosition = Vector2.Lerp(panImage.localPosition, (cur - start) * 100f, Time.deltaTime * 5f);
            });
            p.Input.Pan.TouchEnd((Vector2 start, Vector2 cur) => {
                panImage.localPosition = Vector2.zero;
            });
        }

        void CheckMotionInput (Player p)
        {
            string text = p.Input.Orientation.State.ToString() + "\n\n";

            text += "Roll " + p.Input.Motion.GetRoll(p.Input.Orientation.State) + "\n\n";

            text += "Tilt " + p.Input.Motion.GetTilt(p.Input.Orientation.State) + "\n\n";

            motionText.text = text;
        }

        void CheckShakeInput (Player p)
        {

        }

        void CheckJoystickInput (Player p)
        {
            joystickImage.localPosition = Vector2.Lerp(joystickImage.localPosition, p.Input.GetVector("movement") * 100f, 0.2f);
        }

        void CheckProfileInput (Player p)
        {
            profileImage.texture = p.ProfilePicture;
            profileText.text = p.Nickname;
        }

        void SwitchState (STATE newState)
        {
            Debug.Log("Switch state to");

            GetCanvas(state).alpha = 0f;
            state = newState;
            GetCanvas(state).alpha = 1f;

            AirController.Instance.UpdateDeviceStates();
        }

        CanvasGroup GetCanvas(STATE state)
        {
            switch (state)
            {
                case STATE.MENU: return menuCanvas;
                case STATE.BUTTONS: return buttonsCanvas;
                case STATE.SWIPE: return swipeCanvas;
                case STATE.PAN: return panCanvas;
                case STATE.SHAKE: return shakeCanvas;
                case STATE.MOTION: return motionCanvas;
                case STATE.JOYSTICK: return joystickCanvas;
                case STATE.PROFILE: return profileCanvas;
                default: return null;
            }
        }
    }
}