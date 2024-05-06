TRUNCATE TABLE [dbo].[Locations] 
GO 

SET IDENTITY_INSERT [dbo].[Locations] ON 
GO
INSERT [dbo].[Locations] ([LocationId], [LocationName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (1, N'GURGAON FACTORY', 1, CAST(N'2023-07-26T17:46:34.637' AS DateTime), 1, CAST(N'2023-07-26T17:46:34.637' AS DateTime), 1, 0)
GO
INSERT [dbo].[Locations] ([LocationId], [LocationName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (2, N'MPT ENGINE PLANT
', 1, CAST(N'2023-07-26T17:46:38.300' AS DateTime), 1, CAST(N'2023-07-26T17:46:38.300' AS DateTime), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Locations] OFF
GO