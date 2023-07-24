using FluentValidation;
using NdfcAPIsMongoDB.Models.DTO;

namespace NdfcAPIsMongoDB.Validators
{
    public class PlayerValidator : AbstractValidator<PlayerDto>
    {
        public PlayerValidator()
        {
            RuleFor(x => x.sName).NotEmpty().WithMessage("Tên không được để trống.");
            RuleFor(x => x.iAge).NotEmpty().WithMessage("Tuổi không được để trống.");
            RuleFor(x => x.iAge).GreaterThan(18).WithMessage("Tuổi phải lớn hơn 18.");
            RuleFor(x => x.sRole).NotEmpty().WithMessage("Vai trò không được để trống.");
            RuleFor(x => x.sPosition).NotEmpty().WithMessage("Vị trí không được để trống.");
            RuleFor(x => x.Image).NotEmpty().WithMessage("Tệp ảnh không được để trống.");
        }
    }
}
