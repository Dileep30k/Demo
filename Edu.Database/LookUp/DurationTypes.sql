TRUNCATE TABLE [dbo].[DurationTypes] 
GO 

SET IDENTITY_INSERT [dbo].[DurationTypes] ON 
GO
INSERT [dbo].[DurationTypes] ([DurationTypeId], [DurationTypeName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (1, N'Year', 1, CAST(N'2023-07-27T15:08:46.360' AS DateTime), 1, CAST(N'2023-07-27T15:08:46.360' AS DateTime), 1, 0)
GO
INSERT [dbo].[DurationTypes] ([DurationTypeId], [DurationTypeName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (2, N'Month', 1, CAST(N'2023-07-27T15:08:50.867' AS DateTime), 1, CAST(N'2023-07-27T15:08:50.867' AS DateTime), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[DurationTypes] OFF
GO
