using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkinatorApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationsAndFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_GameSessions_GameSessionId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Users_AddedByUserId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_Users_UserId",
                table: "GameSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_SessionAnswers_GameSessions_GameSessionId",
                table: "SessionAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Characters",
                table: "Characters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SessionAnswers",
                table: "SessionAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameSessions",
                table: "GameSessions");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Characters",
                newName: "characters");

            migrationBuilder.RenameTable(
                name: "SessionAnswers",
                newName: "session_answers");

            migrationBuilder.RenameTable(
                name: "GameSessions",
                newName: "sessions");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "users",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "characters",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "AddedByUserId",
                table: "characters",
                newName: "added_by_user_id");

            migrationBuilder.RenameColumn(
                name: "AddedAt",
                table: "characters",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "characters",
                newName: "character_id");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_GameSessionId",
                table: "characters",
                newName: "IX_characters_GameSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_AddedByUserId",
                table: "characters",
                newName: "IX_characters_added_by_user_id");

            migrationBuilder.RenameColumn(
                name: "Answer",
                table: "session_answers",
                newName: "answer");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "session_answers",
                newName: "question_id");

            migrationBuilder.RenameColumn(
                name: "GameSessionId",
                table: "session_answers",
                newName: "session_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "session_answers",
                newName: "answer_id");

            migrationBuilder.RenameIndex(
                name: "IX_SessionAnswers_GameSessionId",
                table: "session_answers",
                newName: "IX_session_answers_session_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "sessions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "sessions",
                newName: "started_at");

            migrationBuilder.RenameColumn(
                name: "EndedAt",
                table: "sessions",
                newName: "ended_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "sessions",
                newName: "session_id");

            migrationBuilder.RenameIndex(
                name: "IX_GameSessions_UserId",
                table: "sessions",
                newName: "IX_sessions_user_id");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "characters",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_characters",
                table: "characters",
                column: "character_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_session_answers",
                table: "session_answers",
                column: "answer_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_sessions",
                table: "sessions",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_characters_UserId",
                table: "characters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_characters_sessions_GameSessionId",
                table: "characters",
                column: "GameSessionId",
                principalTable: "sessions",
                principalColumn: "session_id");

            migrationBuilder.AddForeignKey(
                name: "FK_characters_users_UserId",
                table: "characters",
                column: "UserId",
                principalTable: "users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_characters_users",
                table: "characters",
                column: "added_by_user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_session_answers_sessions",
                table: "session_answers",
                column: "session_id",
                principalTable: "sessions",
                principalColumn: "session_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_sessions_users",
                table: "sessions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_characters_sessions_GameSessionId",
                table: "characters");

            migrationBuilder.DropForeignKey(
                name: "FK_characters_users_UserId",
                table: "characters");

            migrationBuilder.DropForeignKey(
                name: "fk_characters_users",
                table: "characters");

            migrationBuilder.DropForeignKey(
                name: "fk_session_answers_sessions",
                table: "session_answers");

            migrationBuilder.DropForeignKey(
                name: "fk_sessions_users",
                table: "sessions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_characters",
                table: "characters");

            migrationBuilder.DropIndex(
                name: "IX_characters_UserId",
                table: "characters");

            migrationBuilder.DropPrimaryKey(
                name: "pk_sessions",
                table: "sessions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_session_answers",
                table: "session_answers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "characters");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "characters",
                newName: "Characters");

            migrationBuilder.RenameTable(
                name: "sessions",
                newName: "GameSessions");

            migrationBuilder.RenameTable(
                name: "session_answers",
                newName: "SessionAnswers");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Characters",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Characters",
                newName: "AddedAt");

            migrationBuilder.RenameColumn(
                name: "added_by_user_id",
                table: "Characters",
                newName: "AddedByUserId");

            migrationBuilder.RenameColumn(
                name: "character_id",
                table: "Characters",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_characters_GameSessionId",
                table: "Characters",
                newName: "IX_Characters_GameSessionId");

            migrationBuilder.RenameIndex(
                name: "IX_characters_added_by_user_id",
                table: "Characters",
                newName: "IX_Characters_AddedByUserId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "GameSessions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "started_at",
                table: "GameSessions",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "ended_at",
                table: "GameSessions",
                newName: "EndedAt");

            migrationBuilder.RenameColumn(
                name: "session_id",
                table: "GameSessions",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_sessions_user_id",
                table: "GameSessions",
                newName: "IX_GameSessions_UserId");

            migrationBuilder.RenameColumn(
                name: "answer",
                table: "SessionAnswers",
                newName: "Answer");

            migrationBuilder.RenameColumn(
                name: "session_id",
                table: "SessionAnswers",
                newName: "GameSessionId");

            migrationBuilder.RenameColumn(
                name: "question_id",
                table: "SessionAnswers",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "answer_id",
                table: "SessionAnswers",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_session_answers_session_id",
                table: "SessionAnswers",
                newName: "IX_SessionAnswers_GameSessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Characters",
                table: "Characters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameSessions",
                table: "GameSessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SessionAnswers",
                table: "SessionAnswers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_GameSessions_GameSessionId",
                table: "Characters",
                column: "GameSessionId",
                principalTable: "GameSessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Users_AddedByUserId",
                table: "Characters",
                column: "AddedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_Users_UserId",
                table: "GameSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionAnswers_GameSessions_GameSessionId",
                table: "SessionAnswers",
                column: "GameSessionId",
                principalTable: "GameSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
