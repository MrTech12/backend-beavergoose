namespace Common.Configuration.Interfaces
{
    public interface IConfigHelper
    {
        string GetConfigValue(string section, string key);
    }
}
