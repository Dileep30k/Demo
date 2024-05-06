CREATE TABLE [dbo].[DurationTypes] (
    [DurationTypeId]                BIGINT        IDENTITY (1, 1) NOT NULL,
    [DurationTypeName]              VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_DurationTypes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_DurationTypes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_DurationTypes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_DurationTypes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_DurationTypes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_DurationTypes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_DurationTypes] PRIMARY KEY CLUSTERED ([DurationTypeId] ASC)
);

