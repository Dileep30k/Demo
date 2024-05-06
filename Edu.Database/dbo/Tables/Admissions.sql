CREATE TABLE [dbo].[Admissions] (
    [AdmissionId]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [SchemeId]              BIGINT        NOT NULL,
    [IntakeId]              BIGINT        NOT NULL,
    [InstituteId]           BIGINT        NOT NULL,
    [ApprovalBy1]           BIGINT        NULL,
    [Approved1]             BIT           NULL,
    [ApprovalDate1]         DATETIME      NULL,
    [ApprovalRemarks1]      VARCHAR (100) NULL,
    [ApprovalBy2]           BIGINT        NULL,
    [Approved2]             BIT           NULL,
    [ApprovalDate2]         DATETIME      NULL,
    [ApprovalRemarks2]      VARCHAR (100) NULL,
    [IsPublish]             BIT           CONSTRAINT [DF_Admissions_IsPublish] DEFAULT ((0)) NOT NULL,
    [IsConfirmUpload]       BIT           CONSTRAINT [DF_Admissions_IsConfirmUpload] DEFAULT ((0)) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Admissions_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Admissions_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Admissions_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Admissions_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Admissions_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Admissions_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Admissions] PRIMARY KEY CLUSTERED ([AdmissionId] ASC)
);

