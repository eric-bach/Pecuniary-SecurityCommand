using System;
using System.Threading;
using System.Threading.Tasks;
using EricBach.CQRS.EventRepository;
using EricBach.LambdaLogger;
using MediatR;
using Pecuniary.Security.Data.Commands;
using _Security = Pecuniary.Security.Data.Models.Security;

namespace Pecuniary.Security.Command.CommandHandlers
{
    public class AccountCommandHandlers : IRequestHandler<CreateSecurityCommand, CancellationToken>
    {
        private readonly IEventRepository<_Security> _repository;

        public AccountCommandHandlers(IEventRepository<_Security> repository)
        {
            _repository = repository ?? throw new InvalidOperationException("Repository is not initialized.");
        }

        public Task<CancellationToken> Handle(CreateSecurityCommand command, CancellationToken cancellationToken)
        {
            Logger.Log($"{nameof(CreateSecurityCommand)} handler invoked");

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (string.IsNullOrEmpty(command.Security.Name))
                throw new Exception($"{nameof(command.Security.Name)} is required");
            if (string.IsNullOrEmpty(command.Security.Description))
                throw new Exception($"{nameof(command.Security.Description)} is required");
            if (string.IsNullOrEmpty(command.Security.ExchangeTypeCode) || (command.Security.ExchangeTypeCode != "TSX" && command.Security.ExchangeTypeCode != "NYSE" && command.Security.ExchangeTypeCode != "NASDAQ"))
                throw new Exception($"{nameof(command.Security.ExchangeTypeCode)} is invalid.  Must be one of [TSX, NYSE, NASDAQ]");

            var aggregate = new _Security(command.Id, command.Security);

            // Save to Event Store
            _repository.Save(aggregate, aggregate.Version);

            // TODO Now CreateTransactionRequest
            //// Issue a CreateTransactionCommand to create the Transaction
            //Logger.Log($"Creating Transaction {command.TransactionId}");
            //var transaction = command.Transaction;
            //// Update the Security Id now that a Security has been created
            //transaction.Security.SecurityId = command.Id;
            //_mediator.Send(new CreateTransactionCommand(command.TransactionId, transaction), CancellationToken.None);

            Logger.Log($"Completed saving {nameof(_Security)} aggregate to event store");

            return Task.FromResult(cancellationToken);
        }
    }
}
