CREATE TABLE [dbo].[Departments] (
    [DepartmentId]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [DepartmentName]        VARCHAR (100) NOT NULL,
    [IsActive]              BIT           CONSTRAINT [DF_Departments_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedDate]           DATETIME      CONSTRAINT [DF_Departments_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]             BIGINT        CONSTRAINT [DF_Departments_CreatedBy] DEFAULT ((1)) NOT NULL,
    [UpdatedDate]           DATETIME      CONSTRAINT [DF_Departments_ModifiedDate] DEFAULT (getdate()) NOT NULL,
    [UpdatedBy]             BIGINT        CONSTRAINT [DF_Departments_ModifiedBy] DEFAULT ((1)) NOT NULL,
    [IsDeleted]             BIT           CONSTRAINT [DF_Departments_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED ([DepartmentId] ASC)
);

