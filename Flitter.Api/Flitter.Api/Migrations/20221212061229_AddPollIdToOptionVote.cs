using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flitter.Api.Migrations
{
    public partial class AddPollIdToOptionVote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OptionVotes_Options_OptionId",
                table: "OptionVotes");

            migrationBuilder.DropIndex(
                name: "IX_OptionVotes_OptionId",
                table: "OptionVotes");

            migrationBuilder.AddColumn<int>(
                name: "PollId",
                table: "OptionVotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PollId",
                table: "OptionVotes");

            migrationBuilder.CreateIndex(
                name: "IX_OptionVotes_OptionId",
                table: "OptionVotes",
                column: "OptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OptionVotes_Options_OptionId",
                table: "OptionVotes",
                column: "OptionId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
