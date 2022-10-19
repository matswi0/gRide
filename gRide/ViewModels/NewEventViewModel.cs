using FluentValidation;
using FluentValidation.Results;
using System;

namespace gRide.ViewModels
{
    public class NewEventViewModel
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Town { get; set; }
    }
    public class NewEventValidator : AbstractValidator<NewEventViewModel>
    {
        public NewEventValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty()
                .MaximumLength(20);
            RuleFor(e => e.Longitude)
                .NotEmpty();
            RuleFor(e => e.Latitude)
                .NotEmpty();
        }
        protected override bool PreValidate(ValidationContext<NewEventViewModel> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(string.Empty, "Fill all fields"));
                return false;
            }
            return true;
        }
    }
}