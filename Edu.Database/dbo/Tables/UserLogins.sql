CREATE TABLE [dbo].[UserLogins] (
    [UserLoginId]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [Email]           VARCHAR (50)  NOT NULL,
    [SessionId]       VARCHAR (100) NOT NULL,
    [LogInExpireTime] DATETIME      NOT NULL,
    [IsActive]        BIT           CONSTRAINT [DF_UserLogins_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]     DATETIME      CONSTRAINT [DF_UserLogins_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]       BIGINT        NOT NULL,
    [UpdatedDate]     DATETIME      CONSTRAINT [DF_UserLogins_UpdatedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]       BIGINT        NOT NULL,
    [IsDeleted]       BIT           CONSTRAINT [DF_UserLogins_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED ([UserLoginId] ASC)
);

