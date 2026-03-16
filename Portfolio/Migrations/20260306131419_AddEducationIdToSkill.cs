using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Migrations
{
    /// <inheritdoc />
    public partial class AddEducationIdToSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Educations_EducationsId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_EducationsId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "EducationsId",
                table: "Skills");

            migrationBuilder.AddColumn<int>(
                name: "EducationId",
                table: "Skills",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_EducationId",
                table: "Skills",
                column: "EducationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Educations_EducationId",
                table: "Skills",
                column: "EducationId",
                principalTable: "Educations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Educations_EducationId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_EducationId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "EducationId",
                table: "Skills");

            migrationBuilder.AddColumn<int>(
                name: "EducationsId",
                table: "Skills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_EducationsId",
                table: "Skills",
                column: "EducationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Educations_EducationsId",
                table: "Skills",
                column: "EducationsId",
                principalTable: "Educations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
