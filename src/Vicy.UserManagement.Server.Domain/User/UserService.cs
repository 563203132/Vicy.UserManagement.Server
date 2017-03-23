using System;

namespace Vicy.UserManagement.Server.Domain
{
    public class UserService : IUserService
    {
        private readonly IUserFactory _userFactory;
        private readonly IUserRepository _userRepository;

        public UserService(
            IUserFactory userFactory,
            IUserRepository userRepository)
        {
            _userFactory = userFactory;
            _userRepository = userRepository;
        }

        public User Create(string firstName, string lastName, string email, string phoneNumber)
        {
            var user = _userFactory.Create(firstName, lastName, email, phoneNumber);

            _userRepository.InsertAndSave(user);

            user.AddCreatedEvent();

            return user;
        }

        public User Edit(long Id, string firstName, string lastName, string email, string phoneNumber)
        {
            var user = _userRepository.GetById(Id);
            if (user != null)
            {
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.PhoneNumber = phoneNumber;
            }

            return user;
        }
    }
}
