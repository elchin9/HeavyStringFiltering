using HeavyStringFiltering.Application.Interfaces;
using HeavyStringFiltering.Application.Models;
using HeavyStringFiltering.Domain.Enums;
using MediatR;
using System.Collections.Concurrent;
using System.Text;

namespace HeavyStringFiltering.Application.CQRS.Commands
{
    public class CreateChunkCommandHandler : IRequestHandler<CreateChunkCommand, CreateChunkResponseDto>
    {
        private readonly ITextQueueService _queueService;
        private static readonly ConcurrentDictionary<string, SortedDictionary<int, string>> _chunks = new();

        public CreateChunkCommandHandler(ITextQueueService queueService)
        {
            _queueService = queueService;
        }

        public Task<CreateChunkResponseDto> Handle(CreateChunkCommand command, CancellationToken cancellationToken)
        {
            var dict = _chunks.GetOrAdd(command.UploadId, _ => new SortedDictionary<int, string>());
            dict[command.ChunkIndex] = command.Data;

            if (command.IsLastChunk)
            {
                var sb = new StringBuilder();
                foreach (var chunk in dict)
                {
                    sb.Append(chunk.Value);
                    sb.Append(' ');
                }

                string fullText = sb.ToString().Trim();
                _queueService.Enqueue(fullText);
                _chunks.TryRemove(command.UploadId, out _);
            }

            return Task.FromResult(new CreateChunkResponseDto
            {
                Status = ChunkResponseEnum.Accepted
            });
        }
    }
}
