using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class SpotifyDataAdjusted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArtistImageUri",
                table: "SpotifyData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentArtist",
                table: "SpotifyData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentArtistUri",
                table: "SpotifyData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentSong",
                table: "SpotifyData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentSongUri",
                table: "SpotifyData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopArtist",
                table: "SpotifyData",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtistImageUri",
                table: "SpotifyData");

            migrationBuilder.DropColumn(
                name: "CurrentArtist",
                table: "SpotifyData");

            migrationBuilder.DropColumn(
                name: "CurrentArtistUri",
                table: "SpotifyData");

            migrationBuilder.DropColumn(
                name: "CurrentSong",
                table: "SpotifyData");

            migrationBuilder.DropColumn(
                name: "CurrentSongUri",
                table: "SpotifyData");

            migrationBuilder.DropColumn(
                name: "TopArtist",
                table: "SpotifyData");
        }
    }
}
