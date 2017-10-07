using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20170918112459_RemovedWordDictionaryDefault")]
    partial class RemovedWordDictionaryDefault
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("Inshapardaz")
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.Dictionary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsPublic");

                    b.Property<int>("Language");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<Guid>("UserId")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Dictionary","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.DictionaryDownload", b =>
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

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.File", b =>
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

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.Meaning", b =>
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

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.Translation", b =>
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

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.Word", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<int>("DictionaryId");

                    b.Property<string>("Pronunciation");

                    b.Property<string>("Title");

                    b.Property<string>("TitleWithMovements");

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.ToTable("Word","Inshapardaz");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.WordDetail", b =>
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

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.WordRelation", b =>
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

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.DictionaryDownload", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Database.Entities.Dictionary", "Dictionary")
                        .WithMany("Downloads")
                        .HasForeignKey("DictionaryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Inshapardaz.Domain.Database.Entities.File", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.Meaning", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Database.Entities.WordDetail", "WordDetail")
                        .WithMany("Meaning")
                        .HasForeignKey("WordDetailId")
                        .HasConstraintName("FK_Meaning_WordDetail")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.Translation", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Database.Entities.WordDetail", "WordDetail")
                        .WithMany("Translation")
                        .HasForeignKey("WordDetailId")
                        .HasConstraintName("FK_Translation_WordDetail")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.Word", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Database.Entities.Dictionary", "Dictionary")
                        .WithMany("Word")
                        .HasForeignKey("DictionaryId")
                        .HasConstraintName("FK_Word_Dictionary")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.WordDetail", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Database.Entities.Word", "WordInstance")
                        .WithMany("WordDetail")
                        .HasForeignKey("WordInstanceId")
                        .HasConstraintName("FK_WordDetail_Word")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Inshapardaz.Domain.Database.Entities.WordRelation", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Database.Entities.Word", "RelatedWord")
                        .WithMany("WordRelationRelatedWord")
                        .HasForeignKey("RelatedWordId")
                        .HasConstraintName("FK_WordRelation_RelatedWord")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Inshapardaz.Domain.Database.Entities.Word", "SourceWord")
                        .WithMany("WordRelationSourceWord")
                        .HasForeignKey("SourceWordId")
                        .HasConstraintName("FK_WordRelation_SourceWord");
                });
        }
    }
}
