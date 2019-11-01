using System;
using EricBach.CQRS.Requests;

namespace Pecuniary.Security.Data.Requests
{
    public class CreateSecurityRequest : Request
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExchangeTypeCode { get; set; }

        public CreateSecurityRequest(Guid id) : base(id)
        {
        }
    }
}
