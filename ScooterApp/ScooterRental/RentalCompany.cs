using ScooterRental.Exceptions;

namespace ScooterRental
{
    public class RentalCompany : IRentalCompany
    {
        public string Name { get; }
        private uint _rentalRecordNumber = 1;
        private readonly IScooterService _scooterService;
        private readonly List<RentalRecord> _rentalRecordsList;
        private readonly RentalPriceCalculator _rentalPrice = new RentalPriceCalculator(0.2M, 20M);

        public RentalCompany(string name,
            IScooterService scooterService,
            List<RentalRecord> rentalRecordsList)
        {
            Name = name;
            _scooterService = scooterService;
            _rentalRecordsList = rentalRecordsList;
        }

        public void StartRent(string id)
        {
            RunStartRentTests(id);

            var scooter = _scooterService.GetScooterById(id);

            scooter.IsRented = true;
            _rentalRecordsList.Add(new RentalRecord(_rentalRecordNumber, scooter.Id, DateTime.Now));
            _rentalRecordNumber++;
        }

        public decimal EndRent(string id)
        {
            RunEndRentTests(id);

            var scooter = _scooterService.GetScooterById(id);

            scooter.IsRented = false;

            var rentalRecord = _rentalRecordsList
                .FirstOrDefault(s => s.ScooterId == scooter.Id && !s.RentEndTime.HasValue);
            rentalRecord.RentEndTime = DateTime.Now;

            decimal totalRentEndPrice = _rentalPrice.CalculateRentalPrice(
                rentalRecord.RentStartTime, rentalRecord.RentEndTime.Value);

            return totalRentEndPrice;
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            decimal income = _rentalPrice.CalculateIncome(
                year, includeNotCompletedRentals, _rentalRecordsList);

            return income;
        }
        
        private Scooter RunScooterIdTests(string id)
        {
            var scooter = _scooterService.GetScooterById(id);

            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidIdException();
            }
             
            if (scooter == null || scooter.Id != id)
            {
                throw new ScooterNotFoundException();
            }

            return _scooterService.GetScooterById(id);
        }

        private void RunStartRentTests(string id)
        {
            var scooter = RunScooterIdTests(id);

            if (scooter.IsRented == true)
            {
                throw new ScooterIsAlreadyRentedException();
            }
        }

        private void RunEndRentTests(string id)
        {
            var scooter = RunScooterIdTests(id);
            var rentalRecord = _rentalRecordsList
                .FirstOrDefault(s => s.ScooterId == scooter.Id && !s.RentEndTime.HasValue);

            if (scooter.IsRented == false)
            {
                throw new ScooterIsNotCurrentlyRentedException();
            }

            if (rentalRecord == null || 
                _rentalRecordsList.Count == 0)
            {
                throw new RentalRecordNotFoundException();
            }
        }
    }
}