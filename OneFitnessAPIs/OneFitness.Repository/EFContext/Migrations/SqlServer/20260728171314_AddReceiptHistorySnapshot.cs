using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneFitness.Repository.EFContext.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class AddReceiptHistorySnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "ReceiptHistory",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstallmentId",
                table: "ReceiptHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MembershipTypeId",
                table: "ReceiptHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRenewalDate",
                table: "ReceiptHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentFromDate",
                table: "ReceiptHistory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTypeId",
                table: "ReceiptHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxId",
                table: "ReceiptHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "ReceiptHistory",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentageAmount",
                table: "ReceiptHistory",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "ReceiptHistory",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkOutId",
                table: "ReceiptHistory",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "InstallmentId",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "MembershipTypeId",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "NextRenewalDate",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "PaymentFromDate",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "PaymentTypeId",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "TaxPercentageAmount",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "ReceiptHistory");

            migrationBuilder.DropColumn(
                name: "WorkOutId",
                table: "ReceiptHistory");
        }
    }
}
