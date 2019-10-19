/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *
  FROM Customers
alter table Customers
add CustUsername varchar(50)  null
alter table Customers
add CustPassword varchar(max)  null
update Customers
set CustUsername = CustFirstName
update Customers
set CustPassword = CustLastName

alter table Packages
add ImageFile varchar(50) null
update Packages
set ImageFile = PkgName + '.jpg'

select c.CustFirstName,c.CustLastName, cb.CustomerId,
       cb.BookingId,cb.BookingDate,cb.PackageId
from Bookings cb 
join Customers c on cb.CustomerId = c.CustomerId
select * from Bookings

insert into Bookings
values(GETDATE(),'JXWT213',3,145,'G',2)
insert into Bookings
values(GETDATE(),'JXDG213',1,145,'L',1)

alter table Agents
 add Password varchar(50);
 alter table Agents
 add Username varchar(50);
 update Agents
 set password = AgtLastName
 update Agents
 set Username = AgtFirstName
                                      