using System;
using EricBach.CQRS.Aggregate;
using EricBach.CQRS.EventHandlers;
using Pecuniary.Security.Data.Events;
using Pecuniary.Security.Data.Requests;

namespace Pecuniary.Security.Data.Models
{
    public class Security : AggregateRoot, IEventHandler<SecurityCreatedEvent>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExchangeTypeCode { get; set; }

        public Security()
        {
        }

        public Security(Guid id, CreateSecurityRequest request)
        {
            ApplyChange(new SecurityCreatedEvent(id, request));
        }

        public void Handle(SecurityCreatedEvent e)
        {
            Id = e.Id;
            Name = e.Security.Name;
            Description = e.Security.Description;
            ExchangeTypeCode = e.Security.ExchangeTypeCode;

            Version = e.Version;
            EventVersion = e.EventVersion;
        }
    }
}
