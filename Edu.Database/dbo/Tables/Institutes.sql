CREATE TABLE [dbo].[Institutes] (
    [InstituteId]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [InstituteName]         VARCHAR (100) NOT NULL,
    [InstituteCode]         VARCHAR (50) NOT NULL,
    [EmailAddress]          VARCHAR (100) NULL,
    [ContactNo]             VARCHAR (50)  NULL,
    [ContactPerson]         VARCHAR (100) NULL,
    [Address]               VARCHAR (255) NULL,
    [Pincode]               VARCHAR (50) NULL,
    [City]                  VARCHAR (50) NULL,
    [State]                 VARCHAR (50) NULL,
    [Country]               VARCHAR (50) NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Institutes_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Institutes_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Institutes_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Institutes_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Institutes_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Institutes_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Institutes] PRIMARY KEY CLUSTERED ([InstituteId] ASC)
);

