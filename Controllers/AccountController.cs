using Evento.Core.Domain;
using Evento.Infrastructure.Commands.Users;
using Evento.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Evento.Api.Controllers
{
    [Route("[controller]")]
    public class AccountController : ApiControllerBase
    {
        private IUserService _userService;
        private ITicketService _ticketService;

        public AccountController(IUserService userService, ITicketService ticketService)
        {
            _userService = userService;
            _ticketService = ticketService;
        }

        /// <summary>
        /// 
        /// przykład:
        ///     GET Account
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet]
        // [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> Get() => Json(await _userService.GetAccountAsync(UserId));

        /// <summary>
        /// 
        /// przykład:
        ///     GET Account/tickets
        /// </summary>
        /// <returns> Kolekcja biletów dla danego użytkowanika </returns>
        [HttpGet("tickets")]
        [Authorize]
        public async Task<IActionResult> GetTickets()
            => Json(await _ticketService.GetTicktesForUserAsync(UserId));

        /// <summary>
        /// 
        /// przykład:
        ///     POST Account/registry?email=logan@wp.pl&password=secret12345!@#
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("registry")]
        public async Task<IActionResult> Post([FromBody]Register command)
        {
            await _userService.RegistryAsync(Guid.NewGuid(), command.Email, command.Name, command.Password, command.Role);
            return Created("/account", null);
        }

        /// <summary>
        /// 
        /// przykład:
        ///     POST Account/login?email=logan@wp.pl&password=secret12345!@#
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]Login command)
            => Json(await _userService.LoginAsync(command.Email, command.Password));
    }
}
