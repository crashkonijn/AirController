namespace SwordGC.AirController.Examples.Pong
{
    public class PongDevice : Device
    {

        public PongDevice(int deviceId) : base(deviceId)
        {

        }

        public override string Classes
        {
            get
            {
                if (HasPlayer)
                {
                    return "player" + PlayerId;
                }

                return "";
            }
        }

        public override string View
        {
            get
            {
                if (HasPlayer)
                {
                    if (airController.PlayersAvailable == 0)
                    {
                        return "Gameplay";
                    }

                    return "Waiting";
                }

                return "Full";
            }
        }
    }
}