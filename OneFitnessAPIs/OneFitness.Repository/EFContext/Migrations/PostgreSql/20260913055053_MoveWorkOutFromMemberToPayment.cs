using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneFitness.Repository.EFContext.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class MoveWorkOutFromMemberToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkOutId",
                table: "MemberRegistration");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOutId",
                table: "Payment",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WorkOutId",
                table: "Payment",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "WorkOutId",
                table: "MemberRegistration",
                type: "integer",
                nullable: true);
        }
    }
}
