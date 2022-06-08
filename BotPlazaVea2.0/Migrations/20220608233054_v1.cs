using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BotPlazaVea2._0.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Urls",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "text", nullable: false),
                    pagina = table.Column<int>(type: "integer", nullable: false),
                    endpoint = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urls", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombreProducto = table.Column<string>(type: "text", nullable: false),
                    precioReg = table.Column<decimal>(type: "numeric", nullable: false),
                    precioOferta = table.Column<decimal>(type: "numeric", nullable: false),
                    proveedor = table.Column<string>(type: "text", nullable: false),
                    categoria = table.Column<string>(type: "text", nullable: false),
                    subcategoria = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    subtipo = table.Column<string>(type: "text", nullable: false),
                    imagenUrl = table.Column<string>(type: "text", nullable: false),
                    promocion = table.Column<bool>(type: "boolean", nullable: false),
                    idUrl = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.id);
                    table.ForeignKey(
                        name: "FK_URL_1",
                        column: x => x.idUrl,
                        principalTable: "Urls",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Caracteristicas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    caracteristica = table.Column<string>(type: "text", nullable: false),
                    productoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caracteristicas", x => x.id);
                    table.ForeignKey(
                        name: "FK_Caracteristicas_Productos_productoId",
                        column: x => x.productoId,
                        principalTable: "Productos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Descripciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    productoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Descripciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Descripciones_Productos_productoId",
                        column: x => x.productoId,
                        principalTable: "Productos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Promociones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    condicion = table.Column<string>(type: "text", nullable: false),
                    productoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promociones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Promociones_Productos_productoId",
                        column: x => x.productoId,
                        principalTable: "Productos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Caracteristicas_productoId",
                table: "Caracteristicas",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "IX_Descripciones_productoId",
                table: "Descripciones",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_idUrl",
                table: "Productos",
                column: "idUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_productoId",
                table: "Promociones",
                column: "productoId");

            migrationBuilder.CreateIndex(
                name: "IX_Urls_url",
                table: "Urls",
                column: "url",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Caracteristicas");

            migrationBuilder.DropTable(
                name: "Descripciones");

            migrationBuilder.DropTable(
                name: "Promociones");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Urls");
        }
    }
}
