using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OneFitness.Repository.EFContext.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class AddPaymentDropMemberPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<long>(type: "bigint", nullable: false),
                    WorkOutId = table.Column<int>(type: "int", nullable: true),
                    MembershipTypeId = table.Column<int>(type: "int", nullable: false),
                    InstallmentId = table.Column<int>(type: "int", nullable: false),
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxPercentageAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceNo = table.Column<long>(type: "bigint", nullable: false),
                    PaymentFromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextRenewalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payment_MemberRegistration_MemberId",
                        column: x => x.MemberId,
                        principalTable: "MemberRegistration",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_MemberId",
                table: "Payment",
                column: "MemberId");

            // Backfill: every existing member's current payment/fee snapshot becomes their one Payment row
            // before the equivalent columns are dropped from MemberRegistration below. Only members with a
            // complete snapshot (all legacy payment fields populated) are migrated; a member is skipped if
            // any field is null, since a partial row cannot be turned into a usable Payment.
            migrationBuilder.Sql(@"
                INSERT INTO [Payment] (MemberId, WorkOutId, MembershipTypeId, InstallmentId, PaymentTypeId, TaxId, Amount, TaxPercentage, TaxPercentageAmount, TotalAmount, InvoiceNo, PaymentFromDate, NextRenewalDate, CreatedOn, CreatedBy)
                SELECT
                    m.MemberId,
                    m.WorkOutId,
                    m.MembershipTypeId,
                    m.InstallmentId,
                    m.PaymentTypeId,
                    m.TaxId,
                    m.Amount,
                    m.TaxPercentage,
                    m.TaxPercentageAmount,
                    m.TotalAmount,
                    m.InvoiceNo,
                    DATEADD(month, -ISNULL(i.InstallmentMonths, 0), m.NextRenewalDate),
                    m.NextRenewalDate,
                    COALESCE(m.ModifiedOn, m.CreatedOn),
                    m.CreatedBy
                FROM [MemberRegistration] m
                LEFT JOIN [Installments] i ON i.InstallmentId = m.InstallmentId
                WHERE m.MembershipTypeId IS NOT NULL
                  AND m.InstallmentId IS NOT NULL
                  AND m.PaymentTypeId IS NOT NULL
                  AND m.TaxId IS NOT NULL
                  AND m.Amount IS NOT NULL
                  AND m.TaxPercentage IS NOT NULL
                  AND m.TaxPercentageAmount IS NOT NULL
                  AND m.TotalAmount IS NOT NULL
                  AND m.InvoiceNo IS NOT NULL
                  AND m.NextRenewalDate IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "InstallmentId",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "MembershipTypeId",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "NextRenewalDate",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "PaymentTypeId",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "TaxPercentageAmount",
                table: "MemberRegistration");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "MemberRegistration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "MemberRegistration",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstallmentId",
                table: "MemberRegistration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceNo",
                table: "MemberRegistration",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MembershipTypeId",
                table: "MemberRegistration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRenewalDate",
                table: "MemberRegistration",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTypeId",
                table: "MemberRegistration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxId",
                table: "MemberRegistration",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                table: "MemberRegistration",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentageAmount",
                table: "MemberRegistration",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "MemberRegistration",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
