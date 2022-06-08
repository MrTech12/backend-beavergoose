namespace DeleteFileApp.Interfaces
{
    public interface IRetrieveConfigHelper
    {
        string GetConfigValue(string configurationSection, string configurationKey);
    }
}
