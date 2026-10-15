using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDay.API.Data.IdentityMigrations
{
    /// <inheritdoc />
    public partial class AddIdentityProfileForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
       name: "FK_AspNetUsers_Admins_AdminID",
       table: "AspNetUsers",
       column: "AdminID",
       principalTable: "Admins",
       principalColumn: "AdminID",
       onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Participants_ParticipantID",
                table: "AspNetUsers",
                column: "ParticipantID",
                principalTable: "Participants",
                principalColumn: "ParticipantID",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
       name: "FK_AspNetUsers_Admins_AdminID",
       table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Participants_ParticipantID",
                table: "AspNetUsers");
        }
    }
}
