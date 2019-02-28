namespace SwordGC.AirController.InputTypes
{
    interface IInput
    {
        void HandleData(JSONObject data);
        void Update();
    }
}
