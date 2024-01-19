namespace ScooterRental.Exceptions
{
    public class ScooterIsAlreadyRentedException : Exception
    {
        public ScooterIsAlreadyRentedException() : base("Scooter is already rented")
        {
        }
    }
}