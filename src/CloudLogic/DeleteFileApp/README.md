# DeleteFileAPP

## Overview
* API project for deleting a file of the file storage, that is part of the 'backend-beavergoose' project.

## Setup
* The project uses the 'DeleteFileApp' profile, configured in the `Properties` --> `launchSettings.json` file.
* The service runs on port `5400` for HTTP traffic.

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
* The gateway communicates with several external services.
* The required credentials for them are present in the `appsettings-template.json` file.
* To run this project, one must create an `appsettings.json` file and place the value for the credentials.

### DigitalOcean Spaces
* The project depends on a DigitalOcean Spaces, to find and delete files.
* It requires a `ServiceURL`, `BucketName`, `AccessKey` and `SecretAccessKey` to access it.
* These values need to be placed inside the JSON file.

### Seq
* The project also depends on a Seq instance, to send log data to.
* It requires a `ServerUrl` and `ApiKey` to communicate with it.
* These values need to be placed inside the JSON file.

### JWT
* The project requires information about verifying JWT tokens, as to validate requests.
* To make sure that the request contains a valid JWT, the project contains logic to check for it.
* The logic needs the `Issuer` & `Secret` values to validate the JWT.
* These values need to be placed inside the JSON file.

## Deployment
* The application runs on port `4000` when running the Docker image.
* The application reads config information from the Environment Variables of the machine, when the environment type is set to `Release`.
* The following values are required:

| Environment Variable         | Purpose                                                      |
|:----------------------------:|--------------------------------------------------------------|
| JWT_Issuer                   | Issuer of tokens                                             |
| JWT_Secret                   | Secret for signing tokens                                    |
| Seq_ServerUrl                | Host of Seq instance for ingesting log entries               |
| Seq_ApiKey                   | API key for the project                                      |
| DigitalOcean_ServiceURL      | host of DigitalOcean Spaces instance as file storage service |
| DigitalOcean_BucketName      | Name of storage/object bucket                                |
| DigitalOcean_AccessKey       | Access key to authenticate with file storage                 |
| DigitalOcean_SecretAccessKey | Secret access key to authenticate with file storage          |