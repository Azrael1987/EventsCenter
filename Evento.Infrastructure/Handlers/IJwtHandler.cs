using Evento.Infrastructure.DTO;
using System;

namespace Evento.Infrastructure.Handlers
{
    public interface IJwtHandler
    {
        JwtDto CreateToken(Guid userId, string role);
    }
}
