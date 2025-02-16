using CalendarNotes.API.Models.Notes;
using FluentValidation;

namespace CalendarNotes.API.ModelValidators.Notes
{
    public class CreateNoteRequestModelValidator : AbstractValidator<CreateNoteRequestModel>
    {
        public CreateNoteRequestModelValidator()
        {
            RuleFor(p => p.Title)
                .NotEmpty()
                .NotNull()
                .MaximumLength(256);

            RuleFor(p => p.Text)
                .NotEmpty()
                .NotNull()
                .MaximumLength(10000);

            RuleFor(p => p.NotificationTime)
                .NotEmpty()
                .NotNull();
        }
    }
}