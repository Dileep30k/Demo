CREATE TABLE [dbo].[AdmissionStatuses] (
    [AdmissionStatusId]                BIGINT        IDENTITY (1, 1) NOT NULL,
    [AdmissionStatusName]              VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_AdmissionStatuses_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_AdmissionStatuses_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_AdmissionStatuses_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_AdmissionStatuses_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_AdmissionStatuses_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_AdmissionStatuses_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_AdmissionStatuses] PRIMARY KEY CLUSTERED ([AdmissionStatusId] ASC)
);

