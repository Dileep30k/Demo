CREATE TABLE [dbo].[Schemes] (
    [SchemeId]              BIGINT        IDENTITY (1, 1) NOT NULL,
    [SchemeName]            VARCHAR (100) NOT NULL,
    [SchemeCode]            VARCHAR (50)  NOT NULL,
    [Duration]              INT           NOT NULL,
    [DurationTypeId]        BIGINT        NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Schemes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Schemes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Schemes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Schemes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Schemes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Schemes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Schemes] PRIMARY KEY CLUSTERED ([SchemeId] ASC)
);

