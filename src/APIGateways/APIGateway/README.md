# API Gateway

## Overview
* API Gateway project that uses the Ocelot Nuget package and is part of the 'backend-beavergoose' project.

## ocelot.json
* File which contains the routes that the gateway accepts requests from and the routes which the gateway sends data to.
* There are two JSON files for the different environment: `Development` & `Production`.
* When adding a new route, make sure it is present in both JSON files.

## Setup
* The project uses the 'APIGateway' profile, configured in the `Properties` --> `launchSettings.json` file.
* The service runs on port `5000` for HTTP traffic.

## CORS
* The project uses a custom CORS policy, to only allow requests from specific origins.
* These origins are placed inside the `Program.cs` file.
* It is possible to retrieve the valid origins from the JSON file, at a later moment in time.

## External services
* The gateway needs a couple configuration values to function.
* The required configuration keys are present in the `appsettings-template.json` file.
* To run this project, one must create an `appsettings.json` file and place the values of the configuration keys.

### JWT
* Most requests require authentication, via a JSON Web Token (JWT).
* To make sure that the request contains a valid JWT, the gateway contains logic to check for it.
* The logic needs the `Issuer` & `Secret` values to validate the JWT.
* These values need to be placed inside the JSON file.

---

## Deployment
* The application runs on port `5001` when running the Docker image.
* The application reads config information from the Environment Variables of the machine, when the environment type is set to `Release`.
* The following values are required:

| Environment Variable | Purpose                   |
|:--------------------:|---------------------------|
| JWT_Issuer           | Issuer of tokens          |
| JWT_Secret           | Secret for signing tokens |