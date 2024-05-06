CREATE TABLE [dbo].[IntakeLocations] (
    [IntakeLocationId]      BIGINT        IDENTITY (1, 1) NOT NULL,
    [IntakeId]               BIGINT        NOT NULL,
    [LocationId]           BIGINT        NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_IntakeLocations_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_IntakeLocations_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_IntakeLocations_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_IntakeLocations_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_IntakeLocations_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_IntakeLocations_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IntakeLocations] PRIMARY KEY CLUSTERED ([IntakeLocationId] ASC)
);

