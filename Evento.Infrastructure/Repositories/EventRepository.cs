using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evento.Core.Domain;
using Evento.Core.Repositories;

namespace Evento.Ifrastructure.Repositories {

    public class EventRepository : IEventRepository {
        private static readonly ISet<Event> _events = new HashSet<Event>{
            new Event(Guid.NewGuid(), "Koncert U2", "wystep zespo³u U2", DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddDays(1).AddHours(5)),
            new Event(Guid.NewGuid(), "Koncert IRA", "wystep zespo³u IRA", DateTime.UtcNow.AddDays(7).AddHours(1), DateTime.UtcNow.AddDays(8).AddHours(3))

            };

        public async Task<Event> GetAsync (Guid id) => await Task.FromResult (_events.SingleOrDefault (x => x.Id == id));

        public async Task<Event> GetAsync (string name) => await Task.FromResult (_events.SingleOrDefault (x => x.Name.ToLowerInvariant () == name.ToLowerInvariant ()));

        public async Task<IEnumerable<Event>> BrowseAsync (string name = "") {
            var events = _events.AsEnumerable ();
            if(!string.IsNullOrWhiteSpace(name))
            {
                events = events.Where(x => x.Name.ToLowerInvariant()
                               .Contains(name.ToLowerInvariant()));
            }
            return await Task.FromResult(events);
        }

        public async Task AddAsync (Event @event) {
            _events.Add(@event);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync (Event @event) {
            _events.Remove(@event);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync (Event @event) {
            await Task.CompletedTask;
            //tu bedzie kiedys zapytanie SQL
        }
    }
}