using System;
using EricBach.CQRS.Events;
using Pecuniary.Security.Data.Requests;

namespace Pecuniary.Security.Data.Events
{
    public class SecurityCreatedEvent : Event
    {
        private const int _eventVersion = 1;

        public CreateSecurityRequest Security { get; internal set; }

        public SecurityCreatedEvent() : base(nameof(SecurityCreatedEvent), _eventVersion)
        {
        }

        public SecurityCreatedEvent(Guid id, CreateSecurityRequest security) : base(nameof(SecurityCreatedEvent), _eventVersion)
        {
            Id = id;
            EventName = nameof(SecurityCreatedEvent);

            Security = security;
        }
    }
}
