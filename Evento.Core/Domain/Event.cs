using System;
using System.Collections.Generic;
using System.Linq;

namespace Evento.Core.Domain {
    public class Event : Entity {
        private ISet<Ticket> _tickets = new HashSet<Ticket> ();
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreateAt { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        public DateTime UpdateAt { get; protected set; }
        public IEnumerable<Ticket> Tickets => _tickets;
        /// <summary> kolekcja sprzedanych biletów </summary>
        public IEnumerable<Ticket> PurchasedTickets => Tickets.Where(x => x.IsPurchased);
        /// <summary> kolekcja dostêpnych biletów </summary>
        public IEnumerable<Ticket> AvailableTickets => Tickets.Except(PurchasedTickets);
       // public IEnumerable<Ticket> AvailableTickets => Tickets.Where(x => !x.IsPurchased);

        protected Event(){

        }

        public Event(Guid id, string name, string description, DateTime startDate, DateTime endDate){
            Id = id;
            SetName(name);
            SetDescription(description);
            Description = description;
            CreateAt = DateTime.UtcNow;
            StartDate = startDate;
            EndDate = endDate;
            UpdateAt = CreateAt;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"Event with id: '{Id}' can not have an empty name or white space.");
            }
            Name = name;
            UpdateAt = DateTime.UtcNow;
        }

        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new Exception($"Event with id: '{Id}' can not have an empty description or white space.");
            }
            Description = description;
            UpdateAt = DateTime.UtcNow;
        }

        public void AddTickets (int amount, decimal price) {
            int emptySeating = _tickets.Count + 1;
            for (int i = 0; i < amount; i++) {
                _tickets.Add (new Ticket (this, emptySeating, price));
                emptySeating++;
            }
        }
    }
}