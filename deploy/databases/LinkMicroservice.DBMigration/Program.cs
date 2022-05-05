// See https://aka.ms/new-console-template for more information
using LinkMicroservice.DBMigration.Data;

Console.WriteLine("Console app to create a DB schema on a DB, for LinkMicroservice in a deployment environment.");
Console.WriteLine("Creating DB schema...");

var linkContext = new LinkContext();
linkContext.Database.EnsureCreated();

Console.WriteLine("Application finished!");