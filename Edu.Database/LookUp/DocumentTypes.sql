TRUNCATE TABLE [dbo].[DocumentTypes] 
GO 

SET IDENTITY_INSERT [dbo].[DocumentTypes] ON 
GO
INSERT [dbo].[DocumentTypes] ([DocumentTypeId], [DocumentTypeName]) VALUES (1, N'Service Agreement')
GO
INSERT [dbo].[DocumentTypes] ([DocumentTypeId], [DocumentTypeName]) VALUES (2, N'Surity Bond')
GO																					   
SET IDENTITY_INSERT [dbo].[DocumentTypes] OFF
GO
