using FluentValidation;
using NattiChatBot.Contracts.Requests;

namespace NattiChatBot.Validation;

public class TokenValidator : AbstractValidator<TokenRequest>
{
    public TokenValidator()
    {
        RuleFor(x => x.AccessType).NotNull().IsInEnum();
        RuleFor(x => x.ExpiresAt).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.GrantedTo).NotNull().NotEmpty().Length(1, 64);
    }
}