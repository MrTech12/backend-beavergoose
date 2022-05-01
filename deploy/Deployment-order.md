# Deployment Order

* There are a few components that needs to be deployed inside the Kubernetes cluster. A couple of them are required by other component, that they are given priority. <br><br>
* This file provides an overview of all component and their deployment order.

1. RabbitMQ
* The inner-communication, Event Bus solution for all microservices.
* Certain services **expect** a working RabbitMQ instance upon startup.
* This will be the **first** deployed component. <br><br>


2. Seq
* Centralized logging system for all other components.
* An instance of this component is **required** by other services for sending logs info.
* The **passwordhash** for the user is stored in a `Secrets` object.
* This component makes use of a `Persistent Volume` & `Persistent Volume Claim` for storing log data.
* After deployment, one needs to login in order to create an **API key**, that is used by other applications to ingest data.
* This will be the **second** deployed component. <br><br>


3. PostgeSQL database for LinkMicroservice
* Database for storing data from LinkMicroservice.
* Is **not required** upon bootup of the service, but is needed for functionality.
* The **password of the user** is stored in a `Secrets` object.
* This component makes use of a `Peristent Volume` & `Peristent Volume Claim` for storing user data. 
* This will be the **third** deployed component. <br><br>


4. APIGateway
* The component that the frontend will talk to & communicates with the other microservices.
* Does **not require** that other microservices are available upon bootup of the gateway, but they are needed for functionality.
* This component depends on: Seq for logging storage.
* The **API key for Seq** is stored in a `Secrets` object.
* This will be the **fourth** deployed component. <br><br>

# LinkMicroservice
* This component makes use of Entity Framework, which requires that the database contains a certain schema in order to read and store data.
* The deployment of this component contains an `InitContainer`, which reaches out to a `Kubernetes Job` before starting the 'LinkMicroservice' component.
* The `Job` updates the database to the desired schema. After the Job has been completed, the regular component will start.
* The **connection string for PostgreSQL** is stored in a `Secrets` object.
* The **API key for Seq** is stored in a `Secrets` object.
* This component depends on: RabbitMQ for messaging, PostgeSQL database for data storage & Seq for logging storage. <br><br>
* This can be the fifth or six deployed component.<br><br>

# LinkMicroservice
* The **credentials for DigitalOcean Spaces** is stored in a `Secrets` object.
* The **API key for Seq** is stored in a `Secrets` object.
* This component depends on: RabbitMQ for messaging, Seq for logging storage and External file storage: DigitalOcean Spaces.
* This can be the fifth or six deployed component.<br><br>