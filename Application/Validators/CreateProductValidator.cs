using FluentValidation;
using SDTechnicalAssessment.Application.DTOs;

namespace SDTechnicalAssessment.Application.Validators
{
    // This validator checks whether the data received
    // while creating a Product is valid or not.
    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            // ----------------------------------------------------
            // ProductName validation
            //
            // Product name is required.
            // It cannot be empty.
            // Maximum length is 255 characters.
            // ----------------------------------------------------
            RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required.")

                .MaximumLength(255)
                .WithMessage("Product name cannot exceed 255 characters.");

            // ----------------------------------------------------
            // CreatedBy validation
            //
            // CreatedBy is required.
            // Maximum length is 100 characters.
            // ----------------------------------------------------
            RuleFor(x => x.CreatedBy)
                .NotEmpty()
                .WithMessage("CreatedBy is required.")

                .MaximumLength(100)
                .WithMessage("CreatedBy cannot exceed 100 characters.");
        }
    }
}