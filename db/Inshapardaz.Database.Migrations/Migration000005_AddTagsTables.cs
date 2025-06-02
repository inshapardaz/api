using FluentMigrator;

namespace Inshapardaz.Database.Migrations;

[Migration(000005)]
public class Migration000005_AddTagsTables : Migration
{
    public override void Up()
    {
        // Tag table
        Create.Table("Tag").InSchema(Schemas.Library)
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("LibraryId").AsInt32().NotNullable();
        
        Create.UniqueConstraint("UNQ_Library_Tag_Name")
            .OnTable("Tag").WithSchema(Schemas.Library)
            .Columns("LibraryId", "Name");

        Create.ForeignKey("FK_Tag_Library")
            .FromTable("Tag").InSchema(Schemas.Library).ForeignColumn("LibraryId")
            .ToTable("Library").InSchema(Schemas.Library).PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        
        // BookTag join table
        Create.Table("BookTag").InSchema(Schemas.Library)
            .WithColumn("BookId").AsInt32().NotNullable()
            .WithColumn("TagId").AsInt32().NotNullable();

        Create.PrimaryKey("PK_BookTag")
            .OnTable("BookTag").WithSchema(Schemas.Library)
            .Columns("BookId", "TagId");
        
        Create.ForeignKey("FK_BookTag_Book")
            .FromTable("BookTag").InSchema(Schemas.Library)
                .ForeignColumn("BookId")
            .ToTable("Book").InSchema(Schemas.Library)
                .PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade); 
        
        Create.ForeignKey("FK_BookTag_Tag")
            .FromTable("BookTag").InSchema(Schemas.Library)
                .ForeignColumn("TagId")
            .ToTable("Tag").InSchema(Schemas.Library)
                .PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        

        // PeriodicalTag join table
        Create.Table("PeriodicalTag").InSchema(Schemas.Library)
            .WithColumn("PeriodicalId").AsInt32().NotNullable()
            .WithColumn("TagId").AsInt32().NotNullable();

        Create.PrimaryKey("PK_PeriodicalTag")
            .OnTable("PeriodicalTag").WithSchema(Schemas.Library)
            .Columns("PeriodicalId", "TagId");

        Create.ForeignKey("FK_PeriodicalTag_Periodical")
            .FromTable("PeriodicalTag").InSchema(Schemas.Library)
            .ForeignColumn("PeriodicalId")
            .ToTable("Periodical").InSchema(Schemas.Library)
            .PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_PeriodicalTag_Tag")
            .FromTable("PeriodicalTag").InSchema(Schemas.Library)
            .ForeignColumn("TagId")
            .ToTable("Tag").InSchema(Schemas.Library)
            .PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        
        // IssueTag join table
        Create.Table("IssueTag").InSchema(Schemas.Library)
            .WithColumn("IssueId").AsInt32().NotNullable()
            .WithColumn("TagId").AsInt32().NotNullable();

        Create.PrimaryKey("PK_IssueTag")
            .OnTable("IssueTag").WithSchema(Schemas.Library)
            .Columns("IssueId", "TagId");

        Create.ForeignKey("FK_IssueTag_Issue")
            .FromTable("IssueTag").InSchema(Schemas.Library)
            .ForeignColumn("IssueId")
            .ToTable("Issue").InSchema(Schemas.Library)
            .PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_IssueTag_Tag")
            .FromTable("IssueTag").InSchema(Schemas.Library)
            .ForeignColumn("TagId")
            .ToTable("Tag").InSchema(Schemas.Library)
            .PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        
        // ArticleTag join table
        Create.Table("ArticleTag")
            .InSchema(Schemas.Library)
            .WithColumn("ArticleId").AsInt64().NotNullable()
            .WithColumn("TagId").AsInt32().NotNullable();

        Create.PrimaryKey("PK_ArticleTag")
            .OnTable("ArticleTag").WithSchema(Schemas.Library)
            .Columns("ArticleId", "TagId");
        
        Create.ForeignKey("FK_ArticleTag_Article")
            .FromTable("ArticleTag").InSchema(Schemas.Library)
                .ForeignColumn("ArticleId")
            .ToTable("Article").InSchema(Schemas.Library)
                .PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        
        Create.ForeignKey("FK_ArticleTag_Tag")
            .FromTable("ArticleTag").InSchema(Schemas.Library)
                .ForeignColumn("TagId")
            .ToTable("Tag").InSchema(Schemas.Library)
                .PrimaryColumn("Id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
    }

    public override void Down()
    {
        Delete.Table("IssueTag").InSchema(Schemas.Library);
        Delete.Table("PeriodicalTag").InSchema(Schemas.Library);
        Delete.Table("ArticleTag").InSchema(Schemas.Library);
        Delete.Table("BookTag").InSchema(Schemas.Library);
        Delete.Table("Tag").InSchema(Schemas.Library);
    }
}
