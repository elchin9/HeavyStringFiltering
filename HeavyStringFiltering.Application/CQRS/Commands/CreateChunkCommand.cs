using HeavyStringFiltering.Application.Models;
using MediatR;

namespace HeavyStringFiltering.Application.CQRS.Commands
{
    public class CreateChunkCommand : IRequest<CreateChunkResponseDto>
    {
        public string UploadId { get; set; }
        public int ChunkIndex { get; set; }
        public string Data { get; set; }
        public bool IsLastChunk { get; set; }
    }
}
