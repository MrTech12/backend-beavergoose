# LinkMicroservice

* Microservice projects for the Link application, that is part of the backend-beavergoose project.


## Setup
* This project depends on a MySQL database and a RabbitMQ connection.
* The required credentials for these services, are placed in the `appsettings-template.json` file.
* To run this app, one must create an `appsettings.json` file and place the required credentails inside it.

## Deployment
* The project will need to retrieve the values of the `appsettings.json` file, when it is used in a deployment environment.
* The retrieval of the database string, for the `LinkContext`, MUST CHANGE before creating a deployment variant of the program.

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