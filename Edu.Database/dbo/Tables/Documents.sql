CREATE TABLE [dbo].[Documents] (
    [DocumentId]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [DocumentTableId] BIGINT       NOT NULL,
    [DocumentTable]  VARCHAR (50)  NOT NULL,
    [DocumentType]   VARCHAR (50)  NOT NULL,
    [FileName]       VARCHAR (100) NULL,
    [FilePath]       VARCHAR (255) NULL,
    [ContentType]    VARCHAR (100) NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Documents_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Documents_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Documents_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Documents_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Documents_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Documents_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED ([DocumentId] ASC)
);

