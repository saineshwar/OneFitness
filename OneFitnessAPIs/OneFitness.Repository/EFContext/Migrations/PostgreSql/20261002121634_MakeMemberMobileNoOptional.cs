using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneFitness.Repository.EFContext.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class MakeMemberMobileNoOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberRegistration_MobileNo",
                table: "MemberRegistration");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "MemberRegistration",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateIndex(
                name: "IX_MemberRegistration_MobileNo",
                table: "MemberRegistration",
                column: "MobileNo",
                unique: true,
                filter: "[MobileNo] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberRegistration_MobileNo",
                table: "MemberRegistration");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "MemberRegistration",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberRegistration_MobileNo",
                table: "MemberRegistration",
                column: "MobileNo",
                unique: true);
        }
    }
}
