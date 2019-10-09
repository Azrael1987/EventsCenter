using System;

namespace Evento.Infrastructure.DTO
{
    public class AccountDto
    {
        public Guid Id { get ;set;}
        public string Role { get;  set; }
        public string Name { get;  set; }
        public string Email { get;  set; }
        public DateTime CreatesAt { get;  set; }
        public string Password { get;  set; }
    }
}
