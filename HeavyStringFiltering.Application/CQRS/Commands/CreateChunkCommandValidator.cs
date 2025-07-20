using FluentValidation;

namespace HeavyStringFiltering.Application.CQRS.Commands
{
    public class CreateChunkCommandValidator : AbstractValidator<CreateChunkCommand>
    {
        public CreateChunkCommandValidator() : base()
        {
            RuleFor(command => command.UploadId)
                .NotEmpty()
                .MaximumLength(100)
                .Matches("^[a-zA-Z0-9_-]+$")
                .WithMessage("UploadId only supports alphanumeric, dash and underscore");

            RuleFor(command => command.ChunkIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ChunkIndex must be non-negative");

            RuleFor(command => command.Data)
                .NotEmpty();

            RuleFor(command => command.IsLastChunk)
                .NotNull();
        }
    }
}
