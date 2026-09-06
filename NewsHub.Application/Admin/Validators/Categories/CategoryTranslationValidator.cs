using FluentValidation;
using NewsHub.Application.Admin.DTOs.Categories;

namespace NewsHub.Application.Admin.Validators.Categories;

public class CategoryTranslationValidator : AbstractValidator<CategoryTranslationDto>
{
    public CategoryTranslationValidator()
    {
        RuleFor(x => x.LanguageCode).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Slug).NotEmpty();
    }
}