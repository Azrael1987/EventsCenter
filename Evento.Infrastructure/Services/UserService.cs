using AutoMapper;
using Evento.Core.Domain;
using Evento.Core.Repository;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Extensions;
using Evento.Infrastructure.Handlers;
using System;
using System.Threading.Tasks;

namespace Evento.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHandler _jwtHandler;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IJwtHandler jwtHandler, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtHandler = jwtHandler;
            _mapper = mapper;
        }

        public async Task<AccountDto> GetAccountAsync(Guid userId)
        {
            var user = await _userRepository.GetOrFailAsync(userId);

              var _user =  _mapper.Map<AccountDto>(user);
            _user.Password = null;
            return _user;
        }

        public async Task RegistryAsync(Guid id, string email, string name, string password, string role = "user")
        {
            var user = await _userRepository.GetAsync(email);
            if (user != null)
            {
                throw new Exception($"User with email: '{email}'  already exists.");
            }
            user = new User(id, role, name, email, password);
            await _userRepository.AddAsync(user);
        }
        public async Task<TokenDto> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetAsync(email);
            if (user == null)
            {
                throw new Exception($"Invalid credentials.");
            }
            else if (user.Password != password)
            {
                throw new Exception($"Invalid credentials.");
            }

            var jwt = _jwtHandler.CreateToken(user.Id, user.Role);

            return new TokenDto
            {
                Token = jwt.Token,
                Role = user.Role,
                Expires = jwt.Expires
            };
        }
    }
}
