CREATE TABLE [dbo].[IntakeInstitutes] (
    [IntakeInstituteId]     BIGINT       IDENTITY (1, 1) NOT NULL,
    [IntakeId]              BIGINT       NOT NULL,
    [InstituteId]           BIGINT        NOT NULL,
    [TotalSeats]            INT           NOT NULL,
    [AdmissionCutoffDate]   DATETIME      NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_IntakeInstitutes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_IntakeInstitutes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_IntakeInstitutes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_IntakeInstitutes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_IntakeInstitutes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_IntakeInstitutes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IntakeInstitutes] PRIMARY KEY CLUSTERED ([IntakeInstituteId] ASC)
);

