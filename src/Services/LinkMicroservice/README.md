# LinkMicroservice

## Overview
* Microservice projects for the Link application, that is part of the backend-beavergoose project.
* The application creates and deletes links for downloading user uploaded files.

## Setup
* The project uses the 'LinkMicroservice' profile, configured in the `Properties` --> `launchSettings.json` file.
* The service runs on port `5100` for HTTP traffic.

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

### PostgreSQL database
* This project uses a PostgreSQL database for storing and retrieving link data.
* It uses the ORM `Entity Framework Core` to interact with the database.
* It requires a `Connectionstring`, with the values of the host, database and user credentials to authenticate with it.
* This 'string' needs to be placed inside the JSON file.

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

### JWT
* The project requires information about verifying JWT tokens, as to validate requests.
* To make sure that the request contains a valid JWT, the project contains logic to check for it.
* The logic needs the `Issuer` & `Secret` values to validate the JWT.
* These values need to be placed inside the JSON file.

## Using Entity Framework CLI
* To create a new migration and pass a custom folder for the files, use this command:
```
dotnet ef migrations add name_of_migration -o Data/Migrations
```

* To remove the previous migration, use this command:
```
dotnet ef migrations remove
```

* To update the database to the migration, use this command with the name of the migration:
```
dotnet ef database update name_of_migration
```

* To create an SQL script of all the migrations, use this command with the name of the first migration:
```
dotnet ef migrations script name_of_first_migration --output schema.sql
```

* To get information about a DbContext:
```
dotnet ef dbcontext info
```

## Deployment
* The application runs on port `7000` when running the Docker image.
* The project read config information from the Environment Variables of the machine, when the environment type is set to `Release`.
* The following values are required:

| Environment Variable          | Purpose                                                                      |
|:-----------------------------:|------------------------------------------------------------------------------|
| ConnectionStrings_LinkContext | Connectionstring with host, database and user credentials to access database |
| RabbitMQ_HostName             | Host of RabbitMQ instance for exchanging data between programs               |
| RabbitMQ_Port                 | Port to reach the RabbitMQ instance                                          |
| JWT_Issuer                    | Issuer of tokens                                                             |
| JWT_Secret                    | Secret for signing tokens                                                    |
| Seq_ServerUrl                 | Host of Seq instance for ingesting log entries                               |
| Seq_ApiKey                    | API key for the project                                                      |