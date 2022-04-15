# Folder Structure of every microservice project

## Controllers
* Contains controllers, which make API endpoints available to the Gateway.

## Data
* Contains DBContext and implementations of the Data Interfaces.

## DTOs
* Contains Data Transfer Objects for transferring data to and from Controllers, Services, Repositories and external Services.

## Entities
* Contains Models of the data objects that are stored/retrieved from the database and are used to create database tables.

## Interfaces
* Contains interfaces for the Data Repositories and Service classes.

## Services
* Contains Business logic of the specific microservice.