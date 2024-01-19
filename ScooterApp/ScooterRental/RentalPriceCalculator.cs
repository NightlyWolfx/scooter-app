using ScooterRental.Exceptions;

namespace ScooterRental
{
    public class RentalPriceCalculator
    {
        private readonly decimal _pricePerMinute;
        private readonly decimal _maxPricePerDay;

        public RentalPriceCalculator(decimal pricePerMinute, decimal maxPrice)
        {
            _pricePerMinute = pricePerMinute;
            _maxPricePerDay = maxPrice;
        }

        public decimal CalculateRentalPrice(DateTime rentStart, DateTime rentEnd)
        {
            decimal rentalPrice;
            var yearDifference = rentEnd.Year - rentStart.Year;

            if (yearDifference == 0) 
            {
                rentalPrice = CalculateSameYearRentalPrice(rentStart, rentEnd);
            }
            else if (yearDifference >= 1)
            {
                rentalPrice = CalculateMultipleYearRentalPrice(rentStart, rentEnd);
            }
            else
            {
                throw new IncorrectDatesException();
            }

            return rentalPrice;
        }

        private decimal CalculateSameYearRentalPrice(DateTime rentStart, DateTime rentEnd)
        {
            var monthDifference = rentEnd.Month - rentStart.Month;

            if (monthDifference == 0)
            {
                return CalculateSingleMonth(rentStart, rentEnd);
            }
            else if (monthDifference >= 1)
            {
                return CalculateMultipleMonths(rentStart, rentEnd);
            }
            else
            {
                throw new IncorrectDatesException();
            }
        }

        private decimal CalculateMultipleYearRentalPrice(DateTime rentStart, DateTime rentEnd)
        {
            DateTime endOfFirstYear = new DateTime(rentStart.Year, 12, 31, 23, 59, 59);
            DateTime startOfLastYear = new DateTime(rentEnd.Year, 01, 01, 00, 00, 00);

            decimal startYearPrice = CalculateSameYearRentalPrice(rentStart, endOfFirstYear);
            decimal elapsedYearsPrice = CalculateElapsedYearsPrice(rentStart, rentEnd);
            decimal endYearPrice = CalculateSameYearRentalPrice(startOfLastYear, rentEnd);

            decimal totalPrice = startYearPrice + elapsedYearsPrice + endYearPrice;
            return totalPrice;
        }

        private decimal CalculateElapsedYearsPrice(DateTime rentStart, DateTime rentEnd)
        {
            decimal totalPrice = 0;

            for (int i = rentStart.Year + 1; i < rentEnd.Year; i++)
            {
                decimal daysInYear = DateTime.IsLeapYear(i) ? 366 : 365;

                totalPrice += daysInYear * _maxPricePerDay;
            }

            return totalPrice;
        }

        private decimal CalculateSingleMonth(DateTime rentStart, DateTime rentEnd)
        {
            var dayDifference = rentEnd.Day - rentStart.Day;

            if (dayDifference == 0)
            {
                return CalculateSingleDay(rentStart, rentEnd);
            }
            else if (dayDifference >= 1)
            {
                return CalculateMultipleDays(rentStart, rentEnd);
            }
            else
            {
                throw new IncorrectDatesException();
            }
        }

        private decimal CalculateMultipleMonths(DateTime rentStart, DateTime rentEnd)
        {
            var monthDifference = rentEnd.Month - rentStart.Month;
            int lastDayOfMonth = DateTime.DaysInMonth(rentStart.Year, rentStart.Month);
            DateTime endOfFirstMonth = new DateTime(rentStart.Year, rentStart.Month, lastDayOfMonth, 23, 59, 59);
            DateTime startOfLastMonth = new DateTime(rentEnd.Year, rentEnd.Month, 1);

            decimal startMonthPrice = CalculateSingleMonth(rentStart, endOfFirstMonth);
            decimal elapsedMonthsPrice = CalculateElapsedMonths(rentStart, rentEnd);
            decimal endMonthPrice = CalculateSingleMonth(startOfLastMonth, rentEnd);

            decimal totalPrice = startMonthPrice + elapsedMonthsPrice + endMonthPrice;
            return totalPrice;
        }

        private decimal CalculateElapsedMonths(DateTime rentStart, DateTime rentEnd)
        {
            decimal totalPrice = 0;

            for (int i = rentStart.Month + 1; i < rentEnd.Month; i++)
            {
                decimal daysInMonth = DateTime.DaysInMonth(rentStart.Year, i);

                totalPrice += daysInMonth * _maxPricePerDay;
            }

            return totalPrice;
        }

        private decimal CalculateMultipleDays(DateTime rentStart, DateTime rentEnd)
        {
            var dayDifference = rentEnd.Day - rentStart.Day;
            DateTime endOfFirstDay = new DateTime(rentStart.Year, rentStart.Month, rentStart.Day, 23, 59, 59);
            DateTime startOfLastDay = new DateTime(rentEnd.Year, rentEnd.Month, rentEnd.Day);

            decimal firstDayPrice = CalculateSingleDay(rentStart, endOfFirstDay);
            decimal fullDaysPrice = (dayDifference - 1) * _maxPricePerDay;
            decimal lastDayPrice = CalculateSingleDay(startOfLastDay, rentEnd);

            decimal totalPrice = firstDayPrice + fullDaysPrice + lastDayPrice;
            return totalPrice;
        }

        private decimal CalculateSingleDay(DateTime rentStart, DateTime rentEnd)
        {
            int minutes = (rentEnd.Hour * 60 + rentEnd.Minute)
                - (rentStart.Hour * 60 + rentStart.Minute);

            if (rentEnd.Hour == 23 && rentEnd.Minute == 59 && rentEnd.Second == 59)
            {
                minutes += 1;
            }

            decimal price = minutes * _pricePerMinute;
            return Math.Min(price, _maxPricePerDay);
        }

        public decimal CalculateIncompleteRentalPrice(DateTime rentStart)
        {
            DateTime rentEnd = DateTime.Now;
            decimal price = CalculateRentalPrice(rentStart, rentEnd);

            return price;
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals, List<RentalRecord> _rentalRecordsList)
        {
            decimal income = 0;

            if (year.HasValue)
            {
                var completedRentalsInSpecifiedYear = _rentalRecordsList.Where(
                    s => s.RentEndTime.HasValue && s.RentEndTime.Value.Year == year).ToList();

                foreach (var rental in completedRentalsInSpecifiedYear)
                {
                    income += rental.TotalRentPrice.Value;
                }
            }
            else
            {
                var completedRentals = _rentalRecordsList.Where(s => s.RentEndTime.HasValue);

                foreach (var rental in completedRentals)
                {
                    income += rental.TotalRentPrice.Value;
                }
            }

            if (includeNotCompletedRentals)
            {
                var incompleteRentals = _rentalRecordsList.Where(s => !s.RentEndTime.HasValue);

                foreach (var rental in incompleteRentals)
                {
                    income += CalculateIncompleteRentalPrice(rental.RentStartTime);
                }
            }

            return income;
        }
    }
}