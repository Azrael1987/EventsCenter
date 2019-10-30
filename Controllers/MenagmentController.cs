using Evento.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Evento.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenagmentController : ApiControllerBase
    {
        private readonly IMenagmentService _menagmentService;

        public MenagmentController(IMenagmentService menagmentService)
        {
            _menagmentService = menagmentService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpPost("SetLoggLevel/{level}")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<IActionResult> SetLoggLevel(int level)
        {
            if (level < 0 || level > 5)
            {
                throw new Exception($"Wrong level to set");
            }
            await _menagmentService.SetLoggLevel(level);
            return Ok(); // 200
        }
    }
}