using Akka.Actor;
using Messages;
using System;

namespace Actors
{
    /// <summary>
    /// Actor that simulates traffic.
    /// </summary>
    public class SimulationActor : UntypedActor
    {
        private int _numberOfCars;
        private int _carsSimulated;
        private Random _rnd;

        private int _minEntryDelayInMS = 50;
        private int _maxEntryDelayInMS = 5000;
        private int _minExitDelayInS = 5;
        private int _maxExitDelayInS = 7;

        /// <summary>
        /// Handle received message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case StartSimulation ss:
                    Handle(ss);
                    break;
                case SimulatePassingCar spc:
                    Handle(spc);
                    break;            
                case Shutdown sd:
                    Context.Stop(Self);
                    break;
            }
        }   

        /// <summary>
        /// Handle StartSimulation message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(StartSimulation msg)
        {
            // initialize state
            _numberOfCars = msg.NumberOfCars;
            _carsSimulated = 0;
            _rnd = new Random();

            // start simulationloop
            SimulatePassingCar simulatePassingCar = new SimulatePassingCar(GenerateRandomLicenseNumber());
            Context.System.Scheduler.ScheduleTellOnce(
                _rnd.Next(_minEntryDelayInMS, _maxEntryDelayInMS), Self, simulatePassingCar, Self);
        }

        /// <summary>
        /// Handle SimulatePassingCar message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(SimulatePassingCar msg)
        {
            //  simulate car entry
            int entryLane = _rnd.Next(1, 4);
            ActorSelection entryCamera = Context.System.ActorSelection($"/user/entrycam{entryLane}");
            DateTime entryTimestamp = DateTime.Now;
            VehiclePassed vehiclePassed = new VehiclePassed(msg.VehicleId, entryTimestamp);
            entryCamera.Tell(vehiclePassed);

            // simulate car exit
            int exitLane = _rnd.Next(1, 4);
            TimeSpan delay = TimeSpan.FromSeconds(_rnd.Next(_minExitDelayInS, _maxExitDelayInS) + _rnd.NextDouble());
            DateTime exitTimestamp = entryTimestamp.Add(delay);
            ActorSelection exitCamera = Context.System.ActorSelection($"/user/exitcam{entryLane}");
            vehiclePassed = new VehiclePassed(msg.VehicleId, exitTimestamp);
            Context.System.Scheduler.ScheduleTellOnce(delay, exitCamera, vehiclePassed, Self);

            // handle progress
            _carsSimulated++;
            if (_carsSimulated < _numberOfCars)
            {
                SimulatePassingCar simulatePassingCar = new SimulatePassingCar(GenerateRandomLicenseNumber());
                Context.System.Scheduler.ScheduleTellOnce(
                    _rnd.Next(_minEntryDelayInMS, _maxEntryDelayInMS), Self, simulatePassingCar, Self);
            }
            else
            {
                Self.Tell(new Shutdown());
            }
        }

        #region Private helper methods

        private string _validLicenseNumberChars = "DFGHJKLNPRSTXYZ";

        /// <summary>
        /// Generate random licensenumber.
        /// </summary>
        private string GenerateRandomLicenseNumber()
        {
            int type = _rnd.Next(1, 9);
            string kenteken = null;
            switch (type)
            {
                case 1: // 99-AA-99
                    kenteken = string.Format("{0:00}-{1}-{2:00}", _rnd.Next(1, 99), GenerateRandomCharacters(2), _rnd.Next(1, 99));
                    break;
                case 2: // AA-99-AA
                    kenteken = string.Format("{0}-{1:00}-{2}", GenerateRandomCharacters(2), _rnd.Next(1, 99), GenerateRandomCharacters(2));
                    break;
                case 3: // AA-AA-99
                    kenteken = string.Format("{0}-{1}-{2:00}", GenerateRandomCharacters(2), GenerateRandomCharacters(2), _rnd.Next(1, 99));
                    break;
                case 4: // 99-AA-AA
                    kenteken = string.Format("{0:00}-{1}-{2}", _rnd.Next(1, 99), GenerateRandomCharacters(2), GenerateRandomCharacters(2));
                    break;
                case 5: // 99-AAA-9
                    kenteken = string.Format("{0:00}-{1}-{2}", _rnd.Next(1, 99), GenerateRandomCharacters(3), _rnd.Next(1, 10));
                    break;
                case 6: // 9-AAA-99
                    kenteken = string.Format("{0}-{1}-{2:00}", _rnd.Next(1, 9), GenerateRandomCharacters(3), _rnd.Next(1, 10));
                    break;
                case 7: // AA-999-A
                    kenteken = string.Format("{0}-{1:000}-{2}", GenerateRandomCharacters(2), _rnd.Next(1, 999), GenerateRandomCharacters(1));
                    break;
                case 8: // A-999-AA
                    kenteken = string.Format("{0}-{1:000}-{2}", GenerateRandomCharacters(1), _rnd.Next(1, 999), GenerateRandomCharacters(2));
                    break;
            }

            return kenteken;
        }

        private string GenerateRandomCharacters(int aantal)
        {
            char[] chars = new char[aantal];
            for (int i = 0; i < aantal; i++)
            {
                chars[i] = _validLicenseNumberChars[_rnd.Next(_validLicenseNumberChars.Length - 1)];
            }
            return new string(chars);
        }

        #endregion
    }
}
