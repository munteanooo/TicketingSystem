namespace TicketingSystem.Application.Contracts.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string resource, object key)
            : base($"{resource} with id '{key}' was not found") { }

        public static NotFoundException Create(string resourceName, object id)
        {
            return new NotFoundException($"{resourceName} with id '{id}' was not found");
        }
    }
}