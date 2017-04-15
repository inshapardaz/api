CREATE TABLE [Inshapardaz].[WordDetail] (
    [Id]             BIGINT    NOT NULL IDENTITY,
    [Attributes]     BIGINT NOT NULL,
    [WordInstanceId] BIGINT    NOT NULL,
    [Language]       INT    NULL, 
    CONSTRAINT [PK_WordDetail] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_WordDetail_Word] FOREIGN KEY ([WordInstanceId]) REFERENCES [Inshapardaz].[Word]([Id])
);

