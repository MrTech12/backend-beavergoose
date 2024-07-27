# Deployment Order

* There are a few components that needs to be deployed inside the Kubernetes cluster. A couple of them are required by other component, that they are given priority.
* This file provides an overview of all component and their deployment order. <br><br>

1. Metric Server
* This component fetches metrics data (CPU, RAM) from the worker nodes.
* It is used for implementing scaling in the cluster.
* This will be the **first** deployed component.
* The `metrics-server-deployment.yml` manifest needs to be applied, which handles all the parts of the Metric Server. <br><br>

2. Dbmigration Account
* This component is used for applying a Service Account.
* This account is used to get info about the `Jobs` resource.
* This will be the **second** deployed component.
* The `dbmigration-account.yml` manifest needs to be applied, which handles all the parts of the Metric Server. <br><br>

3. RabbitMQ
* The inner-communication, Event Bus solution for all microservices.
* Certain services **expect** a working RabbitMQ instance upon startup.
* This will be the **third** deployed component.
* First the `deployment` & then the `service` manifest needs to be applied. <br><br>

4. Seq
* Centralized logging system for all other components.
* An instance of this component is **required** by other services to send logs to.
* This component makes use of a `Persistent Volume` & `Persistent Volume Claim` for storing log data.
* After deployment, a password needs to be set and  **API keys** need to be created for the other application, so that they can ingest log data.
* The following applications need an API Key:
  - AccountMicroservice
  - FileMicroservice
  - LinkMicroservice
  - DeleteFileApp
* The component uses `Helm` for deployment and makes use of an `Ingress` for UI and ingesting log data.
* This will be the **fourth** deployed component. 
* The commands to deploy Seq are as follows:
```yml
1. kubectl apply -f seq-volume.yml # Create the Volume & Claims for storage of Seq data.
2. helm install -f seq-config.yml my-seq datalust/seq # Deploy seq onto the cluster.
```
<br><br>

5. PostgeSQL database for LinkMicroservice
* Database for storing user data from LinkMicroservice.
* Is **not required** upon bootup of the service, but is needed for functionality.
* The **password of the user** is stored in a `Secrets` object.
* This component makes use of a `Peristent Volume` & `Peristent Volume Claim` for storing user data. 
* This will be the **fifth** deployed component.
* First the `volume`, then `deployment` & then the `service` manifest needs to be applied. <br><br>

6. PostgeSQL database for AccountMicroservice
* Database for storing user data from AccountMicroservice.
* Is **not required** upon bootup of the service, but is needed for functionality.
* The **password of the user** is stored in a `Secrets` object.
* This component makes use of a `Peristent Volume` & `Peristent Volume Claim` for storing user data. 
* This will be the **Sixth** deployed component. 
* First the `volume`, then `deployment` & then the `service` manifest needs to be applied. <br><br>

7. APIGateway
* The component that the frontend will talk to & communicates with the other microservices.
* Does **not require** that other microservices are available upon bootup of the gateway, but they are needed for functionality.
* The **JWT values** are stored in a `Secrets` object.
* This will be the **seventh** deployed component. <br><br>

8. LinkMicroservice
* This component makes use of `Entity Framework Core`, which requires that the database contains a certain schema in order to read and store data.
* The deployment of this component contains an `InitContainer`, which reaches out to a `Kubernetes Job` before starting the 'LinkMicroservice' container.
* The `Job` applied the desired schema to the database. After the Job has been completed, the regular container will start.
* There is a custom `ServiceAccount` & `Role` which allows the `InitContainer` to access the status of the `Job`. Without the Account, the `InitContainer` would receive a *Forbidden* error.
* The **connectionstring for PostgreSQL** is stored in a `Secrets` object.
* The **API key for Seq** is stored in a `Secrets` object.
* The **JWT values** are stored in a `Secrets` object.
* This component depends on: `PostgeSQL database` for storing user data, `RabbitMQ` for messaging & `Seq` for sending logs to.
* This can be the eight or above deployed component.<br><br>

9. AccountMicroservice
* This component makes use of `Entity Framework Core`, which requires that the database contains a certain schema in order to read and store data.
* The deployment of this component contains an `InitContainer`, which reaches out to a `Kubernetes Job` before starting the 'AccountMicroservice' container.
* The `Job` applied the desired schema to the database. After the Job has been completed, the regular container will start.
* There is a custom `ServiceAccount` & `Role` which allows the `InitContainer` to access the status of the `Job`. Without the Account, the `InitContainer` would receive a *Forbidden* error.
* The **connectionstring for PostgreSQL** is stored in a `Secrets` object.
* The **API key for Seq** is stored in a `Secrets` object.
* The **JWT values** are stored in a `Secrets` object.
* This component depends on: `PostgeSQL database` for storing user data & `Seq` for sending logs to.
* This can be the ninth or above deployed component.<br><br>

10. FileMicroservice
* The **connection information for the file datastore** are stored in a `Secrets` object.
* The **endpoint of the DeleteFileApp program** is stored in `Secrets` object.
* The **API key for Seq** is stored in a `Secrets` object.
* The **JWT values** are stored in a `Secrets` object.
* This component depends on: `PostgreSQL database` for storing user data, `RabbitMQ` for messaging and `Seq` for sending logs to.
* This can be the tenth or above deployed component.<br><br>