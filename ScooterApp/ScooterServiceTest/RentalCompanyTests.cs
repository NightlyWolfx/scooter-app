using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Test
{
    [TestClass]
    public class RentalCompanyTests
    {
        private IRentalCompany _rentalCompany;
        private const string DEFAULT_COMPANY_NAME = "default";
        private List<Scooter> _scooterStorage;
        private List<RentalRecord> _rentalRecords;
        private const string DEFAULT_SCOOTER_ID = "1";
        private const decimal DEFAULT_PRICE_PER_MINUTE = 0.2m;
        private const decimal DEFAULT_MAX_PRICE_PER_DAY = 20M;
        private const uint DEFAULT_RENTAL_RECORD_NUMBER = 1;
        private RentalPriceCalculator _rentalPrice = new RentalPriceCalculator(
            DEFAULT_PRICE_PER_MINUTE, DEFAULT_MAX_PRICE_PER_DAY);

        [TestInitialize]
        public void Setup()
        {
            _scooterStorage = new List<Scooter>();
            _rentalRecords = new List<RentalRecord>();
            _rentalCompany = new RentalCompany(
                DEFAULT_COMPANY_NAME,
                new ScooterService(_scooterStorage),
                _rentalRecords);
        }

    // ================
    // START RENT TESTS :
    // ================

        [TestMethod]
        public void StartRent_IdIsNull_ShouldThrowInvalidIdException()
        {
            string id = null;

            Action action = () => _rentalCompany.StartRent(id);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void StartRent_IdIsEmpty_ShouldThrowInvalidIdException()
        {
            string id = "";

            Action action = () => _rentalCompany.StartRent(id);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void StartRent_ScooterWithSpecifiedIdDoesNotExistInScooterList_ShouldThrowScooterNotFoundException()
        {
            string id = (_scooterStorage.Count + 1).ToString();

            Action action = () => _rentalCompany.StartRent(id);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void StartRent_ScooterIsAlreadyRented_ShouldThrowScooterAlreadyRentedException()
        {
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);
            defaultScooter.IsRented = true;

            Action action = () => _rentalCompany.StartRent(DEFAULT_SCOOTER_ID);

            action.Should().Throw<ScooterIsAlreadyRentedException>();
        }

        [TestMethod]
        public void StartRent_WithCorrectId_ScooterShouldAppearRentedAfterStartingRent()
        {
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);

            _rentalCompany.StartRent(DEFAULT_SCOOTER_ID);

            defaultScooter.IsRented.Should().Be(true);
        }

        [TestMethod]
        public void StartRent_WithCorrectId_ShouldIncreaseTotalRentalRecordsListCount()
        {
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);
            var currentRentalRecordsCount = _rentalRecords.Count;

            _scooterStorage.Add(defaultScooter);

            _rentalCompany.StartRent(DEFAULT_SCOOTER_ID);

            _rentalRecords.Count.Should().Be(currentRentalRecordsCount + 1);
        }

        [TestMethod]
        public void StartRent_WithCorrectId_RentalRecordsShouldContainRecordWithExpectedScooterId()
        {
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);

            _rentalCompany.StartRent(DEFAULT_SCOOTER_ID);

            _rentalRecords[0].ScooterId.Should().Be(DEFAULT_SCOOTER_ID);
        }

        [TestMethod]
        public void StartRent_WithCorrectId_FirstTimeStartingRentShouldCreateRentalRecordWithDefaultRentalRecordNumber()
        {
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);

            _rentalCompany.StartRent(DEFAULT_SCOOTER_ID);

            _rentalRecords[0].RecordNumber.Should().Be(DEFAULT_RENTAL_RECORD_NUMBER);
        }

        [TestMethod]
        public void StartRent_WithCorrectId_SecondTimeStartingRentShouldCreateRentalRecordWithDefaultRecordNumberIncrementedBy1()
        {
            var scooter1 = new Scooter("1", DEFAULT_PRICE_PER_MINUTE);
            var scooter2 = new Scooter("2", DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(scooter1);
            _scooterStorage.Add(scooter2);

            _rentalCompany.StartRent("1");
            _rentalCompany.StartRent("2");

            _rentalRecords[1].RecordNumber.Should().Be(DEFAULT_RENTAL_RECORD_NUMBER + 1);
        }

        [TestMethod]
        public void StartRent_WithCorrectId_ThirdTimeStartingRentShouldCreateRentalRecordWithDefaultRecordNumberIncrementedTwice()
        {
            var scooter1 = new Scooter("1", DEFAULT_PRICE_PER_MINUTE);
            var scooter2 = new Scooter("2", DEFAULT_PRICE_PER_MINUTE);
            var scooter3 = new Scooter("3", DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(scooter1);
            _scooterStorage.Add(scooter2);
            _scooterStorage.Add(scooter3);

            _rentalCompany.StartRent("1");
            _rentalCompany.StartRent("2");
            _rentalCompany.StartRent("3");

            _rentalRecords[2].RecordNumber.Should().Be(DEFAULT_RENTAL_RECORD_NUMBER + 2);
        }

        [TestMethod]
        public void StartRent_WithCorrectId_RentalRecordNumberShouldIncreaseBy1WithEachNewStartedRental()
        {
            for (int i = 0; i < 10; i++)
            {
                string id = i.ToString();
                var scooter = new Scooter(id, DEFAULT_PRICE_PER_MINUTE);

                _scooterStorage.Add(scooter);

                _rentalCompany.StartRent(id);

                _rentalRecords[i].RecordNumber.Should().Be(DEFAULT_RENTAL_RECORD_NUMBER + (uint)i);
            }
        }

        [TestMethod]
        public void StartRent_WithCorrectId_RentalRecordForThatScooterShouldHaveNoRentEndTimeValue()
        {
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);

            _rentalCompany.StartRent(DEFAULT_SCOOTER_ID);

            _rentalRecords[0].RentEndTime.Should().Be(null);
        }

    // ================
    // END RENT TESTS :
    // ================

        [TestMethod]
        public void EndRent_IdIsNull_ShouldThrowInvalidIdException()
        {
            string id = null;

            Action action = () => _rentalCompany.EndRent(id);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void EndRent_IdIsEmpty_ShouldThrowInvalidIdException()
        {
            string id = "";

            Action action = () => _rentalCompany.EndRent(id);

            action.Should().Throw<InvalidIdException>();
        }

        [TestMethod]
        public void EndRent_ScooterWithSpecifiedIdDoesNotExistInScooterList_ShouldThrowScooterNotFoundException()
        {
            string id = (_scooterStorage.Count + 1).ToString();

            Action action = () => _rentalCompany.EndRent(id);

            action.Should().Throw<ScooterNotFoundException>();
        }

        [TestMethod]
        public void EndRent_WithCorrectId_ScooterIsNotRented_ShouldThrowScooterIsNotCurrentlyRentedException()
        {
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);

            Action action = () => _rentalCompany.EndRent(DEFAULT_SCOOTER_ID);

            action.Should().Throw<ScooterIsNotCurrentlyRentedException>();
        }

        [TestMethod]
        public void EndRent_WithCorrectId_ScooterShouldAppearAvailableAfterEndingRent()
        {
            DateTime timeNow = DateTime.Now;
            DateTime timeTenMinutesAgo = timeNow.AddMinutes(-10);
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);

            defaultScooter.IsRented = true;
            _rentalRecords.Add(new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, timeTenMinutesAgo));

            _rentalCompany.EndRent(DEFAULT_SCOOTER_ID);

            defaultScooter.IsRented.Should().Be(false);
        }

        [TestMethod]
        public void EndRent_WithCorrectId_AfterEndingRentItsRentalRecordShouldHaveAnEndTimeValue()
        {
            DateTime timeNow = DateTime.Now;
            DateTime timeTenMinutesAgo = timeNow.AddMinutes(-10);
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);

            defaultScooter.IsRented = true;
            _rentalRecords.Add(new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, timeTenMinutesAgo));

            _rentalCompany.EndRent(DEFAULT_SCOOTER_ID);

            var rentalRecord = _rentalRecords
                .FirstOrDefault(s => s.ScooterId == DEFAULT_SCOOTER_ID);
            rentalRecord.RentEndTime.Should().NotBeNull();
        }

        [TestMethod]
        public void EndRent_WithCorrectId_AfterEndingRentItsRentalRecordShouldHaveAnAccurateEndTime()
        {
            DateTime timeNow = DateTime.Now;
            DateTime timeTenMinutesAgo = timeNow.AddMinutes(-10);

            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);

            defaultScooter.IsRented = true;
            _rentalRecords.Add(new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, timeTenMinutesAgo));

            _rentalCompany.EndRent(DEFAULT_SCOOTER_ID);

            var rentalRecord = _rentalRecords
                .FirstOrDefault(s => s.ScooterId == DEFAULT_SCOOTER_ID);

            TimeSpan oneSecond = TimeSpan.FromSeconds(1);
            var timeDifference = (timeNow - rentalRecord.RentEndTime);

            timeDifference.Should().BeLessThan(oneSecond);
        }

        [TestMethod]
        public void EndRent_CalculateRentPriceForSameDayRentOf10Minutes_ExpectPriceToBeCorrect()
        {
            decimal expectedResult = 10M * DEFAULT_PRICE_PER_MINUTE;

            DateTime timeNow = DateTime.Now;
            DateTime timeTenMinutesAgo = timeNow.AddMinutes(-10);
            var defaultScooter = new Scooter(DEFAULT_SCOOTER_ID, DEFAULT_PRICE_PER_MINUTE);

            _scooterStorage.Add(defaultScooter);
            defaultScooter.IsRented = true;
            _rentalRecords.Add(new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, timeTenMinutesAgo));

            decimal price = _rentalCompany.EndRent(DEFAULT_SCOOTER_ID);
            price.Should().Be(expectedResult);
        }

    // ================
    // CALCULATE INCOME TESTS :
    // ================

        [TestMethod]
        public void CalculateIncome_CalculateSingleYearEarningsExcludingIncompleteRentals_EarningsAmountShouldbeCorrect()
        {
            GenerateRentalRecords(DateTime.Now);

            var result = _rentalCompany.CalculateIncome(2022, false);
            result.Should().Be(210m * DEFAULT_PRICE_PER_MINUTE);
        }

        [TestMethod]
        public void CalculateIncome_CalculatesAllTimeEarningsExcludingIncompleteRentals_ResultShouldBeCorrect()
        {
            GenerateRentalRecords(DateTime.Now);

            var result = _rentalCompany.CalculateIncome(null, false);
            result.Should().Be(890m * DEFAULT_PRICE_PER_MINUTE);
        }

        [TestMethod]
        public void CalculateIncome_CalculatesAllTimeEarningsIncludingIncompleteRentals_ResultShouldBeCorrect()
        {
            DateTime timeNow = DateTime.Now;
            GenerateRentalRecords(timeNow);

            decimal result = _rentalCompany.CalculateIncome(null, true);
            result.Should().Be(980m * DEFAULT_PRICE_PER_MINUTE);
        }

        [TestMethod]
        public void CalculateIncome_CalculatesSingleYearEarningsIncludingIncompleteRentals_EarningsAmountShouldbeCorrect()
        {
            DateTime timeNow = DateTime.Now;
            GenerateRentalRecords(timeNow);

            var year = 2023;
            decimal result = _rentalCompany.CalculateIncome(year, true);
            result.Should().Be(410m * DEFAULT_PRICE_PER_MINUTE);
        }

        private void GenerateRentalRecords(DateTime timeNow)
        {
            DateTime firstRecordStartTime = new DateTime(2020, 1, 1, 15, 0, 0);
            DateTime firstRecordEndTime = new DateTime(2020, 1, 1, 16, 0, 0);
            DateTime secondRecordStartTime = new DateTime(2021, 2, 2, 13, 0, 0);
            DateTime secondRecordEndTime = new DateTime(2021, 2, 2, 15, 0, 0);
            DateTime thirdRecordStartTime = new DateTime(2021, 3, 3, 13, 0, 0);
            DateTime thirdRecordEndTime = new DateTime(2021, 3, 3, 15, 0, 0);
            DateTime fourthRecordStartTime = new DateTime(2021, 4, 4, 15, 0, 0);
            DateTime fourthRecordEndTime = new DateTime(2021, 4, 4, 17, 0, 0);
            DateTime fifthRecordStartTime = new DateTime(2022, 5, 5, 16, 20, 0);
            DateTime fifthRecordEndTime = new DateTime(2022, 5, 5, 16, 30, 0);
            DateTime sixthRecordStartTime = new DateTime(2022, 6, 6, 17, 0, 0);
            DateTime sixthRecordEndTime = new DateTime(2022, 6, 7, 17, 0, 0);
            DateTime seventhRecordStartTime = new DateTime(2023, 8, 8, 15, 50, 0);
            DateTime seventhRecordEndTime = new DateTime(2023, 8, 8, 16, 10, 0);
            DateTime eighthRecordStartTime = new DateTime(2023, 9, 9, 17, 0, 0);
            DateTime eighthRecordEndTime = new DateTime(2023, 9, 11, 11, 0, 0);
            DateTime ninthRecordStartTime = timeNow.AddMinutes(-20);
            DateTime tenthRecordStartTime = timeNow.AddMinutes(-10);
            DateTime eleventhRecordStartTime = timeNow.AddHours(-1);

            var rentalRecord1 = new RentalRecord(1, "1", firstRecordStartTime);
            rentalRecord1.RentEndTime = firstRecordEndTime;
            rentalRecord1.TotalRentPrice = 12M;
            _rentalRecords.Add(rentalRecord1);

            var rentalRecord2 = new RentalRecord(2, "1", secondRecordStartTime);
            rentalRecord2.RentEndTime = secondRecordEndTime;
            rentalRecord2.TotalRentPrice = 20M;
            _rentalRecords.Add(rentalRecord2);

            var rentalRecord3 = new RentalRecord(3, "1", thirdRecordStartTime);
            rentalRecord3.RentEndTime = thirdRecordEndTime;
            rentalRecord3.TotalRentPrice = 20M;
            _rentalRecords.Add(rentalRecord3);

            var rentalRecord4 = new RentalRecord(4, "1", fourthRecordStartTime);
            rentalRecord4.RentEndTime = fourthRecordEndTime;
            rentalRecord4.TotalRentPrice = 20M;
            _rentalRecords.Add(rentalRecord4);

            var rentalRecord5 = new RentalRecord(5, "1", fifthRecordStartTime);
            rentalRecord5.RentEndTime = fifthRecordEndTime;
            rentalRecord5.TotalRentPrice = 2M;
            _rentalRecords.Add(rentalRecord5);

            var rentalRecord6 = new RentalRecord(6, "2", sixthRecordStartTime);
            rentalRecord6.RentEndTime = sixthRecordEndTime;
            rentalRecord6.TotalRentPrice = 40M;
            _rentalRecords.Add(rentalRecord6);

            var rentalRecord7 = new RentalRecord(7, "1", seventhRecordStartTime);
            rentalRecord7.RentEndTime = seventhRecordEndTime;
            rentalRecord7.TotalRentPrice = 4M;
            _rentalRecords.Add(rentalRecord7);

            var rentalRecord8 = new RentalRecord(8, "2", eighthRecordStartTime);
            rentalRecord8.RentEndTime = eighthRecordEndTime;
            rentalRecord8.TotalRentPrice = 60M;
            _rentalRecords.Add(rentalRecord8);

            var rentalRecord9 = new RentalRecord(9, "1", ninthRecordStartTime);
            _rentalRecords.Add(rentalRecord9);

            var rentalRecord10 = new RentalRecord(10, "2", tenthRecordStartTime);
            _rentalRecords.Add(rentalRecord10);

            var rentalRecord11 = new RentalRecord(11, "3", eleventhRecordStartTime);
            _rentalRecords.Add(rentalRecord11);
        }

    // ================
    // RENTAL PRICE CALCULATOR TESTS :
    // ================

        [TestMethod]
        public void CalculateRentalPrice_SameDaySameHourRentOf12Minutes_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 11, 11, 15, 0, 0);
            DateTime endTime = new DateTime(2023, 11, 11, 15, 12, 0);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 12M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_SameDayDifferentHourRentOf12Minutes_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 11, 11, 14, 56, 0);
            DateTime endTime = new DateTime(2023, 11, 11, 15, 8, 0);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 12M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_SameDayRentOf200Minutes_PriceShouldBeMaxPricePerDay()
        {
            DateTime startTime = new DateTime(2023, 11, 11, 14, 56, 0);
            DateTime endTime = new DateTime(2023, 11, 11, 18, 16, 0);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(DEFAULT_MAX_PRICE_PER_DAY);
        }

        [TestMethod]
        public void CalculateRentalPrice_SameDaySameHourSpanOf14MinutesWithOddSeconds_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 11, 11, 15, 0, 15);
            DateTime endTime = new DateTime(2023, 11, 11, 15, 14, 43);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 14M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_SameDayDifferentHourSpanOf14MinutesWithOddSeconds_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 11, 11, 14, 53, 15);
            DateTime endTime = new DateTime(2023, 11, 11, 15, 7, 56);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 14M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_DifferentDaysRentSpanOf4MinutesAcrossMidnight_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 11, 10, 23, 58, 00);
            DateTime endTime = new DateTime(2023, 11, 11, 00, 02, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentSpanOf4MinutesAcrossMidnightWithOddSeconds_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 11, 10, 23, 58, 13);
            DateTime endTime = new DateTime(2023, 11, 11, 00, 02, 36);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentSpanOf4MinutesAcrossMonthChange_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 10, 31, 23, 58, 15);
            DateTime endTime = new DateTime(2023, 11, 01, 00, 02, 48);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentSpanOf4MinutesAcrossYearChange_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2022, 12, 31, 23, 58, 15);
            DateTime endTime = new DateTime(2023, 01, 01, 00, 02, 48);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_SameMonthRentAccross3DaysWithTotalSpanOf24Hours4Minutes_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 11, 10, 23, 58, 00);
            DateTime endTime = new DateTime(2023, 11, 12, 00, 02, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY + 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_DifferentMonthRentAccross3DaysWithTotalSpanOf24Hours4Minutes_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 10, 30, 23, 58, 00);
            DateTime endTime = new DateTime(2023, 11, 01, 00, 02, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY + 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_DifferentYearRentAccross3DaysWithTotalSpanOf24Hours4Minutes_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2022, 12, 30, 23, 58, 00);
            DateTime endTime = new DateTime(2023, 01, 01, 00, 02, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY + 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentSpanningOneFullYear_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2022, 01, 01, 00, 00, 00);
            DateTime endTime = new DateTime(2023, 01, 01, 00, 00, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 365;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentSpanningOneFullLeapYear_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2020, 01, 01, 00, 00, 00);
            DateTime endTime = new DateTime(2021, 01, 01, 00, 00, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 366;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentSpanningTwoFullYears_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2021, 01, 01, 00, 00, 00);
            DateTime endTime = new DateTime(2023, 01, 01, 00, 00, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 365 * 2;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentSpanningThreeFullYears_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2020, 05, 05);
            DateTime endTime = new DateTime(2023, 05, 05);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 365 * 3;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentSpanningThreeFullYearsWithLeapYear_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2020, 01, 01, 00, 00, 00);
            DateTime endTime = new DateTime(2023, 01, 01, 00, 00, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 365 * 2 + (DEFAULT_MAX_PRICE_PER_DAY * 366);
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_SameYearRentSpanning364FullDays_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2022, 01, 01, 00, 00, 00);
            DateTime endTime = new DateTime(2022, 12, 31, 00, 00, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 364;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_SameLeapYearRentSpanning365FullDays_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2020, 01, 01);
            DateTime endTime = new DateTime(2020, 12, 31);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 365;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_AcrossYearsRentSpanning364FullDays_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2022, 05, 05);
            DateTime endTime = new DateTime(2023, 05, 04);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 364;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_AcrossLeapYearRentSpanning365FullDays_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2019, 05, 05);
            DateTime endTime = new DateTime(2020, 05, 04);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY * 365;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentAcrossEndOfFebruaryNonLeapYearSpan4Minutes_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2023, 02, 28, 23, 58, 00);
            DateTime endTime = new DateTime(2023, 03, 01, 00, 02, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CalculateRentalPrice_RentAcrossEndOfFebruaryLeapYearSpan24h4min_PriceShouldBeCorrect()
        {
            DateTime startTime = new DateTime(2020, 02, 28, 23, 58, 00);
            DateTime endTime = new DateTime(2020, 03, 01, 00, 02, 00);

            var rentalRecord = new RentalRecord(DEFAULT_RENTAL_RECORD_NUMBER, DEFAULT_SCOOTER_ID, startTime);
            rentalRecord.RentEndTime = endTime;

            decimal expectedResult = DEFAULT_MAX_PRICE_PER_DAY + 4M * DEFAULT_PRICE_PER_MINUTE;
            decimal price = _rentalPrice.CalculateRentalPrice(startTime, endTime);

            price.Should().Be(expectedResult);
        }
    }
}