using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Auth.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_user_auth_username",
                table: "user_auth");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "user_auth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "user_auth",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "idx_user_auth_username",
                table: "user_auth",
                column: "UserName",
                unique: true);
        }
    }
}
