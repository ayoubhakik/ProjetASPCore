using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjetASPCore.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departement",
                columns: table => new
                {
                    id_departement = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    nom_departement = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    username = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    phone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departement", x => x.id_departement);
                });

            migrationBuilder.CreateTable(
                name: "Filieres",
                columns: table => new
                {
                    idFil = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    nomFil = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filieres", x => x.idFil);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    idSettings = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Delai = table.Column<DateTime>(nullable: false),
                    importEtudiant = table.Column<bool>(nullable: false),
                    importNote = table.Column<bool>(nullable: false),
                    Attributted = table.Column<bool>(nullable: false),
                    DatedeRappel = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.idSettings);
                });

            migrationBuilder.CreateTable(
                name: "Etudiants",
                columns: table => new
                {
                    cne = table.Column<string>(maxLength: 10, nullable: false),
                    nom = table.Column<string>(nullable: false),
                    prenom = table.Column<string>(nullable: false),
                    password = table.Column<string>(nullable: true),
                    nationalite = table.Column<string>(nullable: true),
                    cin = table.Column<string>(maxLength: 8, nullable: false),
                    email = table.Column<string>(nullable: true),
                    phone = table.Column<string>(maxLength: 10, nullable: true),
                    gsm = table.Column<string>(maxLength: 10, nullable: true),
                    address = table.Column<string>(nullable: true),
                    ville = table.Column<string>(nullable: true),
                    typeBac = table.Column<string>(nullable: true),
                    anneeBac = table.Column<int>(nullable: false),
                    noteBac = table.Column<double>(nullable: false),
                    mentionBac = table.Column<string>(nullable: true),
                    noteFstYear = table.Column<double>(nullable: false),
                    noteSndYear = table.Column<double>(nullable: false),
                    dateNaiss = table.Column<DateTime>(nullable: true),
                    lieuNaiss = table.Column<string>(nullable: true),
                    photo_link = table.Column<string>(nullable: true),
                    Choix = table.Column<string>(nullable: true),
                    Validated = table.Column<bool>(nullable: false),
                    Modified = table.Column<bool>(nullable: false),
                    Redoubler = table.Column<bool>(nullable: false),
                    idFil = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etudiants", x => x.cne);
                    table.ForeignKey(
                        name: "FK_Etudiants_Filieres_idFil",
                        column: x => x.idFil,
                        principalTable: "Filieres",
                        principalColumn: "idFil",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departement",
                columns: new[] { "id_departement", "email", "nom_departement", "password", "phone", "username" },
                values: new object[] { 1, "assmaesafae20@gmail.com", "ENSA", "12345", "0600000000", "ENSA" });

            migrationBuilder.InsertData(
                table: "Filieres",
                columns: new[] { "idFil", "nomFil" },
                values: new object[,]
                {
                    { 1, "informatique" },
                    { 2, "gtr" },
                    { 3, "Indus" },
                    { 4, "gpmc" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "idSettings", "Attributted", "DatedeRappel", "Delai", "importEtudiant", "importNote" },
                values: new object[] { 1, false, new DateTime(2020, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false });

            migrationBuilder.CreateIndex(
                name: "IX_Etudiants_idFil",
                table: "Etudiants",
                column: "idFil");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departement");

            migrationBuilder.DropTable(
                name: "Etudiants");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Filieres");
        }
    }
}
