using System;

namespace Messages
{
    public class VehicleExitRegistered
    {
        public string VehicleId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public VehicleExitRegistered(string vehicleId, DateTime timestamp)
        {
            VehicleId = vehicleId;
            Timestamp = timestamp;
        }
    }
}
