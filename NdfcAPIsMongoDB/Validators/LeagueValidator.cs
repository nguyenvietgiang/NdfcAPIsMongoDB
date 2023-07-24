using FluentValidation;
using NdfcAPIsMongoDB.Models.DTO;

namespace NdfcAPIsMongoDB.Validators
{
    public class LeagueValidator : AbstractValidator<LeagueDTO>
    {
        public LeagueValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Tên league không được để trống.");
            RuleFor(x => x.Reward).NotEmpty().WithMessage("Phần thưởng không được để trống.");
            RuleFor(x => x.Year).NotEmpty().WithMessage("Năm không được để trống.");
        }
    }
}
