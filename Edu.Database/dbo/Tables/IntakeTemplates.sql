CREATE TABLE [dbo].[IntakeTemplates] (
    [IntakeTemplateId]      BIGINT        IDENTITY (1, 1) NOT NULL,
    [IntakeId]              BIGINT        NOT NULL,
    [TemplateId]            BIGINT        NOT NULL,
    [TemplateSubject]       VARCHAR (255) NOT NULL DEFAULT (''),
    [TemplateContent]       VARCHAR (MAX) NOT NULL DEFAULT (''),
    [IsActive]              BIT           CONSTRAINT [DF_IntakeTemplates_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_IntakeTemplates_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_IntakeTemplates_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_IntakeTemplates_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_IntakeTemplates_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_IntakeTemplates_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IntakeTemplates] PRIMARY KEY CLUSTERED ([IntakeTemplateId] ASC)
);

