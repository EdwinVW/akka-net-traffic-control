using System;

namespace Messages
{
    public class VehiclePassed
    {
        public string VehicleId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public VehiclePassed(string vehicleId, DateTime timestamp)
        {
            VehicleId = vehicleId;
            Timestamp = timestamp;
        }
    }
}
