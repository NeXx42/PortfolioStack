Command to update database, run under ./backend

```sh
dotnet ef database update --project Portfolio.Data --startup-project Portfolio.Api
```

Adding migrations
```sh
dotnet ef migrations add AddEmailHash --project Portfolio.Data --startup-project Portfolio.Api/
```