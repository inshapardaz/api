using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Database;

namespace Inshapardaz.Domain.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20170823184020_AddedFileProperties")]
    partial class AddedFileProperties
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("Inshapardaz")
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Inshapardaz.Domain.Model.Dictionary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsPublic");

                    b.Property<int>("Language");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("UserId")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Dictionary","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.DictionaryDownload", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DictionaryId");

                    b.Property<int>("FileId");

                    b.Property<string>("MimeType");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.HasIndex("FileId");

                    b.ToTable("DictionaryDownload","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Contents");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("FileName");

                    b.Property<DateTime>("LiveUntil");

                    b.Property<string>("MimeType");

                    b.HasKey("Id");

                    b.ToTable("File","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Meaning", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Context");

                    b.Property<string>("Example");

                    b.Property<string>("Value");

                    b.Property<long>("WordDetailId");

                    b.HasKey("Id");

                    b.HasIndex("WordDetailId");

                    b.ToTable("Meaning","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Translation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Language");

                    b.Property<string>("Value");

                    b.Property<long>("WordDetailId");

                    b.HasKey("Id");

                    b.HasIndex("WordDetailId");

                    b.ToTable("Translation","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Word", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<int>("DictionaryId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("1");

                    b.Property<string>("Pronunciation");

                    b.Property<string>("Title");

                    b.Property<string>("TitleWithMovements");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.ToTable("Word","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.WordDetail", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("Attributes");

                    b.Property<int>("Language");

                    b.Property<long>("WordInstanceId");

                    b.HasKey("Id");

                    b.HasIndex("WordInstanceId");

                    b.ToTable("WordDetail","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.WordRelation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("RelatedWordId");

                    b.Property<int>("RelationType");

                    b.Property<long>("SourceWordId");

                    b.HasKey("Id");

                    b.HasIndex("RelatedWordId");

                    b.HasIndex("SourceWordId");

                    b.ToTable("WordRelation","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.DictionaryDownload", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.Dictionary", "Dictionary")
                        .WithMany("Downloads")
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Inshapardaz.Domain.Model.File", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Meaning", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.WordDetail", "WordDetail")
                        .WithMany("Meaning")
                        .HasForeignKey("WordDetailId")
                        .HasConstraintName("FK_Meaning_WordDetail");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Translation", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.WordDetail", "WordDetail")
                        .WithMany("Translation")
                        .HasForeignKey("WordDetailId")
                        .HasConstraintName("FK_Translation_WordDetail");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Word", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.Dictionary", "Dictionary")
                        .WithMany("Word")
                        .HasForeignKey("DictionaryId")
                        .HasConstraintName("FK_Word_Dictionary")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.WordDetail", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.Word", "WordInstance")
                        .WithMany("WordDetail")
                        .HasForeignKey("WordInstanceId")
                        .HasConstraintName("FK_WordDetail_Word");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.WordRelation", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.Word", "RelatedWord")
                        .WithMany("WordRelationRelatedWord")
                        .HasForeignKey("RelatedWordId")
                        .HasConstraintName("FK_WordRelation_RelatedWord");

                    b.HasOne("Inshapardaz.Domain.Model.Word", "SourceWord")
                        .WithMany("WordRelationSourceWord")
                        .HasForeignKey("SourceWordId")
                        .HasConstraintName("FK_WordRelation_SourceWord");
                });
        }
    }
}
