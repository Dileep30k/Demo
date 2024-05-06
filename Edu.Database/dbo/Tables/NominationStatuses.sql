CREATE TABLE [dbo].[NominationStatuses] (
    [NominationStatusId]                BIGINT        IDENTITY (1, 1) NOT NULL,
    [NominationStatusName]              VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_NominationStatuses_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_NominationStatuses_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_NominationStatuses_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_NominationStatuses_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_NominationStatuses_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_NominationStatuses_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_NominationStatuses] PRIMARY KEY CLUSTERED ([NominationStatusId] ASC)
);

