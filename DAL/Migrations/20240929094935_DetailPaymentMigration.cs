using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class DetailPaymentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trn_repayment_detail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    repayment_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_repayment_detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trn_repayment_detail_trn_repayment_repayment_id",
                        column: x => x.repayment_id,
                        principalTable: "trn_repayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trn_repayment_detail_repayment_id",
                table: "trn_repayment_detail",
                column: "repayment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trn_repayment_detail");
        }
    }
}
