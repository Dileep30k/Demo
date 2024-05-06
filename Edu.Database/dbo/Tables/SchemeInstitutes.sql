CREATE TABLE [dbo].[SchemeInstitutes] (
    [SchemeInstituteId]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [SchemeId]              BIGINT        NOT NULL,
    [InstituteId]           BIGINT        NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_SchemeInstitutes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_SchemeInstitutes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_SchemeInstitutes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_SchemeInstitutes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_SchemeInstitutes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_SchemeInstitutes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SchemeInstitutes] PRIMARY KEY CLUSTERED ([SchemeInstituteId] ASC)
);

