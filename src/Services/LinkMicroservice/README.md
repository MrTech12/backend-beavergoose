# LinkMicroservice

## Using Entity Framework CLI

* To create a new migration, use this command for passing a custom folder to place the files into:
```
dotnet ef migrations add AAA -o Data/Migrations
```

* To remove the previous migration, use this command:
```
dotnet ef migrations remove
```

* To update the database to the migration, use this command with the name of the migration:
```
dotnet ef database update AAA
```