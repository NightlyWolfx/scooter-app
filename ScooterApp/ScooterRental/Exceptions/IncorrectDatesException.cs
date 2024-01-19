namespace ScooterRental.Exceptions
{
    public class IncorrectDatesException : Exception
    {
        public IncorrectDatesException() : base("Rental record start date cannot be greater than end date. Please verify the record dates are correct")
        {
        }
    }
}
