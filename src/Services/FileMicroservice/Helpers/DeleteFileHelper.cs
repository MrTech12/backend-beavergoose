using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FileMicroservice.Helpers
{
    public class DeleteFileHelper : IDeleteFileHelper
    {
        private readonly IRetrieveConfigHelper _retrieveConfigHelper;
        private readonly ILogger _logger;
        private readonly HttpClient client = new HttpClient();

        public DeleteFileHelper(IRetrieveConfigHelper retrieveConfigHelper, ILogger<DeleteFileHelper> logger)
        {
            this._retrieveConfigHelper = retrieveConfigHelper;
            this._logger = logger;
        }
        public async Task DeleteFile(string fileName, string token)
        {
            string url = this._retrieveConfigHelper.GetConfigValue("DeleteFile", "Url");
            string uri = url + fileName;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
