# Pool-Game

## Brief description 

- The application is used to book billiard tables
- Supports a user who can book tables and have an overview of their reservations
- Supports an admin who can also book and have an overview of all reservations in the database in RT

## Relational Schema

![Realtional Schema](https://github.com/lukijan98/Pool-Game/blob/main/img/database.png?raw=true)


## Technology used

- .NET Core/ASP.NET Core MVC
- Entity Framework Core
- Identity Core
- SignalR
- MS SQL Server Express


## Note
App has a database initializer who creates the database if it doesnt exist, and adds a default admin with parameters:
- email: admin@email.com
- password: Admin1!
