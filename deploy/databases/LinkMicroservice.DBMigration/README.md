# LinkMicroservice.DBMigration

* A project to automate the creation of the desired schema, for the LinkMicroservice in a production environment.
* It is a simple Console application, which makes use of Entity Framework Core and Npgsql packages for the database logic.
* The application can also be packaged with Docker for use in a deployment scenario.
* The application is being used for a Kubernetes Job that gets executed by the Init Container of the LinkMicroservice deployment.

# Setup
* The application expects an environment variable of `ConnectionString`, which has the connection string of the database where the migration will take place on.
* The `appsettings-template.json` file has an template for creating a valid connection string.