using FluentValidation;
using NdfcAPIsMongoDB.Models.DTO;

public class NewsDTOValidator : AbstractValidator<NewsDTO>
{
    public NewsDTOValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Tiêu đề không được để trống.");
        RuleFor(x => x.Image).NotEmpty().WithMessage("Tệp ảnh không được để trống.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Mô tả không được để trống.");
        RuleFor(x => x.Detail).NotEmpty().WithMessage("Chi tiết không được để trống.");
    }
}

