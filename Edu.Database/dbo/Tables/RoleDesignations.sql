CREATE TABLE [dbo].[RoleDesignations] (
    [RoleDesignationId]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [RoleId]                BIGINT        NOT NULL,
    [DesignationId]         BIGINT        NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_RoleDesignations_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_RoleDesignations_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_RoleDesignations_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_RoleDesignations_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_RoleDesignations_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_RoleDesignations_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_RoleDesignations] PRIMARY KEY CLUSTERED ([RoleDesignationId] ASC)
);

