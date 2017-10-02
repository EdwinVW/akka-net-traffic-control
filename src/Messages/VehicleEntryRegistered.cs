using System;

namespace Messages
{
    public class VehicleEntryRegistered
    {
        public string VehicleId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public VehicleEntryRegistered(string vehicleId, DateTime timestamp)
        {
            VehicleId = vehicleId;
            Timestamp = timestamp;
        }
    }
}
