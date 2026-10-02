using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneFitness.Repository.EFContext.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class MakeEnquiryMobileEmailReasonOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enquiry_EmailId",
                table: "Enquiry");

            migrationBuilder.DropIndex(
                name: "IX_Enquiry_MobileNo",
                table: "Enquiry");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "Enquiry",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "EmailId",
                table: "Enquiry",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Enquiry_EmailId",
                table: "Enquiry",
                column: "EmailId",
                unique: true,
                filter: "[EmailId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Enquiry_MobileNo",
                table: "Enquiry",
                column: "MobileNo",
                unique: true,
                filter: "[MobileNo] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enquiry_EmailId",
                table: "Enquiry");

            migrationBuilder.DropIndex(
                name: "IX_Enquiry_MobileNo",
                table: "Enquiry");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNo",
                table: "Enquiry",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailId",
                table: "Enquiry",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enquiry_EmailId",
                table: "Enquiry",
                column: "EmailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enquiry_MobileNo",
                table: "Enquiry",
                column: "MobileNo",
                unique: true);
        }
    }
}
