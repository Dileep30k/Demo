CREATE TABLE [dbo].[BatchUsers] (
    [BatchUserId]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [BatchId]               BIGINT        NOT NULL,
    [UserId]                BIGINT        NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_BatchUsers_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_BatchUsers_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_BatchUsers_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_BatchUsers_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_BatchUsers_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_BatchUsers_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_BatchUsers] PRIMARY KEY CLUSTERED ([BatchUserId] ASC)
);

