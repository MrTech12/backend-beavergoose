// See https://aka.ms/new-console-template for more information

using AccountMicroservice.Data;

Console.WriteLine("Console app to create a DB schema on a DB, for AccountMicroservice in a deployment environment.");
Console.WriteLine("Creating DB schema...");

var accountContext = new AccountContext();
accountContext.Database.EnsureCreated();

Console.WriteLine("Application finished!");
