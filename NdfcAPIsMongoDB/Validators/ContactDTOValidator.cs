using FluentValidation;
using NdfcAPIsMongoDB.Models.DTO;

public class ContactDTOValidator : AbstractValidator<ContactDTO>
{
    public ContactDTOValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Tên không được để trống.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email không được để trống.");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email không hợp lệ.");
        RuleFor(x => x.Topic).NotEmpty().WithMessage("Chủ đề không được để trống.");
        RuleFor(x => x.Detail).NotEmpty().WithMessage("Chi tiết không được để trống.");
    }
}
