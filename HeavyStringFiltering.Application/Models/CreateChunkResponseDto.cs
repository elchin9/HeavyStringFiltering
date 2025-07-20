using HeavyStringFiltering.Domain.Enums;
using System.Text.Json.Serialization;

namespace HeavyStringFiltering.Application.Models
{
    public class CreateChunkResponseDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChunkResponseEnum Status { get; set; }
    }
}
