namespace Itminus.Azure.Abstraction
{
    public interface ITokenAccessor
    {
        string Token {get;}
        int RefreshTokenDuration {get;} 
    }
}