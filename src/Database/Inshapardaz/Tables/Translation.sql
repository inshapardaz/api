CREATE TABLE [Inshapardaz].[Translation] (
    [Id]       BIGINT            NOT NULL IDENTITY,
    [Language] INT            NOT NULL,
    [Value]    NVARCHAR (MAX) NULL,
    [WordDetailId] BIGINT            NOT NULL, 
    CONSTRAINT [PK_Translation] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Translation_WordDetail] FOREIGN KEY ([WordDetailId]) REFERENCES [Inshapardaz].[WordDetail]([Id])
);

