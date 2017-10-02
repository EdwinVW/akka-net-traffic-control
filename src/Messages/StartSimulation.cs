namespace Messages
{
    public class StartSimulation
    {
        public int NumberOfCars { get; private set; }

        public StartSimulation(int numberOfCars)
        {
            NumberOfCars = numberOfCars;
        }
    }
}
