namespace Messages
{
    public class VehicleInfoAvailable
    {
        public string VehicleId { get; private set; }

        public string Brand { get; private set; }

        public string Color { get; private set; }

        public VehicleInfoAvailable(string vehicleId, string brand, string color)
        {
            VehicleId = vehicleId;
            Brand = brand;
            Color = color;
        }
    }
}
