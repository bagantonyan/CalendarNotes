using CalendarNotes.API.Models.Notes;
using FluentValidation;

namespace CalendarNotes.API.ModelValidators.Notes
{
    public class UpdateNoteRequestModelValidator : AbstractValidator<UpdateNoteRequestModel>
    {
        public UpdateNoteRequestModelValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0);

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