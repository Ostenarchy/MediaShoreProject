
use DIPLOMAT_01
go
create table Authors(
id int identity primary key,
authorFirstName nvarchar(150),
authorLastName nvarchar(150),
authorEmail nvarchar(100)
);
go

create table DiscTypes(
id int identity primary key,
typeName nvarchar(100)
);
go

create table DiscGenres(
id int identity primary key,
genreName nvarchar(150),
typeID int references DiscTypes(id)
);
go

create table Roles(
id int identity primary key,
roleName nvarchar(100)
);
go

create table Users(
id int identity primary key,
userName nvarchar(200),
userPassword nvarchar(200),
roleID int references Roles(id),
userFirstName nvarchar(150),
userLastName nvarchar(150),
userEmail nvarchar(200),
userBalance int,
userRegDate date default GETDATE()
);
go

create table Discs(
id int identity primary key,
discName nvarchar(max),
discAuthorID int references Authors(id),
genreID int references DiscGenres(id),
discQuantityInStock int,
discReleaseDate date default GETDATE(),
discDescription nvarchar(max),
discPrice int,
discCoverPath nvarchar(max)
);
go

create table Statuses(
id int identity primary key,
statusName nvarchar(200)
);
go

create table Orders(
id int identity primary key,
userID int references Users(id),
orderDateOfPurchase date default GETDATE(),
totalSum int,
statusID int references Statuses(id)
);
go

create table OrderItem(
id int identity primary key,
orderID int references Orders(id),
discID int references Discs(id),
quantity int,
PriceAtTime int
);

