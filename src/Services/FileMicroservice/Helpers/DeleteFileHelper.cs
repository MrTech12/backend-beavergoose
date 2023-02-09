using Common.Configuration.Helpers;
using Common.Configuration.Interfaces;
using FileMicroservice.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FileMicroservice.Helpers
{
    public class DeleteFileHelper : IDeleteFileHelper
    {
        private readonly ILogger _logger;
        private readonly HttpClient client = new HttpClient();
        private readonly IConfigHelper _configHelper;

        public DeleteFileHelper(ILogger<DeleteFileHelper> logger, IConfigHelper configHelper)
        {
            this._logger = logger;
            this._configHelper = configHelper;
        }
        public async Task DeleteFile(string fileName, string token)
        {
            string url = this._configHelper.GetConfigValue("DeleteFile", "Endpoint");
            string uri = url + fileName;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            this._logger.LogInformation("Sending request for file deletion to external application");
            var deleteTask = client.DeleteAsync(uri);

            var response = await deleteTask;

            if (!response.IsSuccessStatusCode)
            {
                var messages = response.Content.ReadAsStringAsync();
                this._logger.LogError("There was a problem deleting the file. Message: {Message}", messages);
            }
        }
    }
}
