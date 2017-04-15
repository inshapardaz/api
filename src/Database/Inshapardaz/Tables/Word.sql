CREATE TABLE [Inshapardaz].[Word] (
    [Id]                 BIGINT            NOT NULL IDENTITY,
    [Title]              NVARCHAR (MAX) NULL,
    [TitleWithMovements] NVARCHAR (MAX) NULL,
    [Description]        NVARCHAR (MAX) NULL,
    [Pronunciation]      NVARCHAR (MAX) NULL,
    [DictionaryId] INT NULL DEFAULT 1, 
    CONSTRAINT [PK_Word] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Word_Dictionary] FOREIGN KEY ([DictionaryId]) REFERENCES [Inshapardaz].[Dictionary](Id)
);


GO
