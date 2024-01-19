namespace ScooterRental.Exceptions
{
    public class ScooterNotFoundException : Exception
    {
        public ScooterNotFoundException() : base("Scooter does not exist in list")
        {
        }
    }
}
