CREATE TABLE [dbo].[DocumentTypes] (
    [DocumentTypeId]                BIGINT        IDENTITY (1, 1) NOT NULL,
    [DocumentTypeName]              VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_DocumentTypes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_DocumentTypes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_DocumentTypes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_DocumentTypes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_DocumentTypes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_DocumentTypes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_DocumentTypes] PRIMARY KEY CLUSTERED ([DocumentTypeId] ASC)
);

