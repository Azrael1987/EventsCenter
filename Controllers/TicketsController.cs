using Evento.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Evento.Api.Controllers
{
    [Route("events/{eventId}/[controller]")]
    [Authorize]
    // [ApiController]
    public class TicketsController : ApiControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        //pobierz bilet
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> Get(Guid eventId, Guid ticketId)
        {
            var ticket = await _ticketService.GetAsync(UserId, eventId, ticketId);
            if (ticket == null)
            {
                return NotFound();
            }
            return Json(ticket);
        }

        //// GET: api/Ticket
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Ticket/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/Ticket

        // kup bilety
        [HttpPost("purchase/{amount}")]
        public async Task<IActionResult> Post(Guid eventId, int amount)
        {
            await _ticketService.PurchaseTicketsAsync(UserId, eventId, amount);
            if (eventId == null)
            {
                return NotFound();
            }
            return NoContent(); // 204
        }


        //// PUT: api/Ticket/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE: api/ApiWithActions/5
        // anuluj bilet
        [HttpDelete("cancel/{amount}")]
        public async Task<IActionResult> Delete(Guid eventId, int amount)
        {
            /*var event = */
            await _ticketService.CancelTicketAsync(UserId, eventId, amount);
            return NoContent();
        }
    }
}
