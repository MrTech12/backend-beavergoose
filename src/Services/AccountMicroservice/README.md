# AccountMicroservice

## Overview
* Microservice projects for the Account application, that is part of the backend-beavergoose project.
* The application creates and manages users, access tokens and refresh tokens.

## Setup
* The project uses the 'AccountMicroservice' profile, configured in the `Properties` --> `launchSettings.json` file.
* The service runs on port `5300` for HTTP traffic.

## Account creation
* When creating an account, the following requirements need to be met:
  - An entered Email must not be in use by another user.
  - A valid passwords needs to have a minimum of 8 characters.
  - A valid passwords needs to contain a capital letters, a number and a special character.
* ASP.NET Core Identity is used to create and manage accounts.
* The passwords get hashed by Identity before being stored in the database.
* ASP.NET Core Version 3 is used for hashing passwords.

## Logging in
* Upon sending valid account credentials, the application returns an Access Token and Refresh Token.
* The Access Token is valid for **15 minutes**. After that, both tokens need to be send to the application for generating new ones.

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
* This project uses a PostgreSQL database for storing and retrieving account data.
* It uses the ORM `Entity Framework Core` to interact with the database.
* It requires a `Connectionstring`, with the values of the host, database and user credentials to authenticate with it.
* This 'string' needs to be placed inside the JSON file.

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
* The application runs on port `8000` when running the Docker image.
* The project reads config information from the Environment Variables of the machine, when the environment type is set to `Release`.
* The following values are required:

| Environment Variable             | Purpose                                                                      |
|:--------------------------------:|------------------------------------------------------------------------------|
| AccountContext                   | Connectionstring with host, database and user credentials to access database |
| JWT_Issuer                       | Issuer of tokens                                                             |
| JWT_Secret                       | Secret for signing tokens                                                    |
| Seq_ServerUrl                    | Host of Seq instance for ingesting log entries                               |
| Seq_ApiKey                       | API key for the project                                                      |