insert into AspNetUsers values
('3F3B0DBF-BF6A-4BD7-9858-27ABB5A486E0', 'Maria Akram karam', 'Maria Akram karam', 'maria@gmail.com', 'maria@gmail.com', 1,
'AQAAAAIAAYagAAAAEL+VTEUhN2nZuKtAgv6PeFz//Qagri1ZgjqFp8mD5XX7lyDXqA4to1+lOcHFqPi1aQ==',
newid(), newid(), '', 1, 0, NULL, 1, 0) 

insert into students values
('3F3B0DBF-BF6A-4BD7-9858-27ABB5A486E0', '', '', '', 1, '', '000', NULL, 0)

insert into AspNetUserRoles values
('3F3B0DBF-BF6A-4BD7-9858-27ABB5A486E0', (select id from AspNetRoles where name = 'Student'))

select * from aspnetusers where username = 'Maria Akram karam'
delete aspnetusers where email = 'maria@gmail.com'