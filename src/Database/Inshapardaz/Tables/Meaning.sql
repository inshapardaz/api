CREATE TABLE [Inshapardaz].[Meaning] (
    [Id]           BIGINT            NOT NULL IDENTITY,
    [Context]      NVARCHAR (MAX) NULL,
    [Value]        NVARCHAR (MAX) NULL,
    [Example]      NVARCHAR (MAX) NULL,
    [WordDetailId] BIGINT            NOT NULL, 
    CONSTRAINT [PK_Meaning] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Meaning_WordDetail] FOREIGN KEY ([WordDetailId]) REFERENCES [Inshapardaz].[WordDetail]([Id])
);

