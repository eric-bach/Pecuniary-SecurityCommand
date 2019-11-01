using System;
using System.Threading;
using EricBach.CQRS.Commands;
using MediatR;
using Pecuniary.Security.Data.Requests;

namespace Pecuniary.Security.Data.Commands
{
    public class CreateSecurityCommand : Command, IRequest<CancellationToken>
    {
        public CreateSecurityRequest Security { get; set; }

        //public Guid TransactionId { get; set; }

        public CreateSecurityCommand(Guid id, CreateSecurityRequest security) : base(id)
        {
            Security = security;
        }
    }
}
