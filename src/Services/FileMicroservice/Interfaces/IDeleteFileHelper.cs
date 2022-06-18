namespace FileMicroservice.Interfaces
{
    public interface IDeleteFileHelper
    {
        Task DeleteFile(string fileName, string token);
    }
}
