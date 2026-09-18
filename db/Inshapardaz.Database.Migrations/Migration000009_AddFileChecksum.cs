using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

[Migration(000009)]
public class Migration000009_AddFileChecksum : Migration
{
    public override void Up()
    {
        // Nullable: existing rows have no checksum yet - it is backfilled lazily the first time
        // each file is read (see GetFileRequestHandler), rather than computed for every row here.
        Alter.Table(Tables.File).InSchema(Schemas.Library)
            .AddColumn("Checksum").AsString(64).Nullable();
    }

    public override void Down()
    {
        Delete.Column("Checksum").FromTable(Tables.File).InSchema(Schemas.Library);
    }
}
