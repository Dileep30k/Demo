TRUNCATE TABLE [Settings]

SET IDENTITY_INSERT [dbo].[Settings] ON 
GO
INSERT [dbo].[Settings] ([SettingId], [SettingName], [SettingKey], [SettingValue], [SettingDesc]) VALUES (1, N'Reciepient List', N'ReciepientList', N'ALLEOs@maruti.co.in,allsrdvm@maruti.co.in,alldvms@maruti.co.in,allddvms@maruti.co.in,allsfmgr2@maruti.co.in,allsfmgr1@maruti.co.in,allmsildpms@maruti.co.in,ALLFMGRs@maruti.co.in,ALLDEPTOFFICERS@maruti.co.in,Manoj.Agrawal@maruti.co.in,SK.Sinha@maruti.co.in,Sujay.Gupta@maruti.co.in,Sushant.Kumar@maruti.co.in,kumar.dharmendra@maruti.co.in,Neha.Gupta@maruti.co.in,Aakash.Sharma@maruti.co.in', N'CC Email address while publish admission by GTS. Comma seprated for multiple emails.')
GO
INSERT [dbo].[Settings] ([SettingId], [SettingName], [SettingKey], [SettingValue], [SettingDesc]) VALUES (2, N'Override Email Address', N'OverrideEmail', N'', N'Override Email address for Development')
GO
SET IDENTITY_INSERT [dbo].[Settings] OFF
GO