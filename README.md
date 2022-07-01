# backend beavergoose

## Overview
This is a file sharing service, where people can exchange files with each other. The recipient of the file, receives a 'link' to retrieve them.
The files are deleted from the service, once the amount of 'allowed downloads' has been reached. <br>
The service is build with ASP.NET Core, using C# and .Net 6.

---

The following aspects are used for this project:
* Microservices
* Messaging (Communication between microservices)
* ASP.NET Core Identity for account management.
* JSON Web Tokens as Access Token for Authentication with the API's.
* Unit Testing with Stubs.
* Integration Testing with a real database in Docker.
* CI/CD with GitHub Actions.
* Retrieving Test Reports and Code Coverage data from the CI environment.
* Automated Integration Testing by using a database in Docker & a Bash script to run the database and tests.
* Automated Deployments using 'release' branches, Kubernetes and the DigitalOcean CLI.

---

## Directories

* Source code for the projects, is located in the `src` directory.
* Test code for the projects, is located in the `test` directory.
* Kubernetes Manifests for the projects, are located in the `deploy` directory.

---

## Current Limitations (As of 29-6-2022)
* The application deletes files upon dowloading them, regardless of the 'allowed downloads' amount.
* The files that get stored in the file storage service, are not encrypted beforehand.