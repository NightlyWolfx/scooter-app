namespace ScooterRental.Exceptions
{
    internal class RentalRecordNotFoundException : Exception
    {
        public RentalRecordNotFoundException() : base("Rental record not found")
        { 
        }
    }
}
