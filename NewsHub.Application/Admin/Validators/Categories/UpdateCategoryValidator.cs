using FluentValidation;
using NewsHub.Application.Admin.DTOs.Categories;

namespace NewsHub.Application.Admin.Validators.Categories;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ImagePath).NotEmpty();
        RuleForEach(x => x.Translations).SetValidator(new CategoryTranslationValidator());
    }
}