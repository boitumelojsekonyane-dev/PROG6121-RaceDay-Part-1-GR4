using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDay.API.Data.IdentityMigrations
{
    /// <inheritdoc />
    public partial class AddIdentityProfileConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AdminID",
                table: "AspNetUsers",
                column: "AdminID",
                unique: true,
                filter: "[AdminID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ParticipantID",
                table: "AspNetUsers",
                column: "ParticipantID",
                unique: true,
                filter: "[ParticipantID] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AspNetUsers_ProfileLink",
                table: "AspNetUsers",
                sql: "([ParticipantID] IS NOT NULL AND [AdminID] IS NULL) OR ([ParticipantID] IS NULL AND [AdminID] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AdminID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ParticipantID",
                table: "AspNetUsers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AspNetUsers_ProfileLink",
                table: "AspNetUsers");
        }
    }
}
