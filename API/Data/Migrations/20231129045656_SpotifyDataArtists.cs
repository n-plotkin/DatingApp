using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class SpotifyDataArtists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentArtistUri",
                table: "SpotifyData",
                newName: "CurrentArtistsUris");

            migrationBuilder.RenameColumn(
                name: "CurrentArtist",
                table: "SpotifyData",
                newName: "CurrentArtists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentArtistsUris",
                table: "SpotifyData",
                newName: "CurrentArtistUri");

            migrationBuilder.RenameColumn(
                name: "CurrentArtists",
                table: "SpotifyData",
                newName: "CurrentArtist");
        }
    }
}
