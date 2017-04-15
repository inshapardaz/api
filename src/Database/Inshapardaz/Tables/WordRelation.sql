CREATE TABLE [Inshapardaz].[WordRelation] (
    [Id]            BIGINT NOT NULL IDENTITY,
    [SourceWordId]  BIGINT NOT NULL,
    [RelatedWordId] BIGINT NOT NULL,
    [RelationType]  INT NOT NULL, 
    CONSTRAINT [PK_WordRelation] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_WordRelation_SourceWord] FOREIGN KEY ([SourceWordId]) REFERENCES [Inshapardaz].[Word]([Id]),
    CONSTRAINT [FK_WordRelation_RelatedWord] FOREIGN KEY ([RelatedWordId]) REFERENCES [Inshapardaz].[Word]([Id])
);

