using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

[Migration(000008)]
public class Migration000008_AddFileChecksum : Migration
{
    public override void Up()
    {
        Alter.Table(Tables.File)
            .InSchema(Schemas.Library)
            // SHA-256 hex digest of the file's contents, computed when the file is stored.
            // Nullable: files stored before this column existed are left unbackfilled.
            .AddColumn("Checksum").AsString(64).Nullable();
    }

    public override void Down()
    {
        Delete.Column("Checksum")
            .FromTable(Tables.File)
            .InSchema(Schemas.Library);
    }
}
