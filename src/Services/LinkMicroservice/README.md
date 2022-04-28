# LinkMicroservice

* Microservice projects for the Link application, that is part of the backend-beavergoose project.


## Setup
* This project depends on a MySQL database and a RabbitMQ connection.
* The required credentials for these services, are placed in the `appsettings-template.json` file.
* To run this app, one must create an `appsettings.json` file and place the required credentails inside it.

## Deployment
* The project read config information from the environment values of the machine, when the environment type is set to `Release`.

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