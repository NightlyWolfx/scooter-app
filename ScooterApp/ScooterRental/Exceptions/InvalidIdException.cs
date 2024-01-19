namespace ScooterRental
{
    public class InvalidIdException : Exception 
    {
        public InvalidIdException() : base("Id cannot be null or empty")
        { 
        }
    }
}