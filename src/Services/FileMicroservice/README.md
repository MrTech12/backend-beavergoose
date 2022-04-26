# FileMicroservice

* Microservice projects for the File application, that is part of the backend-beavergoose project.


## Setup
* This project depends on a DigitalOcean Spaces instance and a RabbitMQ connection.
* The required credentials for these services, are placed in the `appsettings-template.json` file.
* To run this app, one must create an `appsettings.json` file and place the required credentails inside it.


## Deployment
* The project read config information from the environment values of the machine, when the environment type is set to `Release`.