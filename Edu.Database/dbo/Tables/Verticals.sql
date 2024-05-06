CREATE TABLE [dbo].[Verticals] (
    [VerticalId]            BIGINT        IDENTITY (1, 1) NOT NULL,
    [VerticalName]          VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Verticals_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Verticals_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Verticals_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Verticals_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Verticals_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Verticals_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Verticals] PRIMARY KEY CLUSTERED ([VerticalId] ASC)
);

