-- ==========================================================
-- Drop ALL Foreign Key Constraints (to avoid dependency issues)
-- ==========================================================
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += N'ALTER TABLE [' + s.name + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + '];'
FROM sys.foreign_keys AS fk
INNER JOIN sys.tables AS t ON fk.parent_object_id = t.object_id
INNER JOIN sys.schemas AS s ON t.schema_id = s.schema_id;

EXEC sp_executesql @sql;
GO


-- ==========================================================
-- DROP existing tables in dependency order (child → parent)
-- ==========================================================
IF OBJECT_ID('[dbo].[AutoPartVehicles]', 'U') IS NOT NULL DROP TABLE [dbo].[AutoPartVehicles];
IF OBJECT_ID('[dbo].[AutoPartBrands]', 'U') IS NOT NULL DROP TABLE [dbo].[AutoPartBrands];
IF OBJECT_ID('[dbo].[Vehicles]', 'U') IS NOT NULL DROP TABLE [dbo].[Vehicles];
IF OBJECT_ID('[dbo].[AutoParts]', 'U') IS NOT NULL DROP TABLE [dbo].[AutoParts];
IF OBJECT_ID('[dbo].[Brands]', 'U') IS NOT NULL DROP TABLE [dbo].[Brands];
IF OBJECT_ID('[dbo].[Categories]', 'U') IS NOT NULL DROP TABLE [dbo].[Categories];
If OBJECT_ID('[dbo].[UserRoles]', 'U') IS NOT NULL DROP TABLE [dbo].[UserRoles];
IF OBJECT_ID('[dbo].[Users]', 'U') IS NOT NULL DROP TABLE [dbo].[Users];
If OBJECT_ID('[dbo].[Roles]', 'U') IS NOT NULL DROP TABLE [dbo].[Roles];

CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1),
    [Email] NVARCHAR(256) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [FirstName] NVARCHAR(100),
    [LastName] NVARCHAR(100),
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_Users PRIMARY KEY ([Id]),
    CONSTRAINT UQ_Users_Email UNIQUE ([Email])
);

CREATE TABLE [dbo].[Roles] (
    [Id] INT IDENTITY(1,1),
    [Name] NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_Roles PRIMARY KEY ([Id]),
    CONSTRAINT UQ_Roles_Name UNIQUE ([Name])
);

CREATE TABLE [dbo].[UserRoles] (
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,

    CONSTRAINT PK_UserRoles PRIMARY KEY ([UserId], [RoleId]),

    CONSTRAINT FK_UserRoles_Users 
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,

    CONSTRAINT FK_UserRoles_Roles 
        FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Categories] (
    [CategoryId] INT IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [ImageURL] NVARCHAR(500),

    CONSTRAINT PK_Categories PRIMARY KEY ([CategoryId]),
    CONSTRAINT UQ_Categories_Name UNIQUE ([Name])
);

CREATE TABLE [dbo].[Brands] (
    [BrandId] INT IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [ImageURL] NVARCHAR(500),

    CONSTRAINT PK_Brands PRIMARY KEY ([BrandId]),
    CONSTRAINT UQ_Brands_Name UNIQUE ([Name])
);

CREATE TABLE [dbo].[AutoParts] (
    [AutoPartId] INT IDENTITY(1,1),
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX),
    [ImageURL] NVARCHAR(500),
    [CategoryId] INT NOT NULL,
    [Cost] DECIMAL(10,2) NOT NULL,
    [Price] DECIMAL(10,2) NOT NULL,
    [Location] NVARCHAR(100),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_AutoParts PRIMARY KEY ([AutoPartId]),

    CONSTRAINT FK_AutoParts_Categories 
        FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([CategoryId]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Vehicles] (
    [VehicleId] INT IDENTITY(1,1),
    [BrandId] INT NOT NULL,
    [Model] NVARCHAR(100) NOT NULL,
    [StartYear] INT NOT NULL,
    [EndYear] INT NULL,

    CONSTRAINT PK_Vehicles PRIMARY KEY ([VehicleId]),

    CONSTRAINT FK_Vehicles_Brands
        FOREIGN KEY ([BrandId]) REFERENCES [Brands]([BrandId]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[AutoPartBrands] (
    [AutoPartId] INT NOT NULL,
    [BrandId] INT NOT NULL,

    CONSTRAINT PK_AutoPartBrands PRIMARY KEY ([AutoPartId], [BrandId]),

    CONSTRAINT FK_AutoPartBrands_Parts
        FOREIGN KEY ([AutoPartId]) REFERENCES [AutoParts]([AutoPartId]) ON DELETE CASCADE,

    CONSTRAINT FK_AutoPartBrands_Brands
        FOREIGN KEY ([BrandId]) REFERENCES [Brands]([BrandId]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[AutoPartVehicles] (
    [AutoPartId] INT NOT NULL,
    [VehicleId] INT NOT NULL,

    CONSTRAINT PK_AutoPartVehicles PRIMARY KEY ([AutoPartId], [VehicleId]),

    CONSTRAINT FK_AutoPartVehicles_Parts
        FOREIGN KEY ([AutoPartId]) REFERENCES [AutoParts]([AutoPartId]) ON DELETE CASCADE,

    CONSTRAINT FK_AutoPartVehicles_Vehicles
        FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles]([VehicleId]) ON DELETE CASCADE
);

-- Insert default roles
INSERT INTO [Roles] ([Name])
VALUES ('staff'), ('admin');







