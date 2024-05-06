TRUNCATE TABLE [dbo].[AdmissionStatuses] 
GO 

SET IDENTITY_INSERT [dbo].[AdmissionStatuses] ON 
GO
INSERT [dbo].[AdmissionStatuses] ([AdmissionStatusId], [AdmissionStatusName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (1, N'Confirm', 1, CAST(N'2023-07-26T17:42:41.203' AS DateTime), 1, CAST(N'2023-07-26T17:42:41.203' AS DateTime), 1, 0)
GO
INSERT [dbo].[AdmissionStatuses] ([AdmissionStatusId], [AdmissionStatusName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (2, N'Waiting', 1, CAST(N'2023-07-26T17:42:46.957' AS DateTime), 1, CAST(N'2023-07-26T17:42:46.957' AS DateTime), 1, 0)
GO
INSERT [dbo].[AdmissionStatuses] ([AdmissionStatusId], [AdmissionStatusName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (3, N'Active', 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, 0)
GO
INSERT [dbo].[AdmissionStatuses] ([AdmissionStatusId], [AdmissionStatusName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (4, N'Left', 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, 0)
GO
INSERT [dbo].[AdmissionStatuses] ([AdmissionStatusId], [AdmissionStatusName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (5, N'Pause', 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, 0)
GO
INSERT [dbo].[AdmissionStatuses] ([AdmissionStatusId], [AdmissionStatusName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (6, N'Accepted', 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, 0)
GO
INSERT [dbo].[AdmissionStatuses] ([AdmissionStatusId], [AdmissionStatusName], [IsActive], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsDeleted]) VALUES (7, N'Rejected', 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, CAST(N'2023-07-26T17:42:55.360' AS DateTime), 1, 0)
GO
SET IDENTITY_INSERT [dbo].[AdmissionStatuses] OFF
GO