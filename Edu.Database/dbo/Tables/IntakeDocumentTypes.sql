CREATE TABLE [dbo].[IntakeDocumentTypes] (
    [IntakeDocumentTypeId]   BIGINT        IDENTITY (1, 1) NOT NULL,
    [IntakeId]               BIGINT        NOT NULL,
    [DocumentTypeId]        BIGINT        NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_IntakeDocumentTypes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_IntakeDocumentTypes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_IntakeDocumentTypes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_IntakeDocumentTypes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_IntakeDocumentTypes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_IntakeDocumentTypes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IntakeDocumentTypes] PRIMARY KEY CLUSTERED ([IntakeDocumentTypeId] ASC)
);

