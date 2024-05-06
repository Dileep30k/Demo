﻿CREATE TABLE [dbo].[AdmissionUsers] (
    [AdmissionUserId]       BIGINT        IDENTITY (1, 1) NOT NULL,
    [AdmissionId]           BIGINT        NOT NULL,
    [UserId]                BIGINT        NOT NULL,
    [NominationId]          BIGINT        NOT NULL,
    [NominationInstituteId] BIGINT        NOT NULL,
    [AdmissionStatusId]     BIGINT        NOT NULL,
    [Rank]                  INT           NOT NULL,
    [IsConfirmByEmp]        BIT           NULL,
    [ConfirmDate]           DATETIME      NULL,
    [EmployeeRemarks]       VARCHAR (100) NULL,
    [IsConfirmByInstitute]  BIT           NULL,
    [ApproverRemarks]       VARCHAR (255) NULL,
    [Semester]              VARCHAR (50)  NULL,
    [IsBondAccepted]        BIT           NULL,
    [BondAcceptedDate]      DATETIME      NULL,
    [IsActive]              BIT           CONSTRAINT [DF_AdmissionUsers_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_AdmissionUsers_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_AdmissionUsers_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_AdmissionUsers_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_AdmissionUsers_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_AdmissionUsers_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_AdmissionUsers] PRIMARY KEY CLUSTERED ([AdmissionUserId] ASC)
);
