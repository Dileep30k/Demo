TRUNCATE TABLE [dbo].[Users] 
GO 

SET IDENTITY_INSERT [dbo].[Users] ON 
GO 

INSERT INTO [dbo].[Users]
           ([UserId]
		   ,[Email]
           ,[FirstName]
           ,[LastName]
           ,[Password]
           ,[Salt]
           ,[IsAdmin]
		   ,[MsilUserId])
     VALUES
           (1,
		   N'admin@test.com',
		   N'Admin',
		   N'Administrator',
		   N'eYwgHqZ/DHGPfUMj8Swk/YSsiUPRGZq/WfSNv04VLfQ=', --Admin123
		   N'j0E1sK50Dw7aPQ==',
		   1,
		   1000)
GO

SET IDENTITY_INSERT [dbo].[Users] OFF
GO


