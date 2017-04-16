CREATE TABLE [Inshapardaz].[Dictionary]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(255) NULL, 
    [Language] INT NULL, 
    [IsPublic] BIT NULL, 
    [UserId] NVARCHAR(50) NULL
)
