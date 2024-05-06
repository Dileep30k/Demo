CREATE TABLE [dbo].[Designations] (
    [DesignationId]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [DesignationName]       VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Designations_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Designations_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Designations_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Designations_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Designations_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Designations_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Designations] PRIMARY KEY CLUSTERED ([DesignationId] ASC)
);

