using System.Diagnostics.CodeAnalysis;
using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.Domain
{
    public class User : ConcurrencyAggregateRoot
    {
        [ExcludeFromCodeCoverage]
        private User()
        {
        }

        internal User(
            string firstName,
            string lastName,
            string email,
            string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public void AddCreatedEvent()
        {
            if (Id > 0)
            {
                AddEvent(new UserCreatedEvent { UserId = Id });
            }
        }
    }
}
