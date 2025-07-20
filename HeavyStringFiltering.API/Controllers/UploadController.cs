using HeavyStringFiltering.Application.CQRS.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HeavyStringFiltering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload([FromBody] CreateChunkCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}
