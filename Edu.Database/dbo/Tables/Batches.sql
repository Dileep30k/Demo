CREATE TABLE [dbo].[Batches] (
    [BatchId]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [BatchCode]             VARCHAR (50)  NOT NULL,
    [SchemeId]              BIGINT        NOT NULL,
    [IntakeId]              BIGINT        NOT NULL,
    [InstituteId]           BIGINT        NOT NULL,
    [AdmissionId]           BIGINT        NOT NULL,
    [StartDate]             DATETIME      NOT NULL,
    [TotalSeats]            INT           NOT NULL,
    [TotalFee]              INT           NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Batches_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Batches_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Batches_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Batches_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Batches_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Batches_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Batches] PRIMARY KEY CLUSTERED ([BatchId] ASC)
);

