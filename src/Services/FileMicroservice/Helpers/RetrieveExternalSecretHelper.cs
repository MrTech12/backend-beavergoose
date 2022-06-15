using FileMicroservice.Interfaces;
using Google.Cloud.SecretManager.V1;

namespace FileMicroservice.Helpers
{
    public class RetrieveExternalSecretHelper : IRetrieveExternalSecretHelper
    {
        private readonly IRetrieveConfigHelper _retrieveConfigHelper;
        private readonly ILogger _logger;

        public RetrieveExternalSecretHelper(IRetrieveConfigHelper retrieveConfigHelper, ILogger<RetrieveExternalSecretHelper> logger)
        {
            this._retrieveConfigHelper = retrieveConfigHelper;
            this._logger = logger;
        }

        public string GetSecret(string secretName)
        {
            SecretManagerServiceClient secretManager = SecretManagerServiceClient.Create();

            string projectId = this._retrieveConfigHelper.GetConfigValue("GCP", "ProjectId");
            SecretVersionName secretVersionName = new SecretVersionName(projectId, secretName, "1");

            this._logger.LogInformation("Accessing secret from external secret manager");
            AccessSecretVersionResponse result = secretManager.AccessSecretVersion(secretVersionName);

            return result.Payload.Data.ToStringUtf8();
        }
    }
}
