using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inshapardaz.Domain.Migrations
{
    public partial class CovertedUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "Inshapardaz",
                table: "Dictionary",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Inshapardaz",
                table: "Dictionary",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(Guid),
                oldMaxLength: 50);
        }
    }
}
