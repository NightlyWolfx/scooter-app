namespace ScooterRental.Exceptions
{
    public class ScooterIsNotCurrentlyRentedException : Exception
    {
        public ScooterIsNotCurrentlyRentedException() : base("Scooter is not currently rented")
        {
        }
    }
}
