using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Data.Migrations;

/// <inheritdoc />
public partial class FixCommentEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Comments_AspNetUsers_SenderId",
            table: "Comments");

        migrationBuilder.DropForeignKey(
            name: "FK_Comments_Messages_InitialMessageId",
            table: "Comments");

        migrationBuilder.AddForeignKey(
            name: "FK_Comments_AspNetUsers_SenderId",
            table: "Comments",
            column: "SenderId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Comments_Messages_InitialMessageId",
            table: "Comments",
            column: "InitialMessageId",
            principalTable: "Messages",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Comments_AspNetUsers_SenderId",
            table: "Comments");

        migrationBuilder.DropForeignKey(
            name: "FK_Comments_Messages_InitialMessageId",
            table: "Comments");

        migrationBuilder.AddForeignKey(
            name: "FK_Comments_AspNetUsers_SenderId",
            table: "Comments",
            column: "SenderId",
            principalTable: "AspNetUsers",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Comments_Messages_InitialMessageId",
            table: "Comments",
            column: "InitialMessageId",
            principalTable: "Messages",
            principalColumn: "Id");
    }
}