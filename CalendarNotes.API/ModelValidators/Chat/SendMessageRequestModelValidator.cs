using CalendarNotes.API.Models.Chat;
using FluentValidation;

namespace CalendarNotes.API.ModelValidators.Chat
{
    public class SendMessageRequestModelValidator : AbstractValidator<SendMessageRequestModel>
    {
        public SendMessageRequestModelValidator()
        {
            RuleFor(p => p.ChatRoomId)
                .GreaterThan(0)
                .WithMessage("ID комнаты должен быть положительным числом");

            RuleFor(p => p.Content)
                .NotEmpty()
                .NotNull()
                .WithMessage("Содержимое сообщения не может быть пустым")
                .MaximumLength(5000)
                .WithMessage("Сообщение не может быть длиннее 5000 символов");
        }
    }
}

