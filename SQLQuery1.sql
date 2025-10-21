--Insert into AspNetUserRoles 
--select id, '01C0D75E-CE56-46AF-88F7-B9749C250B9E' from AspNetUsers where id not in (select USERID from AspNetUserRoles)

--insert into AspNetUsers Values
--(NEWID(), 'mary john', 'mary john', 'mj.naguib@outlook.com', 'mj.naguib@outlook.com', 1, 'AQAAAAIAAYagAAAAEDJZYG2O/sg7rRKiZ5cE28Fo9nbGI11sNHNsWlbra8X7lbYgreABDiE9bLuYYmHFBA==', newId(), newId(),
--'01001965074‬', 1, 0, NULL, 1, 0)

--insert into AspNetUsers Values
--(NEWID(), 'farida amr', 'farida amr', 'didaamr995@gmail.com', 'didaamr995@gmail.com', 1, 'AQAAAAIAAYagAAAAEOX1ShNoePueqx+xrKe1Aynyw6ZQI04nygjDkgilGAagEfxurLt4poY0AtYf5kc3Qg==', newId(), newId(),
--'01104360442‬', 1, 0, NULL, 1, 0)

--Insert into Students Values
--('67DAD0A8-D99D-4288-AF51-CCE855B4621E', '1104360442‬', N'شارع فارسكور مصر الجديده', N'مستر تادروس مدرس في مدرستي', 1, N'مستر تادروس مدرس في مدرستي', '000', NULL, 0)

select viewsCount from StudentLectures where ViewsCount > 0