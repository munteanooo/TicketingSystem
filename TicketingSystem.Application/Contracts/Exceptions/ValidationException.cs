namespace TicketingSystem.Application.Contracts.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }

        public static ValidationException BusinessRule(string message)
        {
            return new ValidationException(message);
        }
    }
}