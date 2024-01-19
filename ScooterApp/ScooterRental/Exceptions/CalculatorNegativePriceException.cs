namespace ScooterRental.Exceptions
{
    public class CalculatorNegativePriceException : Exception
    {
        public CalculatorNegativePriceException() : base("Calculator error: Found negative price")
        {
        }
    }
}