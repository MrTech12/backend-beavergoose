# FileMicroservice

## Overview
* Microservice projects for the File application, that is part of the backend-beavergoose project.
* The application saves and retrieves user uploaded files.

## Setup
* The project uses the 'FileMicroservice' profile, configured in the `Properties` --> `launchSettings.json` file.
* The service runs on port `5200` for HTTP traffic.

## File Upload
* The file extension is compared with the file content, to check if they match as a security precaution.
* The application currently handles PNG & MP4 files.
* The uploaded file gets a UUID filename, as a security precaution.
* The 'SenderId', 'ReceiverId' and 'AllowedDownloads' are stored as metadata with the uploaded file.
* The file cannot be larger than 500MB in size.

## Logging
* The project uses the `Serilog` package for logging.
* The following element are present in the logs:
  - Date & Time
  - Log level
  - Class that generates the log
  - Log message
  - Exception
* HTTP activities are also logged.
* Logs are printed to the Console and send to a Datalust Seq instance for centralized logging.

## Swagger
* The project makes use of Swagger, to have an easy webpage for invoking endpoints.
* Every endpoint method contains comment with XML, to label the following:
  - What every endpoint is for
  - What information the endpoint requires.
  - Which status codes the endpoint can respond with.
* The adress to access the Swagger page: localhost:{port}/swagger

## External services
* The project communicates with several external services.
* The required credentials for them are present in the `appsettings-template.json` file.
* To run this project, one must create an `appsettings.json` file and place the value for the credentials.

### DigitalOcean Spaces & GCP Secret Manager
* The project depends on a DigitalOcean Spaces instance.
* This is the file storage service for the program, to save and retrieve files.
* The credentials to interact with this service, are stored inside GCP Secret Manager.
* To communicate with GCP, three item are required.
  - A JSON file of the Service Account credentials, that is present on the machine.
  - An Environment Variable called `GOOGLE_APPLICATION_CREDENTIALS`, which points to the location of the Service Account JSON.
  - The GCP project id (`ProjectId`), of the project that contains the Secrets. This value needs to be placed inside the `appsettings.json` file.

### RabbitMQ
* The project also depends on a RabbitMQ instance, to exchange data between applications.
* It requires a `HostName` and `Port` to communicate with the instance.
* These values need to be placed inside the JSON file.
* It uses the following routingKeys:
  - `create` when a new file is uploaded and a link needs to be created.
  - `delete` when an existing file is downloaded and the associated link needs to be removed.

### Seq
* The project also depends on a Seq instance, to send log data to.
* It requires a `ServerUrl` and `ApiKey` to communicate with it.
* These values need to be placed inside the JSON file.

### DeleteFileApp
* The project also depends on the 'DeleteFileApp' program, which is used to delete files from the file storage service after they have been downloaded.
* It requires the `Endpoint` of the program, for sending delete requests.
* This value needs to be placed inside the JSON file.

### JWT
* The project requires information about verifying JWT tokens, as to validate requests.
* To make sure that the request contains a valid JWT, the project contains logic to check for it.
* The logic needs the `Issuer` & `Secret` values to validate the JWT.
* These values need to be placed inside the JSON file.

## Deployment
* The application runs on port `6000` when running the Docker image.
* The project read config information from the Environment Variables of the machine, when the environment type is set to `Release`.
* The following values are required:

| Environment Variable | Purpose                                                        |
|:--------------------:|----------------------------------------------------------------|
| RabbitMQ_HostName    | Host of RabbitMQ instance for exchanging data between programs |
| RabbitMQ_Port        | Port to reach the RabbitMQ instance                            |
| JWT_Issuer           | Issuer of tokens                                               |
| JWT_Secret           | Secret for signing tokens                                      |
| Seq_ServerUrl        | Host of Seq instance for ingesting log entries                 |
| Seq_ApiKey           | API key for the project                                        |
| DeleteFile_Endpoint  | Endpoint for reaching the DeleteFile program to delete files   |
| GCP_ProjectId        | Project id of the GCP project that contains the Secrets        |