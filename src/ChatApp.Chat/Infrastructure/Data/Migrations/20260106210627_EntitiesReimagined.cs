using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Chat.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesReimagined : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_messages_ReplyToMessageId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_ReplyToMessageId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_chat_participants_ChatId_UserId",
                table: "chat_participants");

            migrationBuilder.DropColumn(
                name: "ReplyToMessageId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "LeftAt",
                table: "chat_participants");

            migrationBuilder.CreateIndex(
                name: "IX_messages_ChatId_SentAt",
                table: "messages",
                columns: new[] { "ChatId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_chats_LastMessageId",
                table: "chats",
                column: "LastMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_participants_ChatId_UserId",
                table: "chat_participants",
                columns: new[] { "ChatId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_chats_messages_LastMessageId",
                table: "chats",
                column: "LastMessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_chats_ChatId",
                table: "messages",
                column: "ChatId",
                principalTable: "chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chats_messages_LastMessageId",
                table: "chats");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_chats_ChatId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_ChatId_SentAt",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_chats_LastMessageId",
                table: "chats");

            migrationBuilder.DropIndex(
                name: "IX_chat_participants_ChatId_UserId",
                table: "chat_participants");

            migrationBuilder.AddColumn<Guid>(
                name: "ReplyToMessageId",
                table: "messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LeftAt",
                table: "chat_participants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_ReplyToMessageId",
                table: "messages",
                column: "ReplyToMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_participants_ChatId_UserId",
                table: "chat_participants",
                columns: new[] { "ChatId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_messages_messages_ReplyToMessageId",
                table: "messages",
                column: "ReplyToMessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
