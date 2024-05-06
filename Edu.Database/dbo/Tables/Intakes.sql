CREATE TABLE [dbo].[Intakes] (
    [IntakeId]              BIGINT        IDENTITY (1, 1) NOT NULL,
    [IntakeName]            VARCHAR (100) NOT NULL,
    [SchemeId]              BIGINT        NOT NULL,
    [StartDate]             DATETIME      NOT NULL,
    [ExamDate]              DATETIME      NOT NULL,
    [NominationCutoffDate]  DATETIME      NOT NULL,
    [ScorecardCutoffDate]   DATETIME      NOT NULL,
    [IsGTSScoreUpload]      BIT           NULL,
    [BrochureFilePath]      VARCHAR (500) NULL,
    [BrochureFileName]      VARCHAR (255) NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Intakes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Intakes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Intakes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Intakes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Intakes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Intakes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Intakes] PRIMARY KEY CLUSTERED ([IntakeId] ASC)
);

