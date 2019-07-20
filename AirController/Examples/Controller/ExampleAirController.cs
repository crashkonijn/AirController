namespace SwordGC.AirController.Examples.Controller
{
    public class ExampleAirController : AirController
    {
        protected override void OnReady(string code)
        {
            base.OnReady(code);
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