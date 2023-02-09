using Common.Configuration.Helpers;
using FileMicroservice.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FileMicroservice.Helpers
{
    public class DeleteFileHelper : IDeleteFileHelper
    {
        private readonly ILogger _logger;
        private readonly HttpClient client = new HttpClient();

        public DeleteFileHelper(ILogger<DeleteFileHelper> logger)
        {
            this._logger = logger;
        }
        public async Task DeleteFile(string fileName, string token)
        {
            string url = LocalConfigHelper.GetConfigValue("DeleteFile", "Endpoint");
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
