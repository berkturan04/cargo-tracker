namespace CargoTracker.Application.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("E-posta veya şifre hatalı.")
    {
    }
}
