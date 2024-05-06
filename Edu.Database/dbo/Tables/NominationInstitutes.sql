CREATE TABLE [dbo].[NominationInstitutes] (
    [NominationInstituteId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [NominationId]          BIGINT        NOT NULL,
    [InstituteId]           BIGINT        NOT NULL,
    [IsExamTaken]           BIT           NULL,
    [Rank]                  DECIMAL(5,2)  NULL,
    [Score]                 DECIMAL(5,2)  NULL,
    [IsActive]              BIT           CONSTRAINT [DF_NominationInstitutes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_NominationInstitutes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_NominationInstitutes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_NominationInstitutes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_NominationInstitutes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_NominationInstitutes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_NominationInstitutes] PRIMARY KEY CLUSTERED ([NominationInstituteId] ASC)
);

