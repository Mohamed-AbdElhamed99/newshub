using FluentValidation;
using NewsHub.Application.Admin.DTOs.Categories;

namespace NewsHub.Application.Admin.Validators.Categories;

public class CreateCategoryValidator: AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.ImagePath).NotEmpty();
        RuleForEach(x => x.Translations).SetValidator(new CategoryTranslationValidator());
    }
}