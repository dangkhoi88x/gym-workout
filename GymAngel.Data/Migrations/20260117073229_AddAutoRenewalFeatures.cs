using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymAngel.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoRenewalFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoRenewal",
                table: "MembershipTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationDate",
                table: "MembershipTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "MembershipTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GracePeriodEnd",
                table: "MembershipTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GracePeriodStart",
                table: "MembershipTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInGracePeriod",
                table: "MembershipTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRenewalAttempt",
                table: "MembershipTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRenewalDate",
                table: "MembershipTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RenewalAttempts",
                table: "MembershipTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MembershipNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MembershipTransactionId = table.Column<int>(type: "int", nullable: true),
                    NotificationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailSubject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    LastRetryDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembershipNotifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembershipNotifications_MembershipTransactions_MembershipTransactionId",
                        column: x => x.MembershipTransactionId,
                        principalTable: "MembershipTransactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembershipNotifications_MembershipTransactionId",
                table: "MembershipNotifications",
                column: "MembershipTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipNotifications_UserId",
                table: "MembershipNotifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembershipNotifications");

            migrationBuilder.DropColumn(
                name: "AutoRenewal",
                table: "MembershipTransactions");

            migrationBuilder.DropColumn(
                name: "CancellationDate",
                table: "MembershipTransactions");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "MembershipTransactions");

            migrationBuilder.DropColumn(
                name: "GracePeriodEnd",
                table: "MembershipTransactions");

            migrationBuilder.DropColumn(
                name: "GracePeriodStart",
                table: "MembershipTransactions");

            migrationBuilder.DropColumn(
                name: "IsInGracePeriod",
                table: "MembershipTransactions");

            migrationBuilder.DropColumn(
                name: "LastRenewalAttempt",
                table: "MembershipTransactions");

            migrationBuilder.DropColumn(
                name: "NextRenewalDate",
                table: "MembershipTransactions");

            migrationBuilder.DropColumn(
                name: "RenewalAttempts",
                table: "MembershipTransactions");
        }
    }
}
