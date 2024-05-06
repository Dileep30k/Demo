CREATE TABLE [dbo].[EmailAccounts] (
    [EmailAccountId]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [Email]                 VARCHAR (100) NOT NULL,
    [DisplayName]           VARCHAR (100) NOT NULL,
    [Host]                  VARCHAR (100) NOT NULL,
    [Port]                  INT           CONSTRAINT [DF_Table_1_IsKEM_1] DEFAULT ((0)) NOT NULL,
    [Username]              VARCHAR (100) CONSTRAINT [DF_Table_1_IsZEM] DEFAULT ((0)) NOT NULL,
    [Password]              VARCHAR (100) CONSTRAINT [DF_Table_1_IsActive] DEFAULT ((1)) NOT NULL,
    [EnableSsl]             BIT           NOT NULL,
    [UseDefaultCredentials] BIT           NOT NULL,
    [IsDefaultAccount]      BIT           NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_EmailAccounts_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_EmailAccounts_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_EmailAccounts_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_EmailAccounts_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_EmailAccounts_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_EmailAccounts_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EmailAccounts] PRIMARY KEY CLUSTERED ([EmailAccountId] ASC)
);

