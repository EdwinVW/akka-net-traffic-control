using Akka.Actor;
using Messages;
using System;

namespace Actors
{
    /// <summary>
    /// Actor that represents a registered vehicle.
    /// </summary>
    public class VehicleActor : UntypedActor
    {
        string _vehicleId;
        string _brand = "Unknown color";
        string _color = "Unknown brand";
        DateTime _entryTimestamp;
        DateTime? _exitTimestamp;

        private RoadInfo _roadInfo;

        double _elapsedMinutes;
        double _avgSpeedInKmh;

        IActorRef _dmvActor;
        ActorSelection _cjcaActor;

        public VehicleActor(RoadInfo roadInfo)
        {
            // initialize state
            _roadInfo = roadInfo;
        }

        /// <summary>
        /// Handle received message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case VehicleEntryRegistered ver:
                    Handle(ver);
                    break;
                case VehicleExitRegistered vxr:
                    Handle(vxr);
                    break;            
                case VehicleInfoAvailable via:
                    Handle(via);
                    break; 
                case MissingCarDetected mcd:
                    Handle(mcd);
                    break;                        
                case Shutdown sd:
                    Handle(sd);
                    break;                        
            }
        }         

        /// <summary>
        /// Handle VehicleEntryRegistered message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(VehicleEntryRegistered msg)
        {
            FluentConsole.Green.Line($"Vehicle '{msg.VehicleId}' entered at {msg.Timestamp.ToString("HH:mm:ss.ffffff")}");

            _vehicleId = msg.VehicleId;
            _entryTimestamp = msg.Timestamp;

            _dmvActor = Context.ActorOf<DMVActor>();
            _dmvActor.Tell(new GetVehicleInfo(_vehicleId));

            // set a time-out after wich we consider a car missing
            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromMinutes(5), Self, new MissingCarDetected(msg.VehicleId), Self);
        }

        /// <summary>
        /// Handle VehicleInfoAvailable message.
        /// </summary>
        /// <param name="msg">Message to handle.</param>
        private void Handle(VehicleInfoAvailable msg)
        {
            _brand = msg.Brand;
            _color = msg.Color;
        }

        /// <summary>
        /// Handle VehicleExitRegistered message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(VehicleExitRegistered msg)
        {
            _exitTimestamp = msg.Timestamp;

            // check speed limit
            int speedingViolation = DetermineSpeedingViolation();

            // log exit
            if (speedingViolation > 0)
            {
                FluentConsole.Red.Line($"{_color} {_brand} '{msg.VehicleId}' exited at {msg.Timestamp.ToString("HH:mm:ss.ffffff")}" + 
                    $"(avg speed {_avgSpeedInKmh} km/h - {speedingViolation} km/h over speed-limit (after correction))");

                // register speeding violation
                _cjcaActor = Context.ActorSelection("/user/cjcaactor");
                var rsv = new RegisterSpeedingViolation(_vehicleId, _roadInfo.RoadId, speedingViolation);
                _cjcaActor.Tell(rsv);
            }
            else
            {
                FluentConsole.Yellow.Line($"{_color} {_brand} '{msg.VehicleId}' exited at {msg.Timestamp.ToString("HH:mm:ss.ffffff")}" + 
                    $"(avg speed {_avgSpeedInKmh} km/h)");
            }

            Self.Tell(new Shutdown());
        }

        /// <summary>
        /// Handle Shutdown message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(Shutdown msg)
        {
            if (_dmvActor != null)
            {
                Context.Stop(_dmvActor);
            }
            Context.Stop(Self);
        }

        /// <summary>
        /// Handle MissingCarDetected message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(MissingCarDetected msg)
        {
            FluentConsole.Magenta.Line($"Vehicle '{msg.VehicleId}' is missing. Sending road assistance.");

            // ...
        }

        #region Private helper methods

        /// <summary>
        /// Determine whether or not the vehicle was speeding.
        /// </summary>
        /// <returns>Violation in Km/h after correction.</returns>
        private int DetermineSpeedingViolation()
        {
            //_elapsedMinutes = _exitTimestamp.Value.Subtract(_entryTimestamp).TotalMinutes;
            _elapsedMinutes = _exitTimestamp.Value.Subtract(_entryTimestamp).TotalSeconds; // 1 sec. == 1 min. in simulation
            _avgSpeedInKmh = Math.Round((_roadInfo.SectionLengthInKm / _elapsedMinutes) * 60);
            int violation = Convert.ToInt32(_avgSpeedInKmh - _roadInfo.MaxAllowedSpeedInKmh - _roadInfo.LegalCorrectionInKmh);
            return violation;
        }

        #endregion
    }
}
