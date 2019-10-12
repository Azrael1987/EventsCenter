using Evento.Infrastructure.Commands.Events;
using Evento.Infrastructure.Services;
//using Evento.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Evento.Api.Controllers
{
    [Route("[controller]")]
    public class EventsController : ApiControllerBase
    {
        private readonly IEventService _eventService;
        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }
        /// <summary>
        /// 
        /// przykład:
        /// Get events
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            var events = await _eventService.BrowseAsync(name);
            return Json(events);
        }

        /// <summary>
        /// 
        /// przykład:
        /// Get events/8c7fccff-0b70-4ee2-856d-48a978b721e9
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns> Szczegóły wydarzenia </returns>
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetDetails(Guid eventId)
        {
            var @event = await _eventService.GetAsync(eventId);
            if (@event == null)
            {
                return NotFound(); // 404
            }
            return Json(@event);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateEvent command)
        {
            command.EventId = Guid.NewGuid();
            await _eventService.CreateAsync(command.EventId, command.Name, command.Description, command.StartDate, command.EndDate);
            await _eventService.AddTicketsAsync(command.EventId, command.Tickets, command.Price);
            return Created($"/events/{command.EventId}", null); // 201
        }

        /// <summary>
        ///  /events/{id} -> HTTP PUT
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{eventId}")]
        public async Task<IActionResult> Put(Guid eventId, [FromBody]UpdateEvent command)
        {
            await _eventService.UpdateAsync(eventId, command.Name, command.Description);
            return NoContent(); // 204
        }

        /// <summary>
        ///  /events/{id}
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            await _eventService.DeleteAsync(eventId);
            return NoContent();
        }
        //filmik z rodziału 5.11) zabezpieczneie dostepu z policy

        // narzedzia do testowania URL - fiddler, postman, curl, SoupUi

        // localhost:5001/events -X POST -H "Content-Type: application/json" -d '{"name":"moje wydarzenie","description":"opis wydarzenia","startDate":"2019-12-05","endDate":"2019-12-05","tickets":10,"price":100}'
    }
}