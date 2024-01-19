using ScooterRental.Exceptions;

namespace ScooterRental
{
    public class ScooterService : IScooterService
    {
        private readonly List<Scooter> _scooters;

        public ScooterService(List<Scooter> scooterStorage)
        {
            _scooters = scooterStorage;
        }

        public void AddScooter(string id, decimal pricePerMinute)
        {
            RunAddScooterTests(id, pricePerMinute);

            _scooters.Add(new Scooter(id, pricePerMinute));
        }

        public void RemoveScooter(string id)
        {
            RunRemoveScooterTests(id);

            _scooters.Remove(GetScooterById(id));
        }

        public IList<Scooter> GetScooters()
        {
            return _scooters.ToList();
        }

        public Scooter GetScooterById(string scooterId)
        {
            RunGetScooterByIdTests(scooterId);

            return _scooters.FirstOrDefault(s => s.Id == scooterId);
        }

        private void ValidateScooterId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidIdException();
            }
        }

        private void CheckForDuplicateScootersInList(string id)
        {
            // todo
        }

        private void RunGetScooterByIdTests(string id)
        {
            ValidateScooterId(id);
        }

        private void RunAddScooterTests(string id, decimal pricePerMinute)
        {
            ValidateScooterId(id);

            if (pricePerMinute <= 0)
            {
                throw new NegativePriceException();
            }

            if (_scooters.Any(s => s.Id == id))
            {
                throw new DuplicateScooterException();
            }
        }

        private void RunRemoveScooterTests(string id)
        {
            ValidateScooterId(id);

            if (!_scooters.Any(s => s.Id == id))
            {
                throw new ScooterNotFoundException();
            }
        }
    }
}
