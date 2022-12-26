using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flitter.Api.Migrations
{
    public partial class AddLikesCountAndOptionVote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Option_Polls_PollId",
                table: "Option");

            migrationBuilder.DropForeignKey(
                name: "FK_OptionVote_Option_OptionId",
                table: "OptionVote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OptionVote",
                table: "OptionVote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Option",
                table: "Option");

            migrationBuilder.RenameTable(
                name: "OptionVote",
                newName: "OptionVotes");

            migrationBuilder.RenameTable(
                name: "Option",
                newName: "Options");

            migrationBuilder.RenameIndex(
                name: "IX_OptionVote_OptionId",
                table: "OptionVotes",
                newName: "IX_OptionVotes_OptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Option_PollId",
                table: "Options",
                newName: "IX_Options_PollId");

            migrationBuilder.AddColumn<int>(
                name: "LikesCount",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OptionVotes",
                table: "OptionVotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Options",
                table: "Options",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Options_Polls_PollId",
                table: "Options",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OptionVotes_Options_OptionId",
                table: "OptionVotes",
                column: "OptionId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Options_Polls_PollId",
                table: "Options");

            migrationBuilder.DropForeignKey(
                name: "FK_OptionVotes_Options_OptionId",
                table: "OptionVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OptionVotes",
                table: "OptionVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Options",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "LikesCount",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "OptionVotes",
                newName: "OptionVote");

            migrationBuilder.RenameTable(
                name: "Options",
                newName: "Option");

            migrationBuilder.RenameIndex(
                name: "IX_OptionVotes_OptionId",
                table: "OptionVote",
                newName: "IX_OptionVote_OptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Options_PollId",
                table: "Option",
                newName: "IX_Option_PollId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OptionVote",
                table: "OptionVote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Option",
                table: "Option",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Option_Polls_PollId",
                table: "Option",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OptionVote_Option_OptionId",
                table: "OptionVote",
                column: "OptionId",
                principalTable: "Option",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
