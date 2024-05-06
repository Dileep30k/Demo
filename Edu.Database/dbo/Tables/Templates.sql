CREATE TABLE [dbo].[Templates] (
    [TemplateId]            BIGINT        IDENTITY (1, 1) NOT NULL,
    [TemplateName]          VARCHAR (100) NOT NULL,
    [TemplateKey]           VARCHAR (100) NOT NULL,
    [TemplateSubject]       VARCHAR (255) NOT NULL,
    [TemplateContent]       VARCHAR (MAX) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Templates_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Templates_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Templates_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Templates_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Templates_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Templates_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED ([TemplateId] ASC)
);

