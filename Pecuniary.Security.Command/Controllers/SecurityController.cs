using System;
using EricBach.CQRS.Commands;
using EricBach.LambdaLogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pecuniary.Security.Data.Commands;
using Pecuniary.Security.Data.Requests;

namespace Pecuniary.Security.Command.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SecurityController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        // POST api/security
        [HttpPost]
        public ActionResult<CommandResponse> Post([FromBody] CreateSecurityRequest request)
        {
            Logger.Log($"Received {nameof(CreateSecurityRequest)}");

            var id = Guid.NewGuid();
            
            try
            {
                _mediator.Send(new CreateSecurityCommand(id, request));
            }
            catch (Exception e)
            {
                return BadRequest(new CommandResponse { Error= e.Message });
            }

            Logger.Log($"Completed processing {nameof(CreateSecurityRequest)}");

            return Ok(new CommandResponse {Id = id, Name = nameof(CreateSecurityCommand) });
        }
    }
}
