using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _10xPV.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            try
            {
                migrationBuilder.DropTable(
                    name: "AspNetRoleClaims");

                migrationBuilder.DropTable(
                 name: "AspNetUserClaims");

                migrationBuilder.DropTable(
                   name: "AspNetUserLogins");

                migrationBuilder.DropTable(
                    name: "AspNetUserRoles");

                migrationBuilder.DropTable(
                    name: "AspNetUserTokens");

                migrationBuilder.DropTable(
                    name: "AspNetRoles");

                migrationBuilder.DropTable(
                    name: "AspNetUsers");

            }
            catch { }

            try
            {
                migrationBuilder.DropTable(
                    name: "SensorReadings");

                migrationBuilder.DropTable(
                    name: "WeatherReadings");
            }
            catch { }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
