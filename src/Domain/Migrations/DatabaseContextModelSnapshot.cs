using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Inshapardaz.Domain;

namespace Inshapardaz.Domain.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Inshapardaz.Domain.Model.AggregatedCounter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ExpireAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<long>("Value");

                    b.HasKey("Id");

                    b.HasIndex("Value", "Key")
                        .IsUnique()
                        .HasName("UX_HangFire_CounterAggregated_Key");

                    b.ToTable("AggregatedCounter","HangFire");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Counter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ExpireAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<short>("Value");

                    b.HasKey("Id");

                    b.HasIndex("Value", "Key")
                        .HasName("IX_HangFire_Counter_Key");

                    b.ToTable("Counter","HangFire");
                });

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

                    b.HasKey("Id");

                    b.HasIndex("DictionaryId");

                    b.HasIndex("FileId");

                    b.ToTable("DictionaryDownload");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Contents");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("LiveUntil");

                    b.Property<string>("MimeType");

                    b.HasKey("Id");

                    b.ToTable("File");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Hash", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ExpireAt");

                    b.Property<string>("Field")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ExpireAt", "Key")
                        .HasName("IX_HangFire_Hash_Key");

                    b.HasIndex("Id", "ExpireAt")
                        .HasName("IX_HangFire_Hash_ExpireAt");

                    b.HasIndex("Key", "Field")
                        .IsUnique()
                        .HasName("UX_HangFire_Hash_Key_Field");

                    b.ToTable("Hash","HangFire");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Job", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Arguments")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("ExpireAt")
                        .HasColumnType("datetime");

                    b.Property<string>("InvocationData")
                        .IsRequired();

                    b.Property<int?>("StateId");

                    b.Property<string>("StateName")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("StateName")
                        .HasName("IX_HangFire_Job_StateName");

                    b.HasIndex("Id", "ExpireAt")
                        .HasName("IX_HangFire_Job_ExpireAt");

                    b.ToTable("Job","HangFire");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.JobParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("JobId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("JobId", "Name")
                        .HasName("IX_HangFire_JobParameter_JobIdAndName");

                    b.ToTable("JobParameter","HangFire");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.JobQueue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("FetchedAt")
                        .HasColumnType("datetime");

                    b.Property<int>("JobId");

                    b.Property<string>("Queue")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("Queue", "FetchedAt")
                        .HasName("IX_HangFire_JobQueue_QueueAndFetchedAt");

                    b.ToTable("JobQueue","HangFire");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.List", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ExpireAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("Id", "ExpireAt")
                        .HasName("IX_HangFire_List_ExpireAt");

                    b.HasIndex("ExpireAt", "Value", "Key")
                        .HasName("IX_HangFire_List_Key");

                    b.ToTable("List","HangFire");
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

            modelBuilder.Entity("Inshapardaz.Domain.Model.Schema", b =>
                {
                    b.Property<int>("Version");

                    b.HasKey("Version")
                        .HasName("PK_HangFire_Schema");

                    b.ToTable("Schema","HangFire");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Server", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.Property<string>("Data");

                    b.Property<DateTime>("LastHeartbeat")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.ToTable("Server","HangFire");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Set", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ExpireAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<double>("Score");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("Id", "ExpireAt")
                        .HasName("IX_HangFire_Set_ExpireAt");

                    b.HasIndex("Key", "Value")
                        .IsUnique()
                        .HasName("UX_HangFire_Set_KeyAndValue");

                    b.HasIndex("ExpireAt", "Value", "Key")
                        .HasName("IX_HangFire_Set_Key");

                    b.ToTable("Set","HangFire");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.State", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Data");

                    b.Property<int>("JobId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("Reason")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("JobId")
                        .HasName("IX_HangFire_State_JobId");

                    b.ToTable("State","HangFire");
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

            modelBuilder.Entity("Inshapardaz.Domain.Model.JobParameter", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.Job", "Job")
                        .WithMany("JobParameter")
                        .HasForeignKey("JobId")
                        .HasConstraintName("FK_HangFire_JobParameter_Job")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.Meaning", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.WordDetail", "WordDetail")
                        .WithMany("Meaning")
                        .HasForeignKey("WordDetailId")
                        .HasConstraintName("FK_Meaning_WordDetail");
                });

            modelBuilder.Entity("Inshapardaz.Domain.Model.State", b =>
                {
                    b.HasOne("Inshapardaz.Domain.Model.Job", "Job")
                        .WithMany("State")
                        .HasForeignKey("JobId")
                        .HasConstraintName("FK_HangFire_State_Job")
                        .OnDelete(DeleteBehavior.Cascade);
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
