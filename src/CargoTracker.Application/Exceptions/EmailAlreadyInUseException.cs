namespace CargoTracker.Application.Exceptions;

public class EmailAlreadyInUseException : Exception
{
    public EmailAlreadyInUseException(string email) : base($"E-posta adresi '{email}' zaten kullanılıyor.")
    {
    }
}
