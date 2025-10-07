using CalendarNotes.API.Models.Chat;
using FluentValidation;

namespace CalendarNotes.API.ModelValidators.Chat
{
    public class CreateChatRoomRequestModelValidator : AbstractValidator<CreateChatRoomRequestModel>
    {
        public CreateChatRoomRequestModelValidator()
        {
            RuleFor(p => p.Name)
                .MaximumLength(256)
                .When(p => !string.IsNullOrEmpty(p.Name));

            RuleFor(p => p.ParticipantIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("Список участников не может быть пустым");

            RuleFor(p => p.ParticipantIds)
                .Must(ids => ids.Count >= 2)
                .When(p => p.IsGroupChat)
                .WithMessage("Групповой чат должен содержать минимум 2 участников");
        }
    }
}

