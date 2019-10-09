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

        public AccountController(IUserService userService)
        {
            _userService = userService;
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

        [HttpGet("tickets")]
        public async Task<IActionResult> GetTickets()
        {
            throw new NotImplementedException();
        }

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
