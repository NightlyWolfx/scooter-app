namespace ScooterRental
{
    public class RentalRecord
    {
        public RentalRecord(uint recordNumber, string scooterId, DateTime rentStartTime)
        {
            RecordNumber = recordNumber;
            ScooterId = scooterId;
            RentStartTime = rentStartTime;
        }

        public uint RecordNumber { get; }
        public string ScooterId { get; }
        public DateTime RentStartTime { get; }
        public DateTime? RentEndTime { get; set; }
        public decimal? TotalRentPrice { get; set; }
    }
}
