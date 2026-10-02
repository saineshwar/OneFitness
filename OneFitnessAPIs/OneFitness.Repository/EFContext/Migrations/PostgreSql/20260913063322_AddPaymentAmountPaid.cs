using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneFitness.Repository.EFContext.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddPaymentAmountPaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "Payment",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            // Existing payments predate partial-payment tracking; treat them as fully paid.
            migrationBuilder.Sql("UPDATE \"Payment\" SET \"AmountPaid\" = \"TotalAmount\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Payment");
        }
    }
}
