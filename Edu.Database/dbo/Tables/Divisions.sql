CREATE TABLE [dbo].[Divisions] (
    [DivisionId]            BIGINT        IDENTITY (1, 1) NOT NULL,
    [DivisionName]          VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Divisions_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Divisions_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Divisions_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Divisions_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Divisions_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Divisions_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Divisions] PRIMARY KEY CLUSTERED ([DivisionId] ASC)
);

