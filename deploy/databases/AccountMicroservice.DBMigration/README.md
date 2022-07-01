# AccountMicroservice.DBMigration

## Overview
* Project to automate the creation of the desired schema, for the 'AccountMicroservice' program in a production environment.
* It is a simple Console application, which makes use of `Entity Framework Core` and `Npgsql` packages for the database logic.
* It creates the schema, based on the classes in the `Entities` folder.
* The application can also be packaged with Docker for use in a container environment.

## External services
* The project communicates with one external services.
* The required credentials for it are present in the `appsettings-template.json` file.
* To run this project, one must create an `appsettings.json` file and place the value for the credentials.

### PostgreSQL database
* This project applies the schema on a PostgreSQL database.
* It uses the ORM `Entity Framework Core` to interact with the database.
* It requires a `Connectionstring`, with the values of the host, database and user credentials to authenticate with it.
* This 'string' needs to be placed inside the JSON file.

## Setup
* The application expects an environment variable of `ConnectionString`, which has the connection string of the database where the migration will take place on.
* The `appsettings-template.json` file has an template for creating a valid connection string.

## Deployment
* The application is being used for a Kubernetes Job that gets invoked by the 'Init Container' of the 'AccountMicroservice' deployment.
* The project read config information from the Environment Variables of the machine, when the environment type is set to `Release`.
* The following values are required:

| Environment Variable             | Purpose                                                                      |
|:--------------------------------:|------------------------------------------------------------------------------|
| ConnectionStrings_AccountContext | Connectionstring with host, database and user credentials to access database |