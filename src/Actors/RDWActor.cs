using Akka.Actor;
using Messages;
using System;

namespace Actors
{
    /// <summary>
    /// Actor that handles communication with the department of motorvehicles.
    /// </summary>
    public class RDWActor : ReceiveActor
    {
        private Random _rnd = new Random();

        public RDWActor()
        {
            // setup message-handling
            Receive<GetVehicleInfo>(msg => Handle(msg));
        }

        /// <summary>
        /// Handle a GetVehicleInfo message.
        /// </summary>
        /// <param name="msg">The message to process.</param>
        private void Handle(GetVehicleInfo msg)
        {
            // simulate web-service call
            VehicleInfoAvailable info = new VehicleInfoAvailable(msg.VehicleId, GetRandomBrand(), GetRandomColor());
            Sender.Tell(info);
        }

        #region Privite helper methods

        private string[] _vehicleBrands = new string[] { "Mercedes", "Toyota", "Saab", "Audi", "BWW", "Volkswagen", "Seat", "Renault", "Skoda", "Kia", "Seat" };
        private string[] _vehicleColors = new string[] { "Black", "White", "Grey", "Red", "Blue", "Green", "Silver" };


        /// <summary>
        /// Get a random VehicleType.
        /// </summary>
        private string GetRandomBrand()
        {
            return _vehicleBrands[_rnd.Next(_vehicleBrands.Length)];
        }

        /// <summary>
        /// Get a random Color.
        /// </summary>
        private string GetRandomColor()
        {
            return _vehicleColors[_rnd.Next(_vehicleColors.Length)];
        }

        #endregion
    }
}
