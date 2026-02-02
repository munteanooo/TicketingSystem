namespace TicketingSystem.Application.Contracts.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }

        public static ForbiddenException Create(string action, string resource)
        {
            return new ForbiddenException($"You do not have permission to {action} this {resource}");
        }

        public static ForbiddenException InvalidOwner(string resource)
        {
            return new ForbiddenException($"You are not the owner of this {resource}");
        }
    }
}