TRUNCATE TABLE [dbo].[Roles] 
GO 

SET IDENTITY_INSERT [dbo].[Roles] ON 
GO
INSERT [dbo].[Roles] ([RoleId], [RoleName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (1, N'GST', 1, CAST(N'2023-07-26T17:46:05.480' AS DateTime), 1, CAST(N'2023-07-26T17:46:05.480' AS DateTime), 1, 0)
GO
INSERT [dbo].[Roles] ([RoleId], [RoleName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (2, N'Approver', 1, CAST(N'2023-07-26T17:46:10.960' AS DateTime), 1, CAST(N'2023-07-26T17:46:10.960' AS DateTime), 1, 0)
GO
INSERT [dbo].[Roles] ([RoleId], [RoleName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (3, N'Employee', 1, CAST(N'2023-07-26T17:46:13.770' AS DateTime), 1, CAST(N'2023-07-26T17:46:13.770' AS DateTime), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
