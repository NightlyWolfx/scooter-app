using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests
{
    [TestClass]
    public class ScooterServiceTests
    {
        private IScooterService _scooterService;
        private List<Scooter> _scooterStorage;
        private const string DEFAULT_SCOOTER_ID = "1";
        private const decimal DEFAULT_PRICE_PER_MINUTE = 0.2m;

        [TestInitialize]
        public void Setup()
        {
            _scooterStorage = new List<Scooter>();
            _scooterService = new ScooterService(_scooterStorage);
        }

    // ================
    // ADD SCOOTER TESTS :
    // ================

        [TestMethod]
        public void AddScooter_WithIdAndPricePerMinute_ScooterAdded()
        {
            _scooterService.AddScooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Count.Should().Be(1);
        }

        [TestMethod]
        public void AddScooter_WithIdAndPricePerMinute1_ScooterAddedWithIdAndPrice1()
        {
            _scooterService.AddScooter(DEFAULT_SCOOTER_ID, 1m);

            var scooter = _scooterStorage.First();

            scooter.Id.Should().Be(DEFAULT_SCOOTER_ID);
            scooter.PricePerMinute.Should().Be(1m);
        }

        [TestMethod]
        public void AddScooter_AddDuplicateScooter_ThrowsDuplicateScooterException()
        {
            _scooterStorage.Add(new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE));

            Action action = () => _scooterService.AddScooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            action.Should().Throw<DuplicateScooterException>();
        }

        [TestMethod]
        public void AddScooter_AddScooterNegativePrice_ThrowsNegativePriceException()
        {
            Action action = () => _scooterService.AddScooter(DEFAULT_SCOOTER_ID, -1);

            action.Should().Throw<NegativePriceException>();
        }

        [TestMethod]
        public void AddScooter_AddScooterWithEmptyId_ThrowsInvalidIdException()
        {
            Action action = () => _scooterService.AddScooter("", DEFAULT_PRICE_PER_MINUTE);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void AddScooter_AddScooterWithNullId_ThrowsInvalidIdException()
        {
            Action action = () => _scooterService.AddScooter(null, DEFAULT_PRICE_PER_MINUTE);

            action.Should().Throw<InvalidIdException>();
        }

    // ================
    // REMOVE SCOOTER TESTS :
    // ================

        [TestMethod]
        public void RemoveScooter_RemoveScooterWithNullId_ThrowsInvalidIdException()
        {
            Action action = () => _scooterService.RemoveScooter(null);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void RemoveScooter_RemoveScooterWithEmptyId_ThrowsInvalidIdException()
        {
            Action action = () => _scooterService.RemoveScooter("");

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void RemoveScooter_IdDoesNotExistInScooterList_ThrowsScooterNotFoundException()
        {
            string thisScooterIdDefinitelyDoesNotExist = (_scooterStorage.Count + 1).ToString();

            Action action = () => _scooterService.RemoveScooter(thisScooterIdDefinitelyDoesNotExist);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void RemoveScooter_WithId_ScooterRemoved()
        {
            _scooterService.AddScooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);
            _scooterService.RemoveScooter(DEFAULT_SCOOTER_ID);

            _scooterStorage.Count.Should().Be(0);
        }

    // ================
    // GETSCOOTERBYID TESTS :
    // ================

        [TestMethod]
        public void GetScooterById_WithEmptyId_ThrowsInvalidIdException()
        {
            Action action = () => _scooterService.GetScooterById("");

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void GetScooterById_WithNullId_ThrowsInvalidIdException()
        {
            Action action = () => _scooterService.GetScooterById(null);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void GetScooterById_WithCorrectScooterId_ShouldReturnScooterObject()
        {
            _scooterStorage.Add(new Scooter("1", DEFAULT_PRICE_PER_MINUTE));

            var result = _scooterService.GetScooterById("1");

            result.GetType().Should().Be(typeof(Scooter));
        }

        [TestMethod]
        public void GetScooterById_WithCorrectScooterId_ShouldReturnScooterWithThatIdFromList()
        {
            _scooterStorage.Add(new Scooter("1", DEFAULT_PRICE_PER_MINUTE));
            _scooterStorage.Add(new Scooter("2", DEFAULT_PRICE_PER_MINUTE));
            _scooterStorage.Add(new Scooter("3", DEFAULT_PRICE_PER_MINUTE));

            var result = _scooterService.GetScooterById("2");

            result.Id.Should().Be("2");
        }

    // ================
    // GET SCOOTERS TEST :
    // ================

        [TestMethod]
        public void GetScooters_ShouldReturnAListOfTypeScooter()
        {
            _scooterStorage.Add(new Scooter("1", DEFAULT_PRICE_PER_MINUTE));
            _scooterStorage.Add(new Scooter("2", DEFAULT_PRICE_PER_MINUTE));
            _scooterStorage.Add(new Scooter("3", DEFAULT_PRICE_PER_MINUTE));

            var result = _scooterService.GetScooters();

            result.GetType().Should().Be(typeof(List<Scooter>));
        }

        [TestMethod]
        public void GetScooters_ShouldReturnListContainingAvailableScooters()
        {
            _scooterStorage.Add(new Scooter("1", DEFAULT_PRICE_PER_MINUTE));
            _scooterStorage.Add(new Scooter("2", DEFAULT_PRICE_PER_MINUTE));
            _scooterStorage.Add(new Scooter("3", DEFAULT_PRICE_PER_MINUTE));

            var expectedResult = new List<Scooter> { 
                new Scooter("1", DEFAULT_PRICE_PER_MINUTE),
                new Scooter("2", DEFAULT_PRICE_PER_MINUTE),
                new Scooter("3", DEFAULT_PRICE_PER_MINUTE)
            };

            _scooterService.GetScooters().Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void GetScooters_CountShouldBeEqualToAvailableScooterCount()
        {
            _scooterStorage.Add(new Scooter("1", DEFAULT_PRICE_PER_MINUTE));
            _scooterStorage.Add(new Scooter("2", DEFAULT_PRICE_PER_MINUTE));
            _scooterStorage.Add(new Scooter("3", DEFAULT_PRICE_PER_MINUTE));

            _scooterService.GetScooters().Count.Should().Be(3);
        }
    }
}