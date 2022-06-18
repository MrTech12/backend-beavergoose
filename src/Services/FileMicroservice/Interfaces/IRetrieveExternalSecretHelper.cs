namespace FileMicroservice.Interfaces
{
    public interface IRetrieveExternalSecretHelper
    {
        string GetSecret(string secretName);
    }
}
