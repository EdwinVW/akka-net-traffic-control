namespace Messages
{
    public class GetVehicleInfo
    {
        public string VehicleId { get; private set; }

        public GetVehicleInfo(string vehicleId)
        {
            VehicleId = vehicleId;
        }
    }
}
