
DROP TABLE IF EXISTS [dbo].[Rentals];
DROP TABLE IF EXISTS [dbo].[Orders];
DROP TABLE IF EXISTS [dbo].[BoardGames];
DROP TABLE IF EXISTS [dbo].[Clients];
DROP TABLE IF EXISTS [dbo].[Users];
DROP TABLE IF EXISTS [dbo].[Categories];
GO


CREATE TABLE [dbo].[Categories] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (150) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
);


CREATE TABLE [dbo].[Users] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Login]        NVARCHAR (100) NOT NULL,
    [PasswordHash] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Users_Login] UNIQUE NONCLUSTERED ([Login] ASC)
);


CREATE TABLE [dbo].[Clients] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [FullName]     NVARCHAR (250) NOT NULL,
    [Phone]        NVARCHAR (20)  NOT NULL,
    [Email]        NVARCHAR (150) NOT NULL,
    [PassportData] NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([Id] ASC)
);


CREATE TABLE [dbo].[BoardGames] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [Title]           NVARCHAR (200)  NOT NULL,
    [PriceSale]       DECIMAL (18, 2) NOT NULL,
    [PriceRentPerDay] DECIMAL (18, 2) NOT NULL,
    [DepositAmount]   DECIMAL (18, 2) NOT NULL,
    [CategoryId]      INT             NOT NULL,
    CONSTRAINT [PK_BoardGames] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BoardGames_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]) ON DELETE CASCADE
);


CREATE TABLE [dbo].[Orders] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [BoardGameId] INT             NOT NULL,
    [ClientId]    INT             NOT NULL,
    [OrderDate]   DATETIME        DEFAULT (getdate()) NOT NULL,
    [SalePrice]   DECIMAL (18, 2) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders_BoardGames] FOREIGN KEY ([BoardGameId]) REFERENCES [dbo].[BoardGames] ([Id]),
    CONSTRAINT [FK_Orders_Clients] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id])
);


CREATE TABLE [dbo].[Rentals] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [BoardGameId]        INT             NOT NULL,
    [ClientId]           INT             NOT NULL,
    [IssueDate]          DATETIME        DEFAULT (getdate()) NOT NULL,
    [PlannedReturnDate]  DATETIME        NOT NULL,
    [TotalPrice]         DECIMAL (18, 2) NOT NULL,
    [DepositPaid]        DECIMAL (18, 2) NOT NULL,
    [Status]             NVARCHAR (50)   DEFAULT (N'Активен') NOT NULL,
    CONSTRAINT [PK_Rentals] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Rentals_BoardGames] FOREIGN KEY ([BoardGameId]) REFERENCES [dbo].[BoardGames] ([Id]),
    CONSTRAINT [FK_Rentals_Clients] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Clients] ([Id])
);
GO
