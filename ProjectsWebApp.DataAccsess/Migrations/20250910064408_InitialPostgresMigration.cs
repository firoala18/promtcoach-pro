using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectsWebApp.DataAccsess.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactEmail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactEmail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactMessageSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessageSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatenschutzContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SectionType = table.Column<string>(type: "text", nullable: false),
                    ContentHtml = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatenschutzContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Termin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Titel = table.Column<string>(type: "text", nullable: false),
                    Referent = table.Column<string>(type: "text", nullable: true),
                    Beschreibung = table.Column<string>(type: "text", nullable: true),
                    Arbeitseinheiten = table.Column<string>(type: "text", nullable: true),
                    Veranstaltungsort = table.Column<string>(type: "text", nullable: true),
                    Organisation = table.Column<string>(type: "text", nullable: true),
                    Hinweis = table.Column<string>(type: "text", nullable: true),
                    InfosFuerTeilnehmer = table.Column<string>(type: "text", nullable: true),
                    Art = table.Column<string>(type: "text", nullable: true),
                    Startzeit = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Endzeit = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "fachgruppen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fachgruppen", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fakultaet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fakultaet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IconClass = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilterCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsAiGenerated = table.Column<bool>(type: "boolean", nullable: false),
                    AiBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    ItemSortMode = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Heroes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Lead = table.Column<string>(type: "text", nullable: false),
                    BackgroundUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heroes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomePageSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HeroTitle = table.Column<string>(type: "text", nullable: false),
                    HeroLead = table.Column<string>(type: "text", nullable: false),
                    HeroBackgroundUrl = table.Column<string>(type: "text", nullable: false),
                    HeroButtonText = table.Column<string>(type: "text", nullable: false),
                    HeroButtonAction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePageSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImpressumContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SectionType = table.Column<string>(type: "text", nullable: false),
                    ContentHtml = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpressumContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KontaktCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Funktion = table.Column<string>(type: "text", nullable: true),
                    KontaktDatenHtml = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KontaktCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeichteSpracheContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentHtml = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeichteSpracheContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MakerSpaceDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SubTitle = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MakerSpaceDescriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MakerSpaceProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    ProjectUrl = table.Column<string>(type: "text", nullable: false),
                    Top = table.Column<bool>(type: "boolean", nullable: false),
                    Forschung = table.Column<bool>(type: "boolean", nullable: false),
                    download = table.Column<bool>(type: "boolean", nullable: false),
                    tutorial = table.Column<bool>(type: "boolean", nullable: false),
                    events = table.Column<bool>(type: "boolean", nullable: false),
                    netzwerk = table.Column<bool>(type: "boolean", nullable: false),
                    lesezeichen = table.Column<bool>(type: "boolean", nullable: false),
                    ITRecht = table.Column<bool>(type: "boolean", nullable: false),
                    Beitraege = table.Column<bool>(type: "boolean", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MakerSpaceProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MitmachenContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SectionType = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MitmachenContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    IconClass = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    RouteType = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortalCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortalVideo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VideoPath = table.Column<string>(type: "text", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ImagePath = table.Column<string>(type: "text", nullable: true),
                    ShowImageInsteadOfVideo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalVideo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titel = table.Column<string>(type: "text", nullable: false),
                    RedirectUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Akronym = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Thema = table.Column<string>(type: "text", nullable: false),
                    Beschreibung = table.Column<string>(type: "text", nullable: false),
                    UsedModels = table.Column<string>(type: "text", nullable: false),
                    Schluesselbegriffe = table.Column<string>(type: "text", nullable: false),
                    Ziele = table.Column<string>(type: "text", nullable: false),
                    Temperatur = table.Column<double>(type: "double precision", nullable: false),
                    MaxZeichen = table.Column<int>(type: "integer", nullable: false),
                    GeneratedImagePath = table.Column<string>(type: "text", nullable: true),
                    PromptHtml = table.Column<string>(type: "text", nullable: false),
                    PromptType = table.Column<string>(type: "text", nullable: false),
                    FilterJson = table.Column<string>(type: "text", nullable: false),
                    MetaHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    Autorin = table.Column<string>(type: "text", nullable: true),
                    Lizenz = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptWords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedPrompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Akronym = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Beschreibung = table.Column<string>(type: "text", nullable: false),
                    Schluesselbegriffe = table.Column<string>(type: "text", nullable: false),
                    UsedModels = table.Column<string>(type: "text", nullable: false),
                    Thema = table.Column<string>(type: "text", nullable: false),
                    Ziele = table.Column<string>(type: "text", nullable: false),
                    GeneratedImagePath = table.Column<string>(type: "text", nullable: true),
                    PromptHtml = table.Column<string>(type: "text", nullable: false),
                    PromptType = table.Column<string>(type: "text", nullable: false),
                    FilterJson = table.Column<string>(type: "text", nullable: false),
                    Temperatur = table.Column<string>(type: "text", nullable: false),
                    MaxZeichen = table.Column<string>(type: "text", nullable: false),
                    MetaHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPrompts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IconPath = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SliderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsForVirtuellesKlassenzimmer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SliderItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TechAnforderung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechAnforderung", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UebersichtContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentHtml = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UebersichtContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrheberechtContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SectionType = table.Column<string>(type: "text", nullable: false),
                    ContentHtml = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrheberechtContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilterItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Info = table.Column<string>(type: "text", nullable: false),
                    Instruction = table.Column<string>(type: "text", nullable: false),
                    FilterCategoryId = table.Column<int>(type: "integer", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterItems_FilterCategories_FilterCategoryId",
                        column: x => x.FilterCategoryId,
                        principalTable: "FilterCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeatureCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HomePageSettingsId = table.Column<int>(type: "integer", nullable: false),
                    IconClass = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureCards_HomePageSettings_HomePageSettingsId",
                        column: x => x.HomePageSettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModeCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HomePageSettingsId = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    RouteType = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    IconClass = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModeCards_HomePageSettings_HomePageSettingsId",
                        column: x => x.HomePageSettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromptVariations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PromptTemplateId = table.Column<int>(type: "integer", nullable: false),
                    VariationJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptVariations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptVariations_PromptTemplate_PromptTemplateId",
                        column: x => x.PromptTemplateId,
                        principalTable: "PromptTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedPromptVariations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VariationJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SavedPromptId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPromptVariations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedPromptVariations_SavedPrompts_SavedPromptId",
                        column: x => x.SavedPromptId,
                        principalTable: "SavedPrompts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ProjectAcronym = table.Column<string>(type: "text", nullable: true),
                    KurzeBeschreibung = table.Column<string>(type: "character varying(1200)", maxLength: 1200, nullable: false),
                    longDescription = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    Conception = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReleaseYear = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    Lizenz = table.Column<string>(type: "text", nullable: true),
                    OpenSource = table.Column<string>(type: "text", nullable: true),
                    CreativeCommons = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    DokuLink = table.Column<string>(type: "text", nullable: true),
                    DownloadURL = table.Column<string>(type: "text", nullable: true),
                    ProjectResponsibility = table.Column<string>(type: "text", nullable: true),
                    Foerderung = table.Column<string>(type: "text", nullable: true),
                    ProjectDevelopment = table.Column<string>(type: "text", nullable: true),
                    ProjectLeadership = table.Column<string>(type: "text", nullable: true),
                    ProjectManagement = table.Column<string>(type: "text", nullable: true),
                    ProjectPartners = table.Column<string>(type: "text", nullable: true),
                    Netzwerk = table.Column<string>(type: "text", nullable: true),
                    Expertise = table.Column<string>(type: "text", nullable: true),
                    ProjectCoordination = table.Column<string>(type: "text", nullable: true),
                    ProjectConception = table.Column<string>(type: "text", nullable: true),
                    ProjectSupport = table.Column<string>(type: "text", nullable: true),
                    zusaetzlicheInformationen = table.Column<string>(type: "text", nullable: true),
                    zusaetzlicheInformationen1 = table.Column<string>(type: "text", nullable: true),
                    Development = table.Column<string>(type: "text", nullable: true),
                    SoftwareDevelopers = table.Column<string>(type: "text", nullable: true),
                    Programming = table.Column<string>(type: "text", nullable: true),
                    Design = table.Column<string>(type: "text", nullable: true),
                    DidacticDesignTeam = table.Column<string>(type: "text", nullable: true),
                    MediaDesign = table.Column<string>(type: "text", nullable: true),
                    UXDesign = table.Column<string>(type: "text", nullable: true),
                    InteractionDesign = table.Column<string>(type: "text", nullable: true),
                    SoundDesign = table.Column<string>(type: "text", nullable: true),
                    ThreeDArtist = table.Column<string>(type: "text", nullable: true),
                    Didactics = table.Column<string>(type: "text", nullable: true),
                    ContentDevelopment = table.Column<string>(type: "text", nullable: true),
                    StoryDesign = table.Column<string>(type: "text", nullable: true),
                    Research = table.Column<string>(type: "text", nullable: true),
                    ResearchTeam = table.Column<string>(type: "text", nullable: true),
                    Evaluation = table.Column<string>(type: "text", nullable: true),
                    EvaluationTeam = table.Column<string>(type: "text", nullable: true),
                    PrimaryTargetGroup = table.Column<string>(type: "text", nullable: true),
                    ProjectGoals = table.Column<string>(type: "text", nullable: true),
                    TaxonomyLevel = table.Column<string>(type: "text", nullable: true),
                    Methods = table.Column<string>(type: "text", nullable: true),
                    Applications = table.Column<string>(type: "text", nullable: true),
                    DidaktischerAnsatz = table.Column<string>(type: "text", nullable: true),
                    DidacticDesign = table.Column<string>(type: "text", nullable: true),
                    Recommendations = table.Column<string>(type: "text", nullable: true),
                    Materialien = table.Column<string>(type: "text", nullable: true),
                    Erfolgsmessung = table.Column<string>(type: "text", nullable: true),
                    Documents = table.Column<string>(type: "text", nullable: true),
                    References = table.Column<string>(type: "text", nullable: true),
                    Media = table.Column<string>(type: "text", nullable: true),
                    CategoryId = table.Column<int>(type: "integer", nullable: true),
                    FakultaetId = table.Column<int>(type: "integer", nullable: true),
                    FachgruppenId = table.Column<int>(type: "integer", nullable: true),
                    TechAnforderungId = table.Column<int>(type: "integer", nullable: true),
                    CategoryIds = table.Column<string>(type: "text", nullable: false),
                    FakultaetIds = table.Column<string>(type: "text", nullable: false),
                    FachgruppenIds = table.Column<string>(type: "text", nullable: false),
                    TechAnforderungIds = table.Column<string>(type: "text", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsVirtuellesKlassenzimmer = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_Fakultaet_FakultaetId",
                        column: x => x.FakultaetId,
                        principalTable: "Fakultaet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_TechAnforderung_TechAnforderungId",
                        column: x => x.TechAnforderungId,
                        principalTable: "TechAnforderung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_fachgruppen_FachgruppenId",
                        column: x => x.FachgruppenId,
                        principalTable: "fachgruppen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    IsMainImage = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectImages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectVideos_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "KI-Projekte" },
                    { 2, 2, "VR-Projekte" },
                    { 3, 3, "Nützliche Tools" }
                });

            migrationBuilder.InsertData(
                table: "ContactEmail",
                columns: new[] { "Id", "Email" },
                values: new object[] { 1, "h.seehagen-marx@uni-wuppertal.de" });

            migrationBuilder.InsertData(
                table: "ContactMessageSettings",
                columns: new[] { "Id", "Message" },
                values: new object[] { 1, "Um unser Prompt-Engineering-Tool auszuprobieren,\r\nkontaktieren Sie uns bitte per E-Mail." });

            migrationBuilder.InsertData(
                table: "DatenschutzContents",
                columns: new[] { "Id", "ContentHtml", "DisplayOrder", "SectionType", "Title" },
                values: new object[,]
                {
                    { 1, "Dies ist der Einleitungstext für das Impressum.", 1, "Text", "Datenschutz Einleitung" },
                    { 2, "Name und Anschrift des Verantwortlichen...", 2, "Accordion", "Verantwortlich" }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Arbeitseinheiten", "Art", "Beschreibung", "Endzeit", "Hinweis", "InfosFuerTeilnehmer", "Organisation", "Referent", "Startzeit", "Termin", "Titel", "Veranstaltungsort" },
                values: new object[] { 1, "3", null, "<p>Ein praktischer Workshop zur Umsetzung digitaler Projekte im Bildungsbereich.</p>", null, "Bitte Laptop mitbringen.", "Teilnahme kostenlos. Anmeldung erforderlich.", "ZIM-MediaLab", "Dr. Heike Seehagen-Marx", null, new DateTime(2025, 9, 17, 0, 0, 0, 0, DateTimeKind.Local), "Einführung in digitale Medienprojekte", "Raum 203, Gebäude K" });

            migrationBuilder.InsertData(
                table: "Fakultaet",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 0, "Fakultaet 6" },
                    { 2, 0, "Fakultaet 5" }
                });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "Description", "IconClass", "SortOrder", "Title" },
                values: new object[,]
                {
                    { 1, "Kuratiertes Set aus Best-Practice-Filtern für deine Prompts.", "bi bi-sliders", 1, "Intelligente Filter" },
                    { 2, "Kuratiertes Set aus Best-Practice-Filtern für deine Prompts.", "bi bi-sliders", 2, "Intelligente Filter" },
                    { 3, "Kuratiertes Set aus Best-Practice-Filtern für deine Prompts.", "bi bi-sliders", 3, "Intelligente Filter" }
                });

            migrationBuilder.InsertData(
                table: "FilterCategories",
                columns: new[] { "Id", "AiBatchId", "DisplayOrder", "IsAiGenerated", "ItemSortMode", "Name", "Type", "UserId" },
                values: new object[,]
                {
                    { 1, null, 0, false, 0, "Schlüsselbegriffe", 0, "system" },
                    { 2, null, 0, false, 0, "Zielgruppe", 0, "system" },
                    { 3, null, 0, false, 0, "Medien", 0, "system" },
                    { 4, null, 0, false, 0, "Didaktik", 0, "system" }
                });

            migrationBuilder.InsertData(
                table: "Heroes",
                columns: new[] { "Id", "BackgroundUrl", "Lead", "Title" },
                values: new object[] { 1, "/images/uploads/hero-bg.jpg", "Baue in Sekunden meisterhafte Prompts<br/>für Text, Bilder und Video.", "PromptCoach AI" });

            migrationBuilder.InsertData(
                table: "ImpressumContents",
                columns: new[] { "Id", "ContentHtml", "DisplayOrder", "SectionType", "Title" },
                values: new object[,]
                {
                    { 1, "Dies ist der Einleitungstext für das Impressum.", 1, "Text", "Impressum Einleitung" },
                    { 2, "Name und Anschrift des Verantwortlichen...", 2, "Accordion", "Verantwortlich" }
                });

            migrationBuilder.InsertData(
                table: "KontaktCards",
                columns: new[] { "Id", "DisplayOrder", "Funktion", "ImageUrl", "KontaktDatenHtml", "Name" },
                values: new object[] { 1, 1, "Leitung MediaLab", "/images/Kontakt/Heike_Seehagen-Marx.jpg", "Bergische Universität Wuppertal, Zentrum für Informations- und Medienverarbeitung (ZIM), Medienlabor (Leitung)", "Dr. Heike Seehagen-Marx" });

            migrationBuilder.InsertData(
                table: "LeichteSpracheContent",
                columns: new[] { "Id", "ContentHtml" },
                values: new object[] { 1, "@{\r\n    ViewData[\"Title\"] = \"Leichtesprache\";\r\n}\r\n\r\n<div class=\"container mt-5\">\r\n    <div class=\"row justify-content-center\">\r\n        <div class=\"col-md-10\">\r\n            <!-- Title -->\r\n            <h2 class=\"mb-4 \" style=\"color:#90bc14\">Projekte im MediaLab (MediaLab-Projekte)</h2>\r\n\r\n            <!-- Introduction -->\r\n            <p class=\"lead\">\r\n                Die Webseite \"MediaLab-Projekte\" präsentiert die Ergebnisse der gemeinsamen Projekte im MediaLab in einer übersichtlichen Projektbibliothek.\r\n            </p>\r\n\r\n            <p class=\"lead\">\r\n                Das MediaLab an der Bergischen Universität Wuppertal ist ein kreativer Raum, in dem Studierende, Lehrende und Forschende ihre Ideen umsetzen, neue Technologien testen und Prototypen entwickeln können.\r\n            </p>\r\n\r\n            <!-- Section: Lehrende -->\r\n            <h2 class=\"mt-5 \" style=\"color:#90bc14\">Für Lehrende – Ihre Chancen im MediaLab</h2>\r\n            <ul class=\"list-group list-group-flush mb-4\">\r\n                <li class=\"list-group-item\"><strong>Ideen umsetzen:</strong> Bringen Sie Ihre Ideen ins MediaLab und setzen Sie sie gemeinsam mit anderen um.</li>\r\n                <li class=\"list-group-item\"><strong>Prototypen entwickeln:</strong> Machen Sie aus Ihren Ideen Modelle, die Ihre Konzepte zeigen und weiterentwickeln.</li>\r\n                <li class=\"list-group-item\"><strong>Neue Lösungen testen:</strong> Probieren Sie neue Technologien und Methoden aus und testen Sie, ob sie gut funktionieren.</li>\r\n                <li class=\"list-group-item\"><strong>Zusammenarbeiten:</strong> Arbeiten Sie mit Studierenden und Kolleg*innen an innovativen Lösungen.</li>\r\n            </ul>\r\n\r\n            <!-- Section: Studierende -->\r\n            <h2 class=\"mt-5 \" style=\"color:#90bc14\">Für Studierende – Ihre Chancen im MediaLab</h2>\r\n            <ul class=\"list-group list-group-flush mb-4\">\r\n                <li class=\"list-group-item\"><strong>Praktika und Abschlussarbeiten:</strong> Nutzen Sie das MediaLab für spannende Themen, die Theorie und Praxis verbinden.</li>\r\n                <li class=\"list-group-item\"><strong>Seminararbeiten:</strong> Arbeiten Sie an praktischen Aufgaben und bringen Sie kreative Ideen ein.</li>\r\n                <li class=\"list-group-item\"><strong>Hilfskraftstellen:</strong> Engagieren Sie sich im MediaLab und sammeln Sie wertvolle Praxiserfahrung.</li>\r\n                <li class=\"list-group-item\"><strong>Eigene Ideen umsetzen:</strong> Haben Sie eine Idee? Nutzen Sie das MediaLab, um Ihre Projekte umzusetzen.</li>\r\n            </ul>\r\n\r\n            <!-- Section: Forschende -->\r\n            <h2 class=\"mt-5 \" style=\"color:#90bc14\">Für Forschende – Ihre Möglichkeiten im MediaLab</h2>\r\n            <ul class=\"list-group list-group-flush mb-4\">\r\n                <li class=\"list-group-item\"><strong>Projekte starten:</strong> Starten Sie eigene Forschung und arbeiten Sie mit anderen Disziplinen zusammen.</li>\r\n                <li class=\"list-group-item\"><strong>Neue Technologien testen:</strong> Nutzen Sie die Ausstattung im MediaLab, um neue Ideen und Technologien auszuprobieren.</li>\r\n                <li class=\"list-group-item\"><strong>Förderprojekte umsetzen:</strong> Holen Sie sich Unterstützung für Projekte mit Fördermitteln.</li>\r\n                <li class=\"list-group-item\"><strong>Forschung teilen:</strong> Stellen Sie Ihre Forschungsergebnisse auf der Webseite vor.</li>\r\n            </ul>\r\n\r\n            <!-- Section: Mitmachen -->\r\n            <h2 class=\"mt-5 \" style=\"color:#90bc14\">Wie können Sie mitmachen?</h2>\r\n            <ul class=\"list-group list-group-flush mb-4\">\r\n                <li class=\"list-group-item\"><strong>Eigene Projekte einreichen:</strong> Haben Sie eine Idee? Reichen Sie sie ein und arbeiten Sie mit uns zusammen.</li>\r\n                <li class=\"list-group-item\"><strong>Bestehende Projekte unterstützen:</strong> Schließen Sie sich laufenden Projekten an und bringen Sie Ihre Stärken ein.</li>\r\n                <li class=\"list-group-item\"><strong>Angebote nutzen:</strong> Bewerben Sie sich auf Praktika, Hilfskraftstellen oder nutzen Sie das MediaLab für Ihre Abschlussarbeit.</li>\r\n                <li class=\"list-group-item\"><strong>Jetzt aktiv werden:</strong> Das MediaLab freut sich auf Ihre Ideen und Ihr Engagement! Kontaktieren Sie uns!</li>\r\n            </ul>\r\n\r\n            <!-- Contact Section -->\r\n            <h2 class=\"mt-5 \" style=\"color:#90bc14\">Kontakt</h2>\r\n            <p class=\"mb-0\">\r\n                <strong>Dr. Heike Seehagen-Marx</strong><br>\r\n                <a href=\"mailto:h.seehagen-marx@uni-wuppertal.de\" class=\"text-decoration-none text-primary\">h.seehagen-marx@uni-wuppertal.de</a>\r\n            </p>\r\n        </div>\r\n    </div>\r\n</div>\r\n" });

            migrationBuilder.InsertData(
                table: "MakerSpaceProjects",
                columns: new[] { "Id", "Beitraege", "Description", "DisplayOrder", "Forschung", "ITRecht", "ImageUrl", "ProjectUrl", "Tags", "Title", "Top", "download", "events", "lesezeichen", "netzwerk", "tutorial" },
                values: new object[,]
                {
                    { 6, false, "<p>Ein kreatives Werkzeug, das mithilfe von KI visuelle Inhalte aus einfachen Textbeschreibungen erzeugt. Es unterstützt Designer und Kreative dabei, Bilder, Stile, Texturen und Effekte schneller und unkomplizierter zu gestalten. Um den kreativen Prozess zu verbessern und die Erstellung von Grafiken für Marketing, Design und soziale Medien zu erleichtern.</p>", 6, false, false, "/images/makerspace/03acee10-9669-4563-a960-74de0d9fcb63.jpg", "https://firefly.adobe.com/", "Bilder", "Adobe Firefly", true, false, false, false, false, false },
                    { 12, false, "<p>AIColors. co ist ein KI-gestützter Farbpalettengenerator, mit dem Nutzer mühelos einzigartige und ansprechende Farbpaletten für ihre Projekte erstellen. Die künstliche Intelligenz wandelt Texteingaben in individuelle Farbschemata um, die sich flexibel anpassen und visualisieren lassen. Designer, Künstler und Entwickler finden so schneller harmonische Farben für ihre Arbeiten.</p>", 12, false, false, "https://www.bairesdev.com/tools/ai-colors/ai-colors-assets/bairesdev-logo.svg", "https://aicolors.co/", "Color", "Aicolors", true, false, false, false, false, false },
                    { 16, false, "<p>ATLAS. ti, eine bewährte Software für qualitative Datenanalyse, integriert mit Intentional AI Coding einen fortschrittlichen KI-Assistenten. Forschende geben ihre spezifischen Forschungsziele an und erhalten automatisierte Codierungsvorschläge.</p>", 16, false, false, "https://getlogovector.com/wp-content/uploads/2021/07/atlas-ti-logo-vector.png", "https://atlasti.com/", "Forschung", "Atlasti", true, false, false, false, false, false },
                    { 17, false, "<p>Diese kostenlose, webbasierte Alternative zu Adobe Photoshop bietet umfassende Bildbearbeitungsfunktionen und unterstützt zahlreiche Dateiformate - ganz ohne Installation. Entdecken Sie, wie Sie mit diesem Tool professionelle Grafiken und Fotos direkt im Browser erstellen und optimieren können.</p>", 17, false, false, "/images/makerspace/21f7a769-62ba-4206-be21-d3840a286e9e.png", "https://www.photopea.com/", "Design", "Photopea", false, false, false, false, false, false },
                    { 22, false, "<p>Entdecken Sie mit dem Brandmark Font Generator ein innovatives Online-Tool, das Designern ermöglicht, einzigartige Schriftartenkombinationen mithilfe von KI zu erstellen und visuelle Kontraste zu finden.</p>", 22, false, false, "/images/makerspace/fa16ad0d-3b65-46fb-b113-f358c7b1c274.png", "https://brandmark.io/font-generator/", "Schriften", "brandmark.io", true, false, false, false, false, false },
                    { 25, false, "<p>Design-Plattform, die KI-gestützte Funktionen integriert, um die Erstellung von Grafiken, Präsentationen, Social-Media-Posts und anderen visuellen Inhalten zu vereinfachen. Mit KI-Tools wie dem Text-zu-Bild-Generator, Designvorschlägen und automatischer Bildoptimierung können Benutzer schnell und einfach ansprechende Designs erstellen.</p>", 25, false, false, "/images/makerspace/c46eb62a-41e8-46ea-af85-572b74a0d225.svg", "https://www.canva.com/", "Visualisieren,Design", "Canva", true, false, false, false, false, false },
                    { 31, false, "<p>Colormind erstellt ein Farbdesign: Mit Deep Learning generiert die KI ästhetische Farbschemata aus Fotos, Filmen und Kunstwerken und bietet Designern eine Lösung für harmonische Farbpaletten.</p>", 31, false, false, "/images/makerspace/5037ec0b-07df-4226-a01f-287768192bec.svg", "http://colormind.io/", "Color,Design", "Colormind", true, false, false, false, false, false },
                    { 32, false, "<p>Windsurf, vorher unter Codeium bekannt, ist ein Tool, das Entwicklern mit intelligenten Codevorschlägen und -ergänzungen hilft, effizienter zu arbeiten.</p>", 32, false, false, "/images/makerspace/d040eb27-3c73-4688-a473-4aaa88347d5d.jpg", "https://windsurf.com/", "Programmieren", "Windsurf", true, false, false, false, false, false },
                    { 33, false, "<p>Das Medienpädagogische Zentrum Landkreis Leipzig demonstriert, wie Lehrkräfte H5P und KI verbinden, um interaktive Lernmaterialien zu gestalten. Der Artikel erläutert die Grundlagen von H5P, KI und Prompts, zeigt deren Einsatz für Quiz, Kreuzworträtsel und Lückentexte und erklärt die Erstellung von Bildern und 360°-Panoramen. Außerdem wird beschrieben, wie man mit KI eine H5P-GameMap-Story entwickelt. Ziel ist es, Lehrkräfte dabei zu unterstützen, motivierende Lernangebote zu schaffen.</p>", 33, false, false, "/images/makerspace/b3f9a834-3142-49f6-85c9-dba2936f4b3d.png", "https://mpz-landkreis-leipzig.de/h5p-mit-ki-erstellen/#H5P-Werkzeuge,-die-eine-problemlose-Erstellung-per-KI-erm%C3%B6glichen", "H5P", "H5P mit KI erstellen", true, false, false, false, false, false },
                    { 36, false, "<p>Visualisiert Verbindungen zwischen wissenschaftlichen Arbeiten und zeigt relevante Publikationen an.</p>", 36, false, false, "/images/makerspace/017c36fc-50aa-4a62-86c2-13e863e44a1a.png", "https://www.connectedpapers.com/", "Forschung", "Connected Papers", true, false, false, false, false, false },
                    { 37, false, "<p>Mit Draw. kit wird das Zeichnen im Internet zum Kinderspiel. Entdecken Sie eine Vielzahl von Werkzeugen und Optionen, um Ihre Ideen in beeindruckende Zeichnungen umzusetzen. Pinsel, Stifte, Formen und mehr stehen Ihnen zur Verfügung, um Ihre Kreationen zu perfektionieren.</p>", 37, false, false, "/images/makerspace/5157b533-7fa1-4c5a-b905-5e1e8cb1c9c0.svg", "https://draw.kits.blog/", "Design", "Excalidraw", false, false, false, false, false, false },
                    { 39, false, "<p>Consensus ist eine KI-gestützte Plattform zur schnellen Extraktion von Ergebnissen und Schlussfolgerungen aus wissenschaftlichen Studien.</p>", 39, false, false, "/images/makerspace/2c153f4e-297f-4d97-8d9e-6ebe0501c6d9.svg", "https://consensus.app/search/", "Forschung", "Consensus", true, false, false, false, false, false },
                    { 40, false, "<p>Plattform zur Erstellung vollständiger Kursstrukturen mit Lektionen, Zielen und Quizfragen auf Basis weniger Stichworte. Ermöglicht Nutzung von Multimedia-Inhalten, individuelle Anpassung und Vorschaufunktionen zur Verbesserung des Lernerlebnisses vor der Veröffentlichung. Bietet kostenlose Pläne und flexible Optionen für effektive Kurserstellung.</p>", 40, false, false, "/images/makerspace/f4f8cd0b-6490-4953-8eac-5cb026fc77be.png", "https://www.coursebox.ai/de", "Lehre planen", "Coursebox", true, false, false, false, false, false },
                    { 43, false, "<p>KI-basierte Bildgenerierung, die detaillierte Bilder aus Textbeschreibungen erzeugt. Unterstützt komplexe Szenen und künstlerische Stile. Ideal für visuelle Konzepte und kreative Projekte.</p>", 43, false, false, "/images/makerspace/06b291e5-cb3c-4bc1-9649-f85b916f6a1a.jpg", "https://openai.com/dall-e", "Bilder", "DALL-E", true, false, false, false, false, false },
                    { 45, false, "<p>Präzise Übersetzungen mit Kontextverständnis für über 30 Sprachen. Besonders geeignet für Fachtexte und idiomatische Ausdrücke. Integriert Dokumentenübersetzung und Glossary-Funktion.</p>", 45, true, false, "/images/makerspace/5534b156-b26f-42b1-8f01-24fabee27297.svg", "https://www.deepl.com/translator", "Übersetzungen", "DeepL", true, false, false, false, false, false },
                    { 48, false, "<p>Whimsical ist ein kollaboratives Online-Tool für visuelles Denken. Es ermöglicht Teams gemeinsam Mindmaps, Flussdiagramme, Wireframes, Sticky Notes und Dokumente zu erstellen. Zudem zeichnet es sich durch seine einfache Bedienung, intuitive Oberfläche und schnelle Visualisierungsideen aus - ideal für Brainstorming, Projektplanung.</p>", 48, true, false, "/images/makerspace/508810bc-bbab-48c7-9d86-370bfcd06778.webp", "https://whimsical.com/", "Visualisieren", "Whimsical", true, false, false, false, false, false },
                    { 50, false, "<p>Audio- und Video-Editor, der auch Transkriptionen und Untertitel erstellen kann.</p>", 50, false, false, "/images/makerspace/9e587792-84ee-463e-be45-008921235782.png", "https://www.veed.io/", "Audio,Video", "Veed.io", true, false, false, false, false, false },
                    { 51, false, "<p>Eine kostenpflichtige Plagiatsprüfungssoftware für Bildungseinrichtungen. Sie vergleicht eingereichte Arbeiten mit einer umfassenden Datenbank, um Übereinstimmungen zu erkennen.</p>", 51, false, false, "/images/makerspace/b40380f9-4226-4ab8-b41c-9578dcd1b0da.svg", "https://www.turnitin.de/", "Plagiatprüfung", "Turnitin Similarity", true, false, false, false, false, false },
                    { 52, false, "<p>DeepAI bietet eine Vielzahl von innovativen KI-Tools. Von einem KI-basierten Chatbot über die Generierung von Bildern und Musik bis hin zur Erstellung von 3D-Modellen - die Möglichkeiten scheinen grenzenlos. Erfahren Sie, wie DeepAI die Grenzen des Machbaren neu definiert und die Kreativität auf ein neues Level hebt.</p>", 52, false, false, "/images/makerspace/31af52ba-c50b-40f8-9c25-ef5113f4b39a.jpg", "https://deepai.org/", "Bilder,Video,Musik,3D-Objekte,Multimodal", "Deepai.org", false, false, false, false, false, false },
                    { 53, false, "<p>Mit Trickle AI können Sie ohne Programmierkenntnisse beeindruckende Websites und KI-gestützte Anwendungen erstellen - dank benutzerfreundlicher Oberfläche und integrierter Designvorlagen. Entdecken Sie, wie einfach digitale Innovation sein kann.</p>", 53, false, false, "/images/makerspace/fe9b4c0f-19d1-4273-8853-55a69cd21f1f.jpg", "https://www.trickle.so/", "Programmieren", "Trickle AI", false, false, false, false, false, false },
                    { 54, false, "<p>Die App ist eine KI-gestützte Plattform, die Bilder in surreal-verzerrte Kunstwerke verwandelt, indem sie Muster und Strukturen verstärkt.</p>", 54, false, false, "/images/makerspace/f00b78a2-2a89-4013-8de5-6a6d012d9015.jpg", "https://deepdreamgenerator.com/", "Bilder", "Deep Dream Generator", false, false, false, false, false, false },
                    { 55, false, "<p>Textoptimierung, die über die bloße Korrektur von Rechtschreib- und Grammatikfehlern hinausgeht und gezielte stilistische Verbesserungsvorschläge bietet.</p>", 55, false, false, "/images/makerspace/309bf82f-35cd-420a-8ddf-ab669d3c726f.svg", "https://www.deepl.com/de/write", "Textanalyse,Schreibprozesse", "DeepL Write", true, false, false, false, false, false },
                    { 56, false, "<p>Die Stable Diffusion-KI ermöglicht eine Open-Source-Bildgenerierung mit lokaler Installation. Außerdem bietet sie eine detaillierte Kontrolle über Generierungsparameter und Erweiterungen durch Community-Modelle.</p>", 56, false, false, "/images/makerspace/996d450e-7a51-459a-bcb8-e6d9f1705adc.png", "https://stability.ai/", "Bilder", "Stable Diffusion", false, false, false, false, false, false },
                    { 57, false, "<p>DokuMet-AI, ein speziell für die dokumentarische Methode entwickeltes Tool, unterstützt den qualitativen Forschungsprozess mit künstlicher Intelligenz. Es hilft Forschenden, Textsequenzen zu interpretieren und tiefere Bedeutungsstrukturen im Datenmaterial zu erkennen.</p>", 57, true, false, "/images/makerspace/cbb03917-f498-4a58-99d5-55208a5bbfcc.png", "https://dokumet.de/", "Forschung", "DokuMet QDA/AI", false, false, false, false, false, false },
                    { 58, false, "<p>Soundraw ist eine KI-gestützte Musikplattform, mit der Nutzer individuelle, lizenzfreie Musikstücke für Videos, Podcasts und andere kreative Projekte generieren können. Durch Auswahl von Stil, Stimmung, Länge und Instrumenten ermöglicht Soundraw personalisierte Kompositionen, die sich flexibel anpassen und nahtlos in digitale Inhalte integrieren lassen.</p>", 58, false, false, "/images/makerspace/4078c720-f28a-4eca-9e25-11fbbb940f95.jpg", "https://soundraw.io/", "Musik", "Soundraw", false, false, false, false, false, false },
                    { 59, false, "<p>Neben Lektorat und Korrekturlesen bietet Scribbir auch eine Plagiatsprüfung an. Der Dienst vergleicht Arbeiten mit einer Datenbank von über 99 Milliarden Quellen und zeigt Ähnlichkeitsanteile sowie entsprechende Textstellen an.</p>", 59, false, false, "/images/makerspace/cc2672b1-d23a-44bf-ad0f-4ae16e54ef1e.svg", "https://www.scribbr.com/", "Plagiatprüfung", "Scribbr", true, false, false, false, false, false },
                    { 60, false, "<p>ElevenLabs ist ein KI-Werkzeug, das synthetischen&nbsp;Stimmen auf ein höheres Level bringt.&nbsp; Es eröffnet neue Möglichkeiten in der Welt der Sprachsynthese, indem es Stimmen erschafft, die so realistisch und natürlich klingen, dass sie kaum von echten Menschen zu unterscheiden sind.</p>", 60, false, false, "https://eleven-public-cdn.elevenlabs.io/payloadcms/9trrmnj2sj8-logo-logo.svg", "https://elevenlabs.io/", "Sprachgeneratoren", "ElevenLabs", true, false, false, false, false, false },
                    { 61, false, "<p>SciSpace ist eine intelligente Plattform zur Literaturverwaltung, die speziell für die kollaborative Forschungsarbeit entwickelt wurde - ideal, um wissenschaftliche Quellen effizient zu organisieren, zu analysieren und im Team zu nutzen.</p>", 61, true, false, "/images/makerspace/dfebd40f-28a8-4f4c-9c7b-f116633f4840.svg", "https://scispace.com/", "Forschung,Zitationen", "SciSpace", true, false, false, false, false, false },
                    { 62, false, "<p>Ein Tool zur Recherche und Entdeckung für Forscher. Hilft bei der Suche nach ähnlichen Arbeiten und dem Aufbau von Publikationsnetzwerken.</p>", 62, true, false, "/images/makerspace/8e7ddd05-f7c9-412a-8ee4-b8b64969b518.png", "https://www.researchrabbit.ai/", "Forschung", "Research Rabbit", true, false, false, false, false, false },
                    { 63, false, "<p>Ellicit führt, mittels KI-Modellen, automatisierte Literaturrecherche und Extraktion relevanter Studien durch.&nbsp;</p>", 63, true, false, "/images/makerspace/06b5cad6-f9ad-4d4f-8c79-62aa7dc85582.svg", "https://elicit.com/", "Forschung,Literaturrecherche", "Elicit", true, false, false, false, false, false },
                    { 64, false, "<p>Gamma ist eine KI-gestützte Plattform, mit der Nutzer Präsentationen, Dokumente und Websites schnell erstellen können- ganz ohne Design- oder Programmierkenntnisse. Die intuitive Oberfläche erlaubt es, interaktive Elemente wie Galerien, Videos und eingebettete Inhalte einzufügen.</p>", 64, false, false, "https://upload.wikimedia.org/wikipedia/commons/thumb/1/16/GAMMA_Logo.svg/681px-GAMMA_Logo.svg.png?20240929072414", "https://gamma.app/", "Visualisieren", "Gamma", true, false, false, false, false, false },
                    { 66, false, "<p>Die App bietet Tools zur Modelloptimierung und -überwachung, die bei der Erstellung von Lernmaterialien, Textgenerierung und der Durchführung von Forschungsprojekten hilfreich sind. Sie kann in Bereichen wie Schreiben, Wissenserwerb und Forschung eingesetzt werden, um Projekte und interaktive Lernanwendungen zu entwickeln.</p>", 66, false, false, "/images/makerspace/6e3e0887-4813-45b2-9075-716e5b25c865.png", "https://aistudio.google.com/", "Schreibprozesse", "Google AI Studio", true, false, false, false, false, false },
                    { 67, false, "<p>Umfassende Suchmaschine für wissenschaftliche Literatur verschiedener Disziplinen und Quellen.</p>", 67, false, false, "https://scholar.google.com/intl/de/scholar/images/2x/scholar_logo_64dp.png", "https://scholar.google.com/", "Forschung,Literaturrecherche", "Google Scholar", true, false, false, false, false, false },
                    { 69, false, "<p>Hailuo AI ist ein KI-Tool, dass aus multimodale und generative Eingaben Videos erstellt.</p>", 69, false, false, "/images/makerspace/d52ec6b3-eecc-4308-b352-b961a439a286.png", "https://hailuoai.video/", "Video", "Hailuo AI", true, false, false, false, false, false },
                    { 70, false, "<p>Mit Huemint bietet eine Ki für das Design: Das Tool erstellt einzigartige Farbpaletten und ermöglicht es Designern, mit einem Schieberegler die Kreativität ihrer Farbkombinationen zu steuern.</p>", 70, false, false, "/images/makerspace/154a2ae6-51be-4bb0-ae2b-93e3221ee241.png", "https://huemint.com/", "Design", "Huemint", true, false, false, false, false, false },
                    { 73, false, "<p>Eine Plattform mit intuitiver Benutzeroberfläche zur Anpassung von Stilen und Designs. Es ermöglicht die Umsetzung kreativer Ideen durch die Eingabe von Prompts aus Bildgeneratoren.</p>", 73, false, false, "/images/makerspace/87d5a4c5-6cba-4417-ad34-1409f486ce2c.png", "https://ideogram.ai/", "Bilder,Prompt", "Ideogram.ai", false, false, false, false, false, false },
                    { 74, false, "<p>Immersity AI verwandelt mit KI 2D-Bilder und -Videos in bewegende 3D-Erlebnisse. Nutzer können ihre Inhalte in 3D-Motion-Bilder, 3D-Videos oder 3D-Bilder umwandeln und sie auf XR-Geräten wie Apple Vision Pro und Meta Quest erleben.</p>", 74, false, false, "/images/makerspace/90ca7f27-c02e-4278-8a4a-ee72ea405e1a.png", "https://www.immersity.ai/", "Video,Bilder", "Immersity.ai", false, false, false, false, false, false },
                    { 75, false, "<p>Jungle Ai ist eine KI-gestützte Plattform, die Vorlesungsfolien in Übungsfragen verwandelt und personalisiertes Feedback bietet. Entdecken Sie, wie diese App den Lernfortschritt verfolgt und individuelle Wiederholungssitzungen anbietet.</p>", 75, false, false, "/images/makerspace/b855f517-cda0-47b8-8b6d-724a613a6f69.png", "https://jungleai.com/", "Quiz,Prüfungen,Feedback", "Jungle AI", true, false, false, false, false, false },
                    { 77, false, "<p>Ein Open-Source-Tool, das mehrere Sprachen unterstützt und Fehler in Rechtschreibung, Grammatik und Stil erkennt.</p>", 77, false, false, "https://upload.wikimedia.org/wikipedia/commons/thumb/5/54/LanguageTool_Logo_%282018%29.svg/551px-LanguageTool_Logo_%282018%29.svg.png", "https://languagetool.org/", "Textanalyse,Schreibprozesse", "LanguageTool", true, false, false, false, false, false },
                    { 79, false, "<p>Looka ist eine KI-gestützte Plattform, auf der in wenigen Schritten professionelle Logos und umfassende Markenidentitäten zu erstellen sind - ganz ohne Designkenntnisse.</p>", 79, false, false, "/images/makerspace/565322be-c02a-464d-b5b9-165c2c71ab0e.svg", "https://looka.com/", "Logo,Design", "Looka", false, false, true, false, false, false },
                    { 80, false, "<p>Leonardo Ai bietet eine professionelle Bildgenerierung mit hoher Kontrolle über Stil und Komposition. Es werden zusätzlich Tools zur Bildverfeinerung und Batch-Erstellung bereitgestellt. Das KI-Tool ist spezialisiert auf kommerzielle Anwendungen.</p>", 80, false, false, "/images/makerspace/72a374b1-337c-40b4-8c15-57177cd3ccfa.jpg", "https://leonardo.ai/", "Bilder,Video", "Leonardo AI", false, false, false, false, false, false },
                    { 81, false, "<p>Ein KI-gestütztes Werkzeug zur Literaturrecherche, das Forschungszusammenhänge visualisiert und wichtige Arbeiten in einem Fachgebiet identifiziert.</p>", 81, true, false, "/images/makerspace/7d10c227-6576-45b3-821b-893735cf81ac.svg", "https://www.litmaps.com/", "Literaturrecherche,Textanalyse", "Litmaps", false, false, false, false, false, false },
                    { 83, false, "<p>MAXQDA revolutioniert die qualitative Datenanalyse: Mit der Einführung von AI Assist werden Forschende von komplexen Analyseprozessen entlastet. Wie genau die Integration von künstlicher Intelligenz den Forschungsalltag erleichtert und welche Vorteile sie bietet, erfahren Sie in unserem Artikel.</p>", 83, true, false, "/images/makerspace/72d2ca96-c86c-4925-8f79-565e8afb6f3e.svg", "https://www.maxqda.com/de/", "Forschung,Textanalyse", "MAXQDA", false, false, false, false, false, false },
                    { 85, false, "<p>Entdecken Sie die innovative Open-Source-Software QualCoder, die mit KI-gestütztem horizontalen Codieren, die qualitative Datenanalyse revolutioniert. Erfahren Sie, wie Forschende mithilfe interaktiver Interpretationen und vollständiger Kontrolle über die KI-Integration tiefere Einblicke gewinnen können.</p>", 85, true, false, "/images/makerspace/748bbd16-a788-4637-9b48-f1648ebc3952.png", "https://github.com/mdwoicke/QualCoder/blob/ai_integration/README.md", "Forschung", "QualCoder", false, false, false, false, false, false },
                    { 87, false, "<p>Midjourney ermöglicht KI-gestützte künstlerisch orientierte Bildgenerierung mit Fokus auf ästhetische Qualität und Experimentierfreude. Die kollaborative Erstellung und Stilmischungen der Bilder werden ebenfalls zur Verfügung gestellt.</p>", 87, false, false, "https://upload.wikimedia.org/wikipedia/commons/e/e6/Midjourney_Emblem.png", "https://www.midjourney.com/", "Bilder", "Midjourney", false, false, true, false, false, false },
                    { 88, false, "<p>Entdecken Sie mit Miro eine innovative Plattform für kollaboratives Arbeiten an digitalen Whiteboards in Echtzeit, inklusive vielfältiger Vorlagen wie KI-Flussdiagramme und der Möglichkeit für Video- und Telefonkonferenzen in der Vollversion.</p>", 88, false, false, "https://upload.wikimedia.org/wikipedia/en/thumb/9/9c/Mir_company_logo_with_text.tiff/lossless-page1-304px-Mir_company_logo_with_text.tiff.png", "https://miro.com/", "Visualisieren", "Miro", true, false, false, false, false, false },
                    { 91, false, "<p>Design, Prototyping und Zusammenarbeit vereint in einem Tool. Wie das die Arbeit von Designern erleichtert und die Benutzererfahrung auf ein neues Level hebt, erfahren Sie hier.</p>", 91, false, false, "/images/makerspace/b40f7ec3-53bc-4284-93af-5ecbb9f6acf9.svg", "https://mockitt.com/", "Visualisieren,Design", "Mockitt", false, false, false, false, false, false },
                    { 92, false, "<p>Eine Lösung, um Text in Sprache umzuwandeln und Videos mit echten Stimmen zu erstellen. Dieses leicht bedienbare KI-Tool ermöglicht es, aus Texten lebendige Sprachaufnahmen zu machen.</p>", 92, false, false, "/images/makerspace/4d94c48e-aef2-4750-91ac-defffa6d96be.svg", "https://murf.ai/", "Sprachgeneratoren", "Murf.ai", true, false, false, false, false, false },
                    { 94, false, "<p>Die künstliche Intelligenz unterstützt die visuelle Ideengestaltung, Ideenorganisation und das Wissensmanagement, indem sie Konzepte vernetzt und Strukturen sichtbar macht. Sie fördert assiziatives Denken, exploratives Lernen und kreative Prozesse wie Design Thinking und Co-Creation- ideal für visuelles Mapping.&nbsp;</p>", 94, false, false, "https://www.napkin.ai/assets/napkin-logo-2024-beta.svg", "https://app.napkin.ai/", "Visualisieren", "Napkin", true, false, false, false, false, false },
                    { 95, false, "<p>Nutzer können Wissensbasen aus unterschiedlichen Quellen erstellen. KI analysiert die Quellen und beantwortet Fragen. Zudem werden Zusammenfassungen, FAQs und Arbeitshilfen generiert. Die gemeinsame Bearbeitung von Wissensbasen ist möglich.</p>", 95, false, false, "/images/makerspace/7807fb7a-4031-42ed-bbb5-6abfe8d791d0.svg", "https://notebooklm.google/", "Schreibprozesse", "NotebookLM", true, false, false, false, false, false },
                    { 97, false, "<p>Eine Transkriptionssoftware, die Meetings und Gespräche in Echtzeit aufzeichnen und transkribieren kann.</p>", 97, false, false, "https://cdn.prod.website-files.com/618e9316785b3582a5178502/65c9f5105c1f5d9effb29333_Otter_Blue_Vertical-p-800.png", "https://otter.ai/", "Transkription", "Otter.ai", true, false, false, false, false, false },
                    { 98, false, "<p>KI-Suchmaschine, die in Echtzeit Internetrecherchen durchführt und Antworten mit Quellenangaben liefert. Besonders nützlich für erste Literaturrecherchen.</p>", 98, true, false, "https://upload.wikimedia.org/wikipedia/commons/thumb/1/1d/Perplexity_AI_logo.svg/768px-Perplexity_AI_logo.svg.png", "https://www.perplexity.ai/", "Literaturrecherche,Textanalyse", "Perplexity AI", true, false, true, false, false, false },
                    { 99, false, "<p>Mit Petalica Paint nutzt eine KI-unterstützte Plattform für die digitale Kunst: Skizzen werden automatisch koloriert, während Künstler aus drei einzigartigen Stilen wählen und ihre Werke mit präzisen Farbhints verfeinern können.</p>", 99, false, false, "/images/makerspace/64d33f69-a54e-4069-94a1-6f4f397a633c.svg", "https://petalica.com/", "Bilder", "Petalica Paint", false, false, true, false, false, false },
                    { 102, false, "<p>QuillBot verbessert Texte durch Korrekturlesen, Umschreiben, Paraphrasieren und Zusammenfassen. Es spart Zeit und bietet Inspiration für neue Schreibstile.</p>", 102, false, false, "/images/makerspace/ffd74df8-8a5d-40ae-8613-ade516ad21f6.png", "https://quillbot.com/de/", "Schreibprozesse,Plagiatprüfung", "QuillBot", true, false, false, false, false, false },
                    { 103, false, "<p>Quizbot ist ein KI-gestützter Fragengenerator, der die Erstellung von Fragen und Prüfungen effizient und präzise optimiert.</p>", 103, false, false, "/images/makerspace/79336b20-f6e1-4cef-9205-8d326b659a61.png", "https://quizbot.ai/de", "Prüfungen,Quiz", "Quizbot", false, false, false, false, false, false },
                    { 115, false, "<p>Trinka AI ist ein KI-basierter Schreibassistent, der speziell für akademisches und technisches Englisch entwickelt wurde. Er prüft Grammatik, Stil und Fachsprache auf hohem Niveau und bietet kontextbezogene Verbesserungsvorschläge. Zusätzlich unterstützt Trinka bei der Plagiatsprüfung sowie beim Umschreiben komplexer Sätze. Die Software lässt sich in Tools wie Microsoft Word und gängigen Browsern integrieren und unterstützt auch LaTeX-Dokumente.</p><p>Obwohl Trinka primär auf englischsprachige Texte ausgerichtet ist, kann die Software auch deutsche Texte lesen und verarbeiten - etwa zur Vorabprüfung oder als Teil eines Übersetzungsprozesses.</p>", 115, true, false, "https://www.trinka.ai/assets/images/trinka_logo.svg", "https://www.trinka.ai/", "Schreibassistent", "Trinka", true, false, false, false, false, false },
                    { 1001, false, "<p>Seelab ist eine KI-Plattform zur schnellen Erstellung hochwertiger, markenkonformer Produktbilder und Videos in 4K. Nutzer trainieren eigene Markenmodelle und bearbeiten Visuals einfach. Ideal für Marken, Agenturen und Kreativteams, mit sicherem, datenschutzkonformem Service.</p>", 1001, false, false, "/images/makerspace/a2147749-4a7c-44a6-8532-12dad25ffaa9.png", "https://www.seelab.ai/en/", "Produktbilder,Video", "Seelab", true, false, true, false, false, false },
                    { 1002, false, "<p>Pollo AI ist eine Plattform zur Erstellung von KI-generierten Bildern und Videos. Sie wandelt Texte, Bilder oder Videos in animierte Clips in verschiedenen Stilen um. Zudem bietet sie Tools wie Upscaling, Face-Swap und Stiltransfers. Ideal ist sie für Content Creator, die schnell visuelle Inhalte produzieren möchten.</p>", 1002, false, false, "/images/makerspace/64de651e-1b58-4557-bbd1-8ffff56057f9.png", "https://pollo.ai/", "Video", "Pollo AI", true, false, true, false, false, false },
                    { 1008, false, "<p><strong>Bundesministerium der Justiz &nbsp;- März 2024</strong><br>Der Text behandelt den Schutz des Urheberrechts im Zusammenhang mit Künstlicher Intelligenz. Er erklärt, dass beim Training von KI urheberrechtlich geschützte Inhalte nur mit Zustimmung genutzt werden dürfen. Reine KI-generierte Inhalte sind nicht urheberrechtlich geschützt - es sei denn, der Mensch wirkt kreativ mit. Die EU-KI-Verordnung soll den Schutz durch Transparenzpflichten stärken. Außerdem wird auf Deepfakes eingegangen, die Urheber- oder Persönlichkeitsrechte verletzen können.</p>", 1008, false, false, "/images/makerspace/b60ab4cc-5ab8-4550-a808-0a6472c249dc.jpg", "https://www.bmjv.de/SharedDocs/Downloads/DE/Themen/Nav_Themen/240305_FAQ_KI_Urheberrecht.pdf?__blob=publicationFile&v=2", "", "Künstliche Intelligenz und Urheberrecht - Fragen und Antworten", false, false, false, false, false, false },
                    { 1009, false, "<p>Die Webseite <a href=https://ai-act-law.eu/de/>ai-act-law.eu/de</a> bietet den vollständigen deutschen Text der EU-KI-Verordnung (AI Act) 2024/1689, die ab August 2026 gilt. Die Verordnung legt harmonisierte Regeln für KI-Systeme fest, insbesondere für Hochrisiko-Anwendungen mit strengeren Anforderungen an Dokumentation, Risikomanagement und Transparenz. Anbieter und Betreiber müssen sich mit diesen Pflichten vertraut machen. Die Seite ermöglicht eine übersichtliche Navigation durch die Verordnung und dient als wichtige Informationsquelle zu den rechtlichen Vorgaben für KI in der EU.</p>", 1009, false, false, "/images/makerspace/2402351d-adac-4487-8f3c-18b4e8647141.jpg", "https://ai-act-law.eu/de/", "", "Die KI-Verordnung von ", false, false, false, false, false, false },
                    { 1010, false, "<p>Das Papier behandelt die Frage, wie das Urheberrecht auf Inhalte anzuwenden ist, die mithilfe Künstlicher Intelligenz (KI) erstellt wurden, und beleuchtet dabei sowohl die Herausforderungen als auch die Chancen für den Bildungsbereich. KI ist zunehmend Teil unseres Alltags - beispielsweise durch Sprachassistenten oder Programme, die automatisch Texte und Bilder erzeugen. Besonders im schulischen Kontext eröffnen sich dadurch neue Möglichkeiten, etwa bei der Erstellung von Unterrichtsmaterialien oder der Nutzung und Entwicklung von Open Educational Resources (OER). Gleichzeitig entsteht jedoch ein erhöhter Bedarf an rechtlicher Orientierung: Lehrkräfte müssen lernen, wie sie mit urheberrechtlichen Fragen im Zusammenhang mit KI-generierten Inhalten verantwortungsvoll umgehen.</p>", 1010, false, false, "/images/makerspace/d0bacbc1-f0c5-44c8-969d-1676575f8918.jpg", "https://www.iit-berlin.de/wp-content/uploads/2025/02/2025-03-iit-perspektive-Urheberrecht_KI_Beitraege_V05.pdf", "", "Urheberrecht bei KI-generierten Beiträgen: Handlungsbedarfe und Nutzungschancen für den Bildungskontext (Working Paper des Instituts für Innovation und Technik (iit) Nr. 78)", false, false, false, false, false, false },
                    { 1011, false, "<p>Das Werk gibt einen kompakten Überblick über zentrale rechtliche Fragen zur Nutzung generativer KI an Hochschulen in NRW. Es behandelt Urheberrechte beim KI-Training und bei der Inhaltserstellung, beleuchtet Persönlichkeitsrechte und Datenschutz und geht auch auf arbeitsrechtliche Aspekte im Hochschulkontext ein.</p>", 1011, false, false, "/images/makerspace/43d85f27-f10e-4046-b1bb-3a23e71a523b.jpg", "https://kiconnect.pages.rwth-aachen.de/pages/download/docs/040_Rechtsfragen_Allgemein_KIconnectNRW_aktuell.pdf", "", "Allgemeine Rechtsfragen zum Umgang mit generativen KI-Diensten in der Version 1.0 von 2024-07-10 (Ki:content.NRW)", false, false, false, false, false, false },
                    { 1012, false, "<p>Dieser Leitfaden zeigt auf, wie textgenerierende KI-Systeme wie ChatGPT im schulischen Kontext sinnvoll genutzt werden können. Er erklärt, wie diese Systeme funktionieren, beleuchtet rechtliche und praktische Rahmenbedingungen für ihren Einsatz im Unterricht und thematisiert die damit verbundenen Chancen und Herausforderungen für Lehrkräfte und Schüler*innen sowie weitere relevante Aspekte.</p>", 1012, false, false, "/images/makerspace/a7adde30-7cc6-4a85-acaf-4ff73a075033.jpg", "https://www.schulministerium.nrw/system/files/media/document/file/handlungsleitfaden_ki_msb_nrw_230223.pdf", "", "Umgang mit textgenerierenden KI-Systemen - Ein Handlungsleitfaden (Ministerium für Schule und Bildung NRW)", false, false, false, false, false, false },
                    { 1013, false, "<p>Die Empfehlungen zeigen auf, wie Künstliche Intelligenz (KI) an der Universität Duisburg-Essen (UDE) sinnvoll in Studium und Lehre integriert werden kann. Sie behandeln Chancen und Grenzen von KI-Tools im Hinblick auf Texte, Urheberrecht und Datenschutz, legen den Fokus auf die Förderung von KI-Kompetenzen bei Studierenden und thematisieren den Umgang mit KI in Prüfungen und wissenschaftlichem Arbeiten - inklusive neuer Prüfungsformate.</p>", 1013, false, false, "/images/makerspace/733ebd42-c0d6-42cd-a1ad-fe3ce5df8435.jpg", "https://www.uni-due.de/imperia/md/content/e-learning/strategie/ki_in_studium_und_lehre_-_empfehlungen_zum_umgang_an_der_ude_v1.0.pdf", "", "KÜNSTLICHE INTELLIGENZ IN STUDIUM UND LEHRE - Empfehlungen zum Umgang an der UDE (Uni Duisburg Essen)", false, false, false, false, false, false },
                    { 1014, false, "<p>Dieser Text untersucht zentrale urheberrechtliche Fragestellungen im Zusammenhang mit Künstlicher Intelligenz und maschinellem Lernen. Er erläutert die rechtliche Einordnung von Trainingsdaten und Lizenzierungen, analysiert einschlägige Ausnahmen wie das Text- und Data-Mining und geht auf den Schutz von Algorithmen, Software und KI-generierten Inhalten ein.</p>", 1014, false, false, "/images/makerspace/7529792d-fdcd-42e9-b25f-3dd0ae864274.jpg", "https://www.bundestag.de/resource/blob/592106/74cd41f0bd7bc5684f6defaade176515/wd-10-067-18-pdf-data.pdf", "", "Künstliche Intelligenz und Machine Learning - Eine urheberrechtliche Betrachtung - Deutscher Bundestag WD 10 - 3000 - 67/18", false, false, false, false, false, false },
                    { 1015, false, "<p>Diese Handreichung bietet praktische Hinweise zum Einsatz generativer KI im Studium. Sie zeigt, was erlaubt ist, wie KI-Tools sinnvoll eingesetzt werden können und welche Risiken zu beachten sind. Im Fokus stehen klare Absprachen mit Lehrenden, der unterstützende Einsatz von KI beim Lernen, Schreiben und Organisieren sowie die Notwendigkeit einer kritischen Reflexion, da KI nicht immer verlässliche Informationen liefert.</p>", 1015, false, false, "/images/makerspace/464fa0ec-f359-4e3d-aab0-4464bd297063.jpg", "https://lehre-virtuell.uni-frankfurt.de/knowhow/einsatz-von-generativer-ki-im-studium-handlungsempfehlungen-fuer-studierende/", "", "Einsatz von generativer KI im Studium - Handlungsempfehlungen für Studierend (Goethe Universität Frankfurt am Main) ", false, false, false, false, false, false },
                    { 1018, false, "<p>Die Ausarbeitung gibt einen umfassenden Überblick über urheberrechtliche Fragestellungen im Kontext des Trainings generativer KI-Modelle. Es behandelt die rechtliche Bewertung der Nutzung urheberrechtlich geschützer Werke als Trainingsdaten, insbesondere im Hinblick auf die urheberrechtlichen Schranken, wie Text- und Data Mining, sowie auf Fragen der Vervielfältigung und der Schutzfähigkeit von KI-generierten Werken. Zudem werden Risiken im Zusammenhang mit der möglichen Memorierung geschützter Inhalte, die zu Lizenzverletzungen führen könnten, analysiert. Das Dokument diskutiert weiterhin die rechtlichen Unsicherheiten bei der Verwendung von Trainingsdaten, die mit bestehenden und geplanten europäischen Regelungen, wie der KI-Verordnung, verknüpft sind. Insgesamt bietet es eine differenzierte Betrachtung der Herausforderung, Innovation im KI-Bereich mit gesetzlichen Vorgaben in Einklang zu bringen.</p>", 1018, false, false, "/images/makerspace/bf1a0099-ca9c-4193-870c-6af535339820.jpg", "https://www.nomos-elibrary.de/de/10.5771/9783748949558/urheberrecht-und-training-generativer-ki-modelle?page=1", "", "Urheberrecht und Training generativer KI-Modelle", false, false, false, false, false, false },
                    { 1019, false, "<p>PromptBase ist ein Online-Marktplatz, auf dem Nutzer KI-Prompts kaufen und verkaufen können - für Tools wie ChatGPT, Midjourney, DALL·E, Stable Diffusion u.v.m. Dort findest du über 200.000 kuratierte Prompts von Expert:innen</p>", 1019, false, false, "/images/makerspace/834b6192-2ad2-4c27-80cc-d00b52db90c7.jpg", "https://promptbase.com/", "Prompt", " PromptBase", false, false, false, false, false, false },
                    { 1020, false, "<p>Sora ist ein von OpenAI entwickeltes KI-Modell für Text-zu-Video. Es kann moderne Kurzvideos direkt aus textuellen Beschreibungen erstellen und bestehende Videos verlängern oder neu bearbeiten.</p>", 1020, false, false, "/images/makerspace/64b475bb-2e29-4d4f-a0fc-bba7f5176a9b.jpg", "https://sora.chatgpt.com/", "Bilder,Video", "Sora", false, false, false, false, false, false },
                    { 1021, false, "<p>Die <strong>Academic Cloud</strong> ist das zentrale Portal für Universitäten, Hochschulen und Forschungseinrichtungen. Nach dem Login erhalten Nutzende Zugriff auf digitale Speicherplätze, gemeinsame Dateibearbeitung (inkl. LaTeX), Umfrage-Tools, PIDs für Publikationen, Kurz-URL-Generatoren sowie Chat- und Videokonferenzräume. Ein zentrales Angebot ist <strong>Chat AI</strong> - ein einfacher und sicherer Zugang zu leistungsstarker generativer KI. Über die intuitive Oberfläche können Nutzende direkt mit verschiedenen KI-Modellen chatten und so Forschung, Studium und Lehre effizient unterstützen.</p>", 1021, true, false, "/images/makerspace/95a56719-db66-4374-beec-17ce58cd5e80.jpg", "https://academiccloud.de/", "LLM-basierten Systeme", "Academinc Cloud", true, false, false, false, false, false },
                    { 1022, false, "<p>Die Veröffentlichung von Schmohl, Watanabe und Schelling (2023) gibt einen umfassenden Überblick über die Integration von Künstlicher Intelligenz (KI) in der Hochschulbildung. Er beleuchtet verschiedene Einsatzmöglichkeiten wie Learning Analytics, individualisierte Lernmaterialien und die Unterstützung forschenden Lernens. Dabei werden sowohl Chancen als auch Herausforderungen, insbesondere ethische Aspekte, Akzeptanzfragen und Transparenz von KI-Systemen, ausführlich diskutiert. Der Beitrag liefert wertvolle Impulse für die zukünftige Gestaltung von Lehre und Lernen mit KI.</p><p><strong>Autoren:</strong><br>Schmohl, Tobias [Hrsg.]; Watanabe, Alice [Hrsg.]; Schelling, Kathrin [Hrsg.]<br><i>Künstliche Intelligenz in der Hochschulbildung. Chancen und Grenzen des KI-gestützten Lernens und Lehrens.</i><br>Bielefeld: transcript 2023, 283 S. - (Hochschulbildung: Lehre und Forschung; 4)</p>", 1022, false, false, "/images/makerspace/5ee86348-21f5-4a88-a794-c6fa63612700.jpg", "https://www.pedocs.de/volltexte/2023/26427/pdf/Schmohl_Watanabe_Schelling_2023_Kuenstliche_Intelligenz.pdf", "", "Künstliche Intelligenz in der Hochschulbildung", false, false, false, false, false, false },
                    { 1023, false, "<p>Die Webseite der Bundesnetzagentur informiert umfassend über Künstliche Intelligenz (KI) in den von ihr regulierten Bereichen wie Telekommunikation, Energie und Post. Sie bietet Unterstützung bei der Umsetzung der EU-KI-Verordnung und fördert den verantwortungsvollen Einsatz von KI. Zudem organisiert die Agentur Formate wie das KI-Café, um den Austausch zwischen verschiedenen Akteuren zu ermöglichen und aktuelle Themen zu diskutieren.</p>", 1023, false, false, "/images/makerspace/ddd4317f-ad9b-4e03-a792-1c91e8a7914d.jpg", "https://www.bundesnetzagentur.de/DE/Fachthemen/Digitales/KI/start_ki.html", "", "Umsetzung der EU-KI-Verordnung", false, false, false, false, false, false },
                    { 1024, false, "<p><strong>Künstliche Intelligenz: Forschung am Fraunhofer IKS </ strong > auf der Fraunhofer Institute - Website gibt einen umfassenden Überblick,was das Institut im Bereich KI - Forschung anbietet:</ p > ", 1024, false, false, "/images/makerspace/d1e9e9cc-74d5-47c2-bbb5-b973ee311de0.jpg", "https://www.iks.fraunhofer.de/de/themen/kuenstliche-intelligenz/kuenstliche-intelligenz-forschung.html", "", "Künstliche Intelligenz: Forschung am Fraunhofer IKS", false, false, false, false, false, false },
                    { 1025, false, "<p>Die Fraunhofer-IKS-Seite <strong>Künstliche Intelligenz in der Medizin</strong> zeigt, wie KI-Technologien sicher und praxisnah im Gesundheitswesen eingesetzt werden können. Sie stellt Projekte vor wie generative KI für Wissensmanagement, intelligente Personaleinsatzplanung, datengestützte Frühversorgung von Frühgeborenen und Frameworks zur rechtssicheren Validierung medizinischer KI-Systeme - stets mit Fokus auf Sicherheit, Effizienz und Patientennutzen.</p>", 1025, false, false, "/images/makerspace/383a014e-c714-4991-8f91-5743c10e9ffe.jpg", "https://www.iks.fraunhofer.de/de/themen/kuenstliche-intelligenz/kuenstliche-intelligenz-medizin.html", "", "Künstliche Intelligenz in der Medizin", false, false, false, false, false, false },
                    { 1026, false, "<p>Der Beitrag beschreibt innovative didaktische Ansätze für digitale Lernangebote am Beispiel des <strong>KI-Campus</strong>. Es zeigt, wie Online-Plattformen durch interaktive, flexible und personalisierte Lernformate gezielt KI-Kompetenzen fördern können, und gibt Empfehlungen für zukunftsfähige digitale Bildungsformate.</p>", 1026, false, false, "/images/makerspace/9ed1aaf6-adcb-448c-b764-854ec799382c.jpg", "https://link.springer.com/chapter/10.1007/978-3-658-32849-8_34", "", "Zukunftsfähige Formate für digitale Lernangebote", false, false, false, false, false, false },
                    { 1027, false, "<p>Der Beitrag beschreibt die Entwicklung des regelbasierten Chatbots <strong>OSABot</strong> an der Technischen Hochschule Nürnberg. Ziel ist es, Studienanfänger:innen schon zu Beginn ihres Studiums bei der Organisation des Lernens und beim Aufbau von study skills zu unterstützen. Grundlage bildet ein psychologisch fundierter Test zur Studierfähigkeit, aus dem der Chatbot personalisierte Tipps - etwa zu Zeitplanung oder Lernmethoden - ableitet. Durch dialogische Interaktionen soll er zugleich Motivation und Selbstreflexion fördern. Nach einer Erprobungsphase ist der dauerhafte Einsatz vorgesehen.</p>", 1027, false, false, "/images/makerspace/ddbe3235-8420-4b35-8cc4-a8f20a2af012.jpg", "https://www.pedocs.de/volltexte/2023/27835/pdf/Helten_et_al_2023_Wie_kann_ich_dich_unterstuetzen.pdf", "", "Wie kann ich dich unterstützen?. Chatbot-basierte Lernunterstützung für Studienanfänger:innen", false, false, false, false, false, false },
                    { 1030, false, "<p>Das <strong>Rechtsgutachten von Prof. Dr. Thomas Hoeren</strong> analysiert die Bedeutung der europäischen KI-Verordnung für Hochschulen. Es klärt, dass das Wissenschaftsprivileg nur für rein forschungsbezogene KI-Systeme gilt, während bei Praxiseinsatz die Verordnung ab Inbetriebnahme greift. Hochschulen müssen KI-Kompetenz bei Beschäftigten sicherstellen, bei Studierenden nur bei geforderter Nutzung; Schulungen sind flexibel, aber verbindlich. Hochschulen sind meist Betreiber, selten Anbieter von KI-Systemen, wobei Anbieterstatus durch Entwicklung oder wesentliche Anpassung entsteht. Hochrisiko-Anwendungen, wie Bewertung von Lernergebnissen, unterliegen strengen Vorgaben, außer bei unverbindlichem Feedback. Learning Analytics sind nur bei komplexen, anpassungsfähigen Systemen KI-Systeme. Open-Source-KI hat begrenzte Ausnahmen, da Hochrisiko-Systeme reguliert bleiben. Das Gutachten, entstanden im Projekt KI:edu.nrw, bietet Hochschulen Orientierung für den rechtskonformen Umgang mit der KI-Verordnung.</p>", 1030, false, false, "/images/makerspace/0bac6777-d88e-436a-8aa4-a6a5f18324d0.jpg", "https://www.itm.nrw/wp-content/uploads/2025/08/KI-edu-nrw_Rechtsgutachten-zur-Bedeutung-der-europaeischen-KI-Verordnung-fuer-Hochschulen.pdf", "", "Rechtsgutachten zur Bedeutung der europäischen KI-Verordnung für Hochschulen", false, false, false, false, false, false },
                    { 1031, false, "<p>Erstellt komplette Songs mit Gesang und Instrumenten aus einem Textprompt.</p>", 1031, false, false, "/images/makerspace/beae188a-492e-42ea-992d-7edf95e3f954.jpg", "https://www.udio.com/", "Musik", "Udio", false, false, false, false, false, false },
                    { 1032, false, "<p>Wandelt Texte in fertige Songs um (inkl. Vocals).</p>", 1032, false, false, "/images/makerspace/04f1aee9-290b-4313-8cdf-5a41a6bdab97.jpg", "https://suno.com/home", "Musik", "Suno AI", false, false, false, false, false, false },
                    { 1033, false, "<p>Der Einstieg: Stil wählen ␦ Song generieren - fertig! Erstellte Songs können auch auf Spotify veröffentlicht werden.</p>", 1033, false, false, "/images/makerspace/37bd0eda-645b-4c84-af3b-31e80302e3ec.jpg", "https://boomy.com", "Musik", "Boomy", false, false, false, false, false, false },
                    { 1034, false, "<p>Erstellt automatisch passende Hintergrundmusik in unterschiedlichen Stilen. Ist geeignet für private Projekte und Social Media Clips.<br>Für professionelle, kommerzielle Einsätze ist ein kostenpflichtiges Abo nötig.</p>", 1034, false, false, "/images/makerspace/28064572-2e46-4738-be24-eb4e2f194ad1.jpg", "https://mubert.com/", "Musik", "Mubert", false, false, false, false, false, false },
                    { 1035, false, "<p>Erzeugt Musik aus Text-Eingaben, indem Klangbilder in hörbare Sounds umgewandelt werden. Die Ergebnisse eignen sich gut für kreative Ideen (Open-Source).</p>", 1035, false, false, "/images/makerspace/fa093163-49eb-45fa-8080-187a31bad9f5.jpg", "https://www.producer.ai/", "Musik", "Riffusion", false, false, false, false, false, false },
                    { 1036, false, "<p>Bietet die Generierung von Tracks in verschiedenen Genres.</p>", 1036, false, false, "/images/makerspace/f20bdeaa-814b-488b-952c-1a14b89d28c2.jpg", "https://www.loudly.com", "Musik", "Loudly", false, false, false, false, false, false },
                    { 1037, false, "<p>Die Website stellt den <strong>Prompt Report</strong> vor - eine umfassende Analyse von Methoden des Prompt-Engineerings für generative KI. Darin werden eine Klassifikation von 58 Prompting-Ansätzen für Sprachmodelle sowie 40 zusätzliche Varianten beschrieben. Außerdem enthält der Bericht Empfehlungen und Leitlinien für erfolgreiches Prompting, inklusive praxisnaher Hinweise für moderne Systeme wie ChatGPT. Ziel der Untersuchung ist es, Begriffe zu präzisieren und ein klares, systematisches Verständnis zu vermitteln.</p>", 1037, true, false, "/images/makerspace/dbc6ba03-eb42-446f-ae03-e22e986f67a1.jpg", "https://arxiv.org/pdf/2406.06608", "", "Studie: Prompt Report", false, false, false, false, false, false },
                    { 1038, false, "<p>Emergent ist ein agentic vibe-coding Tool, das es erlaubt, vollständig funktionsfähige, produktionsbereite Anwendungen aus einfachen Textanweisungen zu erstellen - ganz ohne selbst zu programmieren.</p>", 1038, false, false, "/images/makerspace/c96f823e-d96c-4e8e-af84-443bc4473637.jpg", "https://app.emergent.sh/", "Programmieren", "Emergent", false, false, false, false, false, false },
                    { 1039, false, "<p><strong>Poe</strong> ist ein von Quora entwickelter Dienst, der den Zugang zu unterschiedlichen Sprach-KI-Modellen über eine zentrale Plattform bündelt. Nutzerinnen und Nutzer können damit je nach Bedarf zwischen verschiedenen Assistenten wechseln und ihre Stärken vergleichen. Poe ermöglicht die Erstellung eigener Bots, die auf vorhandenen Modellen aufsetzen und individuell angepasst werden können.</p>", 1039, false, false, "/images/makerspace/4627846d-0649-46c5-8e60-df89db3b6f63.jpg", "https://poe.com/", "LLM-basierten Systeme", "Poe", false, false, false, false, false, false },
                    { 1040, false, "<p>Google AI Studio ist eine Plattform von Google, die den Zugang zu großen Sprachmodellen wie Gemini ermöglicht. Nutzer können damit KI-gestützte Anwendungen erstellen, Texte analysieren oder generative Inhalte erzeugen. Die Plattform dient als Schnittstelle zwischen Entwicklern und den zugrundeliegenden LLM-Technologien und erleichtert die Integration in eigene Projekte.</p>", 1040, false, false, "/images/makerspace/20364079-f5f6-4bbd-b6c9-e4472f3a139f.jpg", "https://aistudio.google.com/", "LLM-basierten Systeme", "Google AI Studio", false, false, false, false, false, false },
                    { 1041, false, "<p>Grok ist ein Large Language Model, das von Meta entwickelt wurde und für Textverarbeitung, Konversation und kreative Inhalte eingesetzt wird. Es kann komplexe Texte verstehen, zusammenfassen und neue Inhalte generieren.</p>", 1041, false, false, "/images/makerspace/057243d6-c790-46dc-8b1e-0a28eaece4c7.jpg", "https://grok.com/", "LLM-basierten Systeme", "Grok", false, false, false, false, false, false },
                    { 1042, false, "<p>Microsoft Copilot ist ein KI-Assistent, der in Office-Anwendungen und den Edge-Browser integriert ist und auf LLM-Technologie basiert. Er unterstützt beim Schreiben, Analysieren und Zusammenfassen von Texten sowie bei Automatisierungen. Copilot nutzt dabei große Sprachmodelle, um produktive Arbeitsprozesse zu vereinfachen.</p>", 1042, false, false, "/images/makerspace/0a1cc7cc-170e-4986-aa50-4fc665ade4f1.jpg", "https://copilot.microsoft.com/", "LLM-basierten Systeme", "Microsoft Copilot", false, false, false, false, false, false },
                    { 1043, false, "<p>Claude ist ein von Anthropic entwickeltes Large Language Model, das natürliche Sprache versteht und generiert. Es wird für Chatbots, Textanalysen und kreative Inhalte eingesetzt. Nutzer können Claude über verschiedene Plattformen in Anwendungen integrieren, um KI-gestützte Aufgaben effizient zu erledigen.</p>", 1043, false, false, "/images/makerspace/72469555-768c-4533-8ad3-df2e3e7ee24b.jpg", "https://claude.ai/", "LLM-basierten Systeme", "Claude", false, false, false, false, false, false },
                    { 1044, false, "<p>ChatGPT ist ein KI-gestützter Chatbot von OpenAI, der auf großen Sprachmodellen basiert und natürliche Unterhaltungen führen kann. Er unterstützt beim Schreiben, Recherchieren und Beantworten komplexer Fragen. ChatGPT wird über Web, Apps oder APIs in verschiedenen Anwendungen integriert.</p>", 1044, false, false, "/images/makerspace/749c6a03-7d60-408a-b43b-14fb093ccb90.jpg", "https://chatgpt.com/", "LLM-basierten Systeme", "ChatGPT", false, false, false, false, false, false },
                    { 1045, false, "<p>Mistral ist ein Large Language Model, das für Textgenerierung und -verarbeitung entwickelt wurde. Es kann komplexe Texte verstehen, zusammenfassen und eigenständig Inhalte erstellen. Mistral wird sowohl direkt als Modell als auch über Plattformen genutzt, die auf LLM-Technologie basieren.</p>", 1045, false, false, "/images/makerspace/3b49c9c7-b80e-4102-ad72-051c5a43e49f.jpg", "https://mistral.ai/", "LLM-basierten Systeme", "Mistral", false, false, false, false, false, false },
                    { 1046, false, "<p>DeepSpeek ist eine KI-Anwendung, die sich auf Sprachverarbeitung und Text-zu-Sprache-Funktionen spezialisiert hat. Sie ermöglicht realistische Sprachausgabe und kann gesprochene Inhalte analysieren oder generieren. Die Plattform wird häufig für Kommunikation, Lernanwendungen und Content-Erstellung eingesetzt.</p>", 1046, false, false, "/images/makerspace/4a5328e9-1b6b-4617-ad54-f60eaee1389f.jpg", "https://www.deepseek.com/", "LLM-basierten Systeme", "DeepSpeek", false, false, false, false, false, false },
                    { 1047, false, "<p><strong>TinyWow</strong> ist eine webbasierte Plattform, die eine Vielzahl von <strong>kostenlosen Online-Tools</strong> anbietet.&nbsp;</p><p>Dazu gehören u. a.:</p><ul><li><strong>PDF-Tools</strong> (komprimieren, konvertieren, zusammenführen)</li><li><strong>Video- und Bildbearbeitung</strong> (konvertieren, zuschneiden, komprimieren)</li><li><strong>Dateikonvertierung</strong> (Word  PDF, Video  MP3 usw.)</li><li><strong>Online-Utilities</strong> wie Screenshots, Meme-Generatoren, Texttools</li></ul>", 1047, false, false, "/images/makerspace/4f7fa365-8fcd-42a5-94ee-0623ec5a2274.jpg", "https://tinywow.com/", "Multimodal", "Tinywow", false, false, false, false, false, false },
                    { 1048, false, "<p><strong>Remove.photos </strong>ist eine App zum schnellen Entfernen von Bildhintergründen. Es erstellt automatisch transparente PNGs und ermöglicht das Ersetzen oder Bearbeiten von Hintergründen.</p><p><strong>Kernfunktionen:</strong></p><p>Automatisches Entfernen von Bildhintergründen Erstellen von transparenten PNG-Bildern Einfache Bildbearbeitung (neuen Hintergrund einsetzen, Bild aufhellen) Entfernen von Vordergrundobjekten Kostenfrei und ohne Registrierung nutzbar</p>", 1048, false, false, "/images/makerspace/cc022bab-fc56-4285-8cca-ca2b07a0c426.jpg", "https://remove.photos/", "Bilder", "Remove.Photos", false, false, false, false, false, false },
                    { 1049, false, "<p>TidyText.cc ist ein kostenloses Online-Tool zur Bereinigung von Textausgaben aus KI-Generatoren wie ChatGPT. Es entfernt automatisch unerwünschte Formatierungen wie Markdown, HTML-Tags, Fußnoten oder Sonderzeichen. Der Text wird in ein sauberes, strukturiertes Format umgewandelt, das sich direkt in Google Docs oder Microsoft Word einfügen lässt.</p><p><strong>Kernfunktionen:</strong></p><ul><li>Automatisches Entfernen von Markdown- und HTML-Formatierungen</li><li>Bereinigung von Fußnoten und Sonderzeichen</li><li>Umwandlung in sauberes, strukturiertes Textformat</li><li>Kostenlose Nutzung ohne Registrierung</li></ul>", 1049, false, false, "/images/makerspace/9edd47b4-fd90-430f-abc3-b7f9e40b025d.jpg", "https://tidytext.cc/", "Plagiatprüfung,Schreibprozesse", "TidyText.cc", false, false, false, false, false, false },
                    { 1050, false, "<p><strong>SoSciSurvey </strong>- View Chars ist ein kostenloses Online-Tool zur Erkennung unsichtbarer oder nicht druckbarer Zeichen in Texten. Es identifiziert versteckte Unicode-Zeichen wie Leerzeichen, Steuerzeichen oder Formatierungen, die beim Kopieren und Einfügen entstehen können. Die Darstellung der Zeichen in Unicode ermöglicht eine genaue Analyse und erleichtert die Bereinigung von Texten, etwa für Online-Umfragen oder Datenverarbeitung.</p><p><strong>Kernfunktionen:</strong></p><ul><li>Erkennung von unsichtbaren und nicht druckbaren Unicode-Zeichen</li><li>Identifikation von Steuerzeichen und versteckten Formatierungen</li><li>Unterstützung bei der Textbereinigung für Datenanalyse</li><li>Kostenlos nutzbar, keine Registrierung erforderlich</li></ul><p>Dieses Tool ist besonders nützlich für Entwickler und Autoren, die mit Textdaten arbeiten und sicherstellen möchten, dass ihre Daten frei von unsichtbaren Störungen sind.</p>", 1050, false, false, "/images/makerspace/ef5526ea-b7ab-4f7c-8345-7cc89e378c73.jpg", "https://www.soscisurvey.de/tools/view-chars.php", "Plagiatprüfung,Schreibprozesse", "Soscisurvey", false, false, false, false, false, false }
                });

            migrationBuilder.InsertData(
                table: "MitmachenContents",
                columns: new[] { "Id", "Content", "DisplayOrder", "SectionType", "Title" },
                values: new object[,]
                {
                    { 2, "Das MediaLab der Bergischen Universität Wuppertal ist ein inspirierender Makerspace, die kreativen Köpfe aus unterschiedlichen Disziplinen zusammenbringt. Hier haben Studierende, Lehrende und Forschende die Möglichkeit, ihre Ideen in die Praxis umzusetzen, Prototypen zu entwickeln und neue Technologien auszuprobieren. Mit einer gut ausgestatteten Infrastruktur und einem interdisziplinären Ansatz bietet das MediaLab einen idealen Raum, um an zukunftsorientierten Projekten zu arbeiten und gemeinsam innovative Lösungen zu entwickeln.", 0, "Card", "Von der Idee zur Umsetzung: Projekte im MediaLab" },
                    { 3, "Ihre Ideen realisieren: Nutzen Sie das MediaLab, um technologische, kreative oder didaktische Ansätze in Form von Prototypen zu verwirklichen. Von der Skizze zum Modell: Entwickeln Sie konkrete Prototypen, die Ihre Konzepte anschaulich machen und eine Weiterentwicklung erleichtern. Innovative Lösungen testen: Experimentieren Sie mit neuen Ansätzen und Technologien, um ihre Praxistauglichkeit zu evaluieren. Förderprojekte mit Prototypen stärken: Prototypen als Grundlage: Entwickeln Sie funktionale Modelle, die Förderprojekten eine klare und überzeugende Basis bieten. Lehrmethoden modellieren: Erstellen Sie Prototypen für digitale Tools, didaktische Konzepte oder virtuelle Formate. Testen und optimieren: Evaluieren Sie die Wirksamkeit Ihrer Ideen in einer experimentellen Umgebung, bevor sie in der Lehre angewendet werden. Didaktische Konzepte visualisieren: Praxisnah und anschaulich: Entwickeln Sie Prototypen, die komplexe didaktische Ansätze verständlich machen. Gemeinsam gestalten: Arbeiten Sie mit Kolleg*innen, Forschenden und Studierenden zusammen, um Konzepte zu entwickeln, die den Anforderungen der digitalen Transformation gerecht werden.", 0, "Accordion", "Für Lehrende – Ihre Chancen im MediaLab" },
                    { 4, "Praktika und Abschlussarbeiten: Nutzen Sie das MediaLab als Ausgangspunkt für spannende Themen, die Praxis und Wissenschaft verbinden. Entwickeln Sie innovative Lösungen und lassen Sie Ihre Abschlussarbeit Teil eines realen Projekts werden. Seminararbeiten: Arbeiten Sie im Rahmen Ihrer Seminare an praxisnahen Aufgaben, die interdisziplinäre Ansätze fördern und kreative Technologien nutzen. Ihre Arbeit kann Impulse für zukünftige Projekte setzen. Hilfskraftstellen: Engagieren Sie sich als studentische Hilfskraft im MediaLab und unterstützen Sie spannende Projekte. Dabei erweitern Sie Ihre Kenntnisse in einem inspirierenden Umfeld und sammeln wertvolle Praxiserfahrung. Einfach aus Interesse: Haben Sie eine eigene Idee oder möchten Sie Teil eines kreativen Teams sein? Das MediaLab bietet Ihnen Raum, Unterstützung und eine Community, um Ihrer Leidenschaft nachzugehen – auch unabhängig von Ihrem Studium.", 0, "Accordion", "Für Studierende – Ihre Chancen im MediaLab" },
                    { 5, "Interdisziplinäre Projekte initiieren: Starten Sie eigene Forschungsvorhaben, die verschiedene Disziplinen miteinander verbinden. Das MediaLab fördert die Zusammenarbeit zwischen Fachbereichen und schafft Synergien für zukunftsweisende Lösungen. Technologische Innovationen erkunden: Nutzen Sie die Ausstattung und Expertise des MediaLabs, um mit vielfältigen Technologien zu experimentieren und innovative Forschungsansätze zu entwickeln. Förderprojekte realisieren: Das MediaLab bietet Unterstützung bei der Konzeption und Umsetzung von Drittmittelprojekten. Von der Antragstellung bis zur Durchführung – wir begleiten Sie bei jedem Schritt. Forschung sichtbar machen: Präsentieren Sie Ihre Ergebnisse auf dem Projekte-Portal und teilen Sie Ihre Arbeit mit einer breiten Community. Ihre Forschung wird Teil eines Netzwerks, das Innovation und Wissenstransfer fördert. Praxisorientierte Lösungen entwickeln: Arbeiten Sie an anwendungsorientierten Konzepten, die nicht nur in der Wissenschaft, sondern auch in Gesellschaft und Wirtschaft einen Unterschied machen.", 0, "Accordion", "Für Forschende – Ihre Möglichkeiten im MediaLab" },
                    { 6, "Eigene Projekte einreichen: Haben Sie eine Idee? Reichen Sie diese ein und setzen Sie sie gemeinsam mit dem MediaLab-Team um. Bestehende Projekte unterstützen: Schließen Sie sich laufenden Projekten an und bringen Sie Ihre Stärken ein. Angebote nutzen: Bewerben Sie sich auf Praktika, Hilfskraftstellen oder nutzen Sie das MediaLab als Basis für Ihre Abschlussarbeit. Jetzt aktiv werden und die digitale Zukunft mitgestalten: Auf Kontakt Verweisen Das MediaLab freut sich auf Ihre Ideen, Ihr Engagement und Ihre Neugier!", 0, "Accordion", "Wie können Sie mitmachen?" }
                });

            migrationBuilder.InsertData(
                table: "Modes",
                columns: new[] { "Id", "IconClass", "ImageUrl", "RouteType", "SortOrder", "Title" },
                values: new object[,]
                {
                    { 1, "bi bi-fonts", "/images/uploads/text-prompt.jpg", "Text", 1, "Text-Prompts" },
                    { 2, "bi bi-image", "/images/uploads/image-prompt.jpg", "Bild", 2, "Bild-Prompts" },
                    { 3, "bi bi-image", "/images/uploads/Video-Prompt.png", "Video", 3, "Video‑Prompts" },
                    { 4, "bi bi-image", "/images/uploads/Sound-Prompt.png", "Sound", 4, "Sound‑Prompts" },
                    { 5, "bi bi-image", "/images/uploads/Academics-Prompt.png", "Lehr", 5, "Lehr‑Prompts" },
                    { 6, "bi bi-image", "/images/uploads/meta-prompt.png", "Meta", 6, "Meta‑Prompts" }
                });

            migrationBuilder.InsertData(
                table: "PortalCards",
                columns: new[] { "Id", "Content", "DisplayOrder", "Title" },
                values: new object[,]
                {
                    { 1, "Das MediaLab des ZIM an der Bergischen Universität Wuppertal (BUW) ist ein zentraler Makerspace, der interdisziplinären Lehre und Forschung fördert. Mit unserem neuen Projekte-Portal möchten wir die Vielfalt und den Impact unserer Arbeit sichtbar machen. Das Portal bietet einen umfassenden Überblick über unsere interdisziplinären Projekte und zeigt unser Engagement für Transparenz, Nachhaltigkeit und die aktive Mitgestaltung der digitalen Transformation. Hier erhalten Sie Einblicke in die innovativen Projekte, die durch die Zusammenarbeit von Studierenden, Forschenden und Lehrenden entstehen.", 0, "Projekte und Ideen sichtbar machen" },
                    { 2, "Das MediaLab ist eine zentrale Anlaufstelle für Akteur*innen aus verschiedenen Disziplinen. Das Portal bietet eine sichtbare Plattform, die Kooperationspartner, Projektergebnisse und interdisziplinären Austausch in den Fokus stellt. So entstehen neue Impulse für Zusammenarbeit und Innovation.", 0, "Sichtbarkeit der Netzwerker" },
                    { 3, "Die Projekte im Portal repräsentieren nachhaltige Konzepte und praxisorientierte Innovationen, die über Disziplinen hinauswirken. Sie werden langfristig zugänglich gemacht, weiterentwickelt und in neue Kontexte übertragen, was den Wissenstransfer und die Förderung interdisziplinärer Expertise unterstützten.", 0, "Nachhaltige Konzepte für die Zukunft" },
                    { 4, "Von Open Educational Resources (OER)-Initiativen bis zu aktuellen Projekten wie „Kollaborativ Biodiversität entdecken“ zeigt das Portal die Vielfalt der MediaLab-Projekte. Diese verdeutlichen, wie innovative Technologien und interdisziplinäre Zusammenarbeit Lehre und Forschung bereichern.", 0, "Projektförderungen im Fokus" },
                    { 5, "Das Projekte-Portal ist eine zentrale Plattform, die Transparenz, Nachhaltigkeit und die Sichtbarkeit von Netzwerken fördert. Es zeigt, wie das MediaLab als Raum für Co-Creation und kreative Zusammenarbeit die digitale Zukunft der BUW aktiv mitgestaltet. Alle Universitätsangehörigen sind eingeladen, die Vielfalt der Projekte zu entdecken und gemeinsam an neuen Lösungen zu arbeiten.\r\n", 0, "Gemeinsam Zukunft gestalten" }
                });

            migrationBuilder.InsertData(
                table: "PortalVideo",
                columns: new[] { "Id", "ImagePath", "ShowImageInsteadOfVideo", "UploadDate", "VideoPath" },
                values: new object[] { 1, null, false, new DateTime(2025, 9, 10, 8, 44, 7, 893, DateTimeKind.Local).AddTicks(3951), "/videos/portal-intro.mp4" });

            migrationBuilder.InsertData(
                table: "PromptModels",
                columns: new[] { "Id", "RedirectUrl", "Titel" },
                values: new object[] { 1, "https://chat.openai.com/", "ChatGPT Default" });

            migrationBuilder.InsertData(
                table: "PromptWords",
                columns: new[] { "Id", "Text" },
                values: new object[,]
                {
                    { 1, "Analysiere" },
                    { 2, "Beurteile" },
                    { 3, "Benenne" },
                    { 4, "Definiere" },
                    { 5, "Ergänze" },
                    { 6, "Erkläre" },
                    { 7, "Formuliere" },
                    { 8, "Generiere" },
                    { 9, "Klassifiziere" },
                    { 10, "Kombiniere" },
                    { 11, "Leite ab" },
                    { 12, "Optimiere" },
                    { 13, "Passe an" },
                    { 14, "Reflektiere" },
                    { 15, "Schätze ein" },
                    { 16, "Simuliere" },
                    { 17, "Skizziere" },
                    { 18, "Strukturiere" },
                    { 19, "Überprüfe" },
                    { 20, "Verbinde" },
                    { 21, "Vergleiche" },
                    { 22, "Visualisiere" },
                    { 23, "Wähle aus" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "IconPath", "Title" },
                values: new object[] { 1, new DateTime(2025, 9, 10, 8, 44, 7, 893, DateTimeKind.Local).AddTicks(3629), "Object-oriented programming language by Microsoft", 1, null, "C#" });

            migrationBuilder.InsertData(
                table: "SliderItems",
                columns: new[] { "Id", "Description", "DisplayOrder", "ImageUrl", "IsForVirtuellesKlassenzimmer", "Title" },
                values: new object[,]
                {
                    { 1, "Globale Innovation trifft auf nachhaltiges Lernen <br /> erleben Sie immersive digitale Erlebnisse, die den Campus der Zukunft gestalten.", 0, "/images/FirstSlider1.png", false, "Virtueller Campus" },
                    { 2, "Fördern Sie internationale Zusammenarbeit und gestalten Sie die Zukunft  <br /> des digitalen Forschens in einem virtuellen Raum.", 0, "/images/FirstSlider2.png", false, "Virtueller Campus" },
                    { 3, "Tauchen Sie ein in die Welt der BUW  <br /> entdecken Sie die Universität interaktiv in einer virtuellen Rundreise.", 0, "/images/FirstSlider3.png", false, "360° BUW Tour" },
                    { 4, "Erkunden Sie die 'Gallery of Walk'  <br /> eine interaktive Reise durch nachhaltige Visualisierungen von Lehre und Forschung.", 0, "/images/FirstSlider4.png", false, "360°-Rundgang – GSA Konvent" },
                    { 5, "Erleben Sie innovative Posterpräsentationen  <br /> ein interaktiver 360°-Rundgang durch die neuesten Forschungsergebnisse.", 0, "/images/FirstSlider5.png", false, "360°-Rundgang – GSA Konvent" },
                    { 6, "Schützen Sie Salamander in einem innovativen virtuellen Game  <br /> ein interaktives Labor zur Rettung gefährdeter Amphibienarten.", 0, "/images/FirstSlider6.png", false, "Amphibienschutz in virtuellen 3D-Räumen" },
                    { 7, "Erforschen Sie den Schutz gefährdeter Amphibien durch forschendes Lernen im virtuellen Labor  <br /> ein einzigartiger Ansatz für Artenschutz.", 0, "/images/FirstSlider7.png", false, "Amphibienschutz in virtuellen 3D-Räumen" },
                    { 8, "Bildung für nachhaltige Entwicklung durch Open Educational Resources  <br /> Wissen für die Zukunft zugänglich und nachhaltig vermitteln.", 0, "/images/FirstSlider8.png", false, "BNE OER" },
                    { 9, "Testen Sie innovative Unterrichtsszenarien in virtuellen Lernräumen  <br /> die Mathematik der Zukunft erleben.", 0, "/images/FirstSlidera9.png", false, "Virtuelle Mathematik" },
                    { 10, "Erproben Sie didaktische Szenarien  <br /> entwickeln Sie hybride Lehr-Lernszenarien und erforschen Sie die Verbindung zwischen virtuellen und physischen Lernwelten.", 0, "/images/FirstSlider10.png", false, "Virtuelle Räume für die Bildung und Forschung" },
                    { 11, "Mit KI zur präzisen Identifikation von Salamandern  <br /> nachhaltig die Forschung und den Schutz gefährdeter Arten vorantreiben.", 0, "/images/FirstSlider11.png", false, "Salamander-KI-Mustererkennungssoftware" },
                    { 12, "Globale Innovation trifft auf nachhaltiges Lernen <br /> erleben Sie immersive digitale Erlebnisse, die den Campus der Zukunft gestalten.", 0, "/images/FirstSlider1.png", true, "Virtueller Campus" }
                });

            migrationBuilder.InsertData(
                table: "TechAnforderung",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 0, "Desktop-/Laptop-kompatibel" },
                    { 2, 0, "VR" }
                });

            migrationBuilder.InsertData(
                table: "UebersichtContent",
                columns: new[] { "Id", "ContentHtml" },
                values: new object[] { 1, "\r\n@{\r\n    ViewData[\"Title\"] = \"Übersicht\";\r\n}\r\n\r\n<div class=\"container mt-5\">\r\n    <div class=\"row justify-content-center\">\r\n        <div class=\"col-md-10\">\r\n\r\n            <!-- Title -->\r\n            <h2 class=\"mb-4\" style=\"color:#90bc14\">Übersicht der Projekte</h2>\r\n\r\n            <!-- Introduction -->\r\n            <p class=\"lead\">\r\n                Hier finden Sie eine Übersicht über die verschiedenen Projekte im MediaLab und BioVersum.\r\n            </p>\r\n\r\n            <p>\r\n                Jedes Projekt ist einzigartig und bietet spannende Einblicke in aktuelle Entwicklungen, Forschungsthemen und kreative Lösungen.\r\n            </p>\r\n\r\n            <!-- Sections -->\r\n            <h2 class=\"mt-5\" style=\"color:#90bc14\">Projektkategorien</h2>\r\n            <ul class=\"list-group list-group-flush mb-4\">\r\n                <li class=\"list-group-item\"><strong>Lehre:</strong> Projekte, die zur Unterstützung von Lehrveranstaltungen entwickelt wurden.</li>\r\n                <li class=\"list-group-item\"><strong>Forschung:</strong> Forschungsbasierte Projekte zur Entwicklung neuer Erkenntnisse und Methoden.</li>\r\n                <li class=\"list-group-item\"><strong>Studentische Arbeiten:</strong> Abschlussarbeiten, Seminarprojekte und Praktika.</li>\r\n                <li class=\"list-group-item\"><strong>Offene Projekte:</strong> Projekte, an denen Studierende und Lehrende gemeinsam arbeiten können.</li>\r\n            </ul>\r\n\r\n            <!-- Call to action -->\r\n            <h2 class=\"mt-5\" style=\"color:#90bc14\">Mitmachen und mehr erfahren</h2>\r\n            <p>\r\n                Wenn Sie mehr über ein Projekt erfahren oder sich beteiligen möchten, wenden Sie sich bitte an das MediaLab-Team.\r\n            </p>\r\n\r\n            <!-- Contact Section -->\r\n            <h2 class=\"mt-5\" style=\"color:#90bc14\">Kontakt</h2>\r\n            <p>\r\n                <strong>Dr. Heike Seehagen-Marx</strong><br>\r\n                <a href=\"mailto:h.seehagen-marx@uni-wuppertal.de\" class=\"text-decoration-none text-primary\">h.seehagen-marx@uni-wuppertal.de</a>\r\n            </p>\r\n\r\n        </div>\r\n    </div>\r\n</div>\r\n" });

            migrationBuilder.InsertData(
                table: "UrheberechtContents",
                columns: new[] { "Id", "ContentHtml", "DisplayOrder", "SectionType", "Title" },
                values: new object[,]
                {
                    { 1, "Dies ist der Einleitungstext für das Impressum.", 1, "Text", "Datenschutz Einleitung" },
                    { 2, "Name und Anschrift des Verantwortlichen...", 2, "Accordion", "Verantwortlich" }
                });

            migrationBuilder.InsertData(
                table: "fachgruppen",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 0, "Architektur" },
                    { 2, 0, "medialab" }
                });

            migrationBuilder.InsertData(
                table: "FilterItems",
                columns: new[] { "Id", "FilterCategoryId", "Info", "Instruction", "SortOrder", "Title" },
                values: new object[,]
                {
                    { 1, 1, "Künstliche Intelligenz", "Erstelle einen Project über Künstliche Intelligenz", 0, "KI" },
                    { 2, 1, "Mathematische Modelle", "Erstelle einen Project über Mathematische Modelle", 0, "Algorithmen" },
                    { 3, 2, "Pädagogisches Personal", "Erstelle einen Project über Pädagogisches Personal", 0, "Lehrer:innen" },
                    { 4, 2, "Lernende", "Erstelle einen Project über Lernende", 0, "Schüler:innen" },
                    { 5, 3, "Lehrvideos", "Erstelle einen Project über Lehrvideos", 0, "Video" },
                    { 6, 3, "Podcasts", "Erstelle einen Project über Podcasts", 0, "Audio" },
                    { 7, 4, "Teamarbeit fördern", "Erstelle einen Project über Teamarbeit fördern", 0, "Kollaborativ" },
                    { 8, 4, "Mitmachformate", "Erstelle einen Project über Mitmachformate", 0, "Interaktiv" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Applications", "CategoryId", "CategoryIds", "Conception", "ContentDevelopment", "CreativeCommons", "Design", "Development", "DidacticDesign", "DidacticDesignTeam", "Didactics", "DidaktischerAnsatz", "DisplayOrder", "Documents", "DokuLink", "DownloadURL", "Erfolgsmessung", "Evaluation", "EvaluationTeam", "Expertise", "FachgruppenId", "FachgruppenIds", "FakultaetId", "FakultaetIds", "Foerderung", "InteractionDesign", "IsEnabled", "IsVirtuellesKlassenzimmer", "KurzeBeschreibung", "Lizenz", "Materialien", "Media", "MediaDesign", "Methods", "Netzwerk", "OpenSource", "PrimaryTargetGroup", "Programming", "ProjectAcronym", "ProjectConception", "ProjectCoordination", "ProjectDevelopment", "ProjectGoals", "ProjectLeadership", "ProjectManagement", "ProjectPartners", "ProjectResponsibility", "ProjectSupport", "Recommendations", "References", "ReleaseYear", "Research", "ResearchTeam", "SoftwareDevelopers", "SoundDesign", "Status", "StoryDesign", "Tags", "TaxonomyLevel", "TechAnforderungId", "TechAnforderungIds", "ThreeDArtist", "Title", "UXDesign", "Url", "Version", "longDescription", "zusaetzlicheInformationen", "zusaetzlicheInformationen1" },
                values: new object[,]
                {
                    { 1, "Visual Studio, Unity", 1, "1", null, "Lehrmaterialien", "CC BY-SA", "Modern UI/UX", "Frontend und Backend", null, null, "Innovative Lernmethoden", null, 0, "https://example-docs.com", "https://docs.example.com", "https://download.example.com", null, "Qualitätskontrolle", null, "KI, VR", 1, "2", 1, "2", null, "Interaktive Elemente", false, false, "Ein innovativer Chatbot für KI-gestützte Kommunikation.", "Open Source", null, "https://example-media.com", "3D-Grafiken", "Agiles Management", "Kooperation C", "Ja", "Studierende", "C#, .NET, Unity", "KICB", "Konzeptionsphase", "Koordinierung der Module", "Team Alpha", "Förderung von KI-Kompetenzen", "John Doe", "Jane Smith", "Partner A, Partner B", "Max Mustermann", "Laufende Unterstützung", "Beste Praktiken", "https://example-references.com", 2024, "Angewandte Forschung", null, "Entwicklerteam X", "3D-Audio", 0, "Interaktive Storyline", "VR, KI", "Bloom Level 3", 1, "1", null, "KI-Chatbot", "Benutzerzentriert", "https://example.com", "1.0", "long description placeholder", null, null },
                    { 2, "Unity, Blender", 2, "2,3", null, "Virtuelle Szenarien", "Keine", "Futuristisches UI", "VR-Frontend und Backend", null, null, "Optional", null, 0, "https://vrraumplaner.example.com/documents", "https://vrraumplaner.example.com/docs", "https://vrraumplaner.example.com/download", null, "Benutzerstudien", null, "Raumplanung, VR-Engineering", 1, "2", 2, "1", null, "Hand-Tracking-Features", false, false, "Ein VR-Tool zur Raumplanung an Hochschulen.", "Proprietär", null, "https://vrraumplaner.example.com/media", "360°-Panoramen", "Scrum", "Bildungsnetzwerk VR", "Nein", "Hochschulen", "C#, Unity, SteamVR", "VRR", "Grundlegende Planungsphase", "Koordination VR-Module", "VR-Team", "Optimierte Raumplanung in VR", "Carla Schmidt", "Michael Braun", "Partner D, Partner E", "Anna Müller", "Wartung und Updates", "Regelmäßige Tests", "https://vrraumplaner.example.com/references", 2025, "Ergonomie und UX-Forschung", null, "Unity VR-Entwickler", "3D-Raumklang", 0, "Virtueller Rundgang", "VR,Raumplanung,Immobilien", "Bloom Level 4", 2, "1", null, "VR-Raumplaner", "Immersive Interaktionen", "https://vrraumplaner.example.com", "2.1", "Mit dem VR-Raumplaner können Studierende und Dozenten virtuell Seminarräume oder Labore an der Hochschule planen und testen.", null, null },
                    { 3, "VS Code, Docker", 3, "1,3", null, "Mechatronik-Kurse", "CC BY-NC", "Technische UI", "Raspberry Pi, Microcontroller", null, null, "Fernlehre-Konzepte", null, 0, "https://remotelab.example.com/documents", "https://remotelab.example.com/docs", "https://remotelab.example.com/download", null, "Pilotstudien", null, "Sensorik, Aktorik", 2, "1,1", 1, "2,2", null, "Steuerungs-Panel", false, false, "Eine Plattform zur Fernsteuerung mechatronischer Systeme.", "Open Source", null, "https://remotelab.example.com/media", "Diagramme, Schaltpläne", "Wasserfallmodell", "Forschungsnetzwerk MINT", "Ja, GitHub Repository", "Ingenieur-Studierende", "C++, Python", "RLM", "Konzeptphase Remote Access", "Koordination Lab-Hardware", "Mechatronik-Projektteam", "Praktische Laborerfahrung aus der Ferne", "Prof. Dr. Meier", "Projektmanagement-Team", "Partner X, Partner Y", "Peter Weber", "Regelmäßige Wartung", "Regelmäßige Integrationstests", "https://remotelab.example.com/references", 2023, "IoT-Forschung", null, "Python, Node.js", "Keine Audioeffekte", 0, "N/A", "Mechatronik,IoT,Fernsteuerung", "Bloom Level 2", 1, "2", null, "Remote-Labor Mechatronik", "Fernbedienung UI", "https://remotelab.example.com", "3.0 Beta", "Das Remote-Labor Mechatronik ermöglicht Studierenden, Maschinen und Sensoren über das Internet zu bedienen und Echtzeit-Daten auszuwerten.", null, null },
                    { 4, "PyCharm, Node.js", 1, "3", null, "Lernmaterial", "CC BY-SA", "Leicht bedienbare UI", "Backend mit Python, Frontend React", null, null, "KI-gestützte Lernstrategien", null, 0, "https://lernbuddy.example.com/documents", "https://lernbuddy.example.com/docs", "https://lernbuddy.example.com/download", null, "A/B-Tests", null, "Künstliche Intelligenz, Pädagogik", 2, "1", 2, "1", null, "Interaktive Quiz-Systeme", false, false, "Eine KI, die Schüler*innen personalisierte Lernpfade vorschlägt.", "Open Source", null, "https://lernbuddy.example.com/media", "Erklärvideos", "Scrum", "Bildungscloud", "Ja (GitHub)", "Schüler*innen 5.-10. Klasse", "Python, React, REST-APIs", "LBKI", "Didaktische Planung", "Aufgaben Koordination", "Lernbuddy Team", "Verbesserte Lernerfolge durch KI", "Prof. Dr. Schulz", "Projektbüro", "Partner M, Partner N", "Lisa König", "Fortlaufender Support", "Regelmäßige User-Feedback", "https://lernbuddy.example.com/references", 2025, "Lernpsychologie, Data Mining", null, "Team Beta", "Optionale Audio-Hinweise", 0, "Gamifizierte Lernpfade", "KI,Personalisierung,Bildung", "Bloom Level 3", 2, "1", null, "Lernbuddy KI", "Schülerorientiertes UX", "https://lernbuddy.example.com", "Final 2.3", "Lernbuddy KI analysiert Stärken und Schwächen von Lernenden und generiert passgenaue Lernempfehlungen und Übungsaufgaben.", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureCards_HomePageSettingsId",
                table: "FeatureCards",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterItems_FilterCategoryId",
                table: "FilterItems",
                column: "FilterCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeCards_HomePageSettingsId",
                table: "ModeCards",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectImages_ProjectId",
                table: "ProjectImages",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CategoryId",
                table: "Projects",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_FachgruppenId",
                table: "Projects",
                column: "FachgruppenId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_FakultaetId",
                table: "Projects",
                column: "FakultaetId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TechAnforderungId",
                table: "Projects",
                column: "TechAnforderungId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVideos_ProjectId",
                table: "ProjectVideos",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptVariations_PromptTemplateId",
                table: "PromptVariations",
                column: "PromptTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedPromptVariations_SavedPromptId",
                table: "SavedPromptVariations",
                column: "SavedPromptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "ContactEmail");

            migrationBuilder.DropTable(
                name: "ContactMessageSettings");

            migrationBuilder.DropTable(
                name: "DatenschutzContents");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "FeatureCards");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "FilterItems");

            migrationBuilder.DropTable(
                name: "Heroes");

            migrationBuilder.DropTable(
                name: "ImpressumContents");

            migrationBuilder.DropTable(
                name: "KontaktCards");

            migrationBuilder.DropTable(
                name: "LeichteSpracheContent");

            migrationBuilder.DropTable(
                name: "MakerSpaceDescriptions");

            migrationBuilder.DropTable(
                name: "MakerSpaceProjects");

            migrationBuilder.DropTable(
                name: "MitmachenContents");

            migrationBuilder.DropTable(
                name: "ModeCards");

            migrationBuilder.DropTable(
                name: "Modes");

            migrationBuilder.DropTable(
                name: "PortalCards");

            migrationBuilder.DropTable(
                name: "PortalVideo");

            migrationBuilder.DropTable(
                name: "ProjectImages");

            migrationBuilder.DropTable(
                name: "ProjectVideos");

            migrationBuilder.DropTable(
                name: "PromptModels");

            migrationBuilder.DropTable(
                name: "PromptVariations");

            migrationBuilder.DropTable(
                name: "PromptWords");

            migrationBuilder.DropTable(
                name: "RegistrationCodes");

            migrationBuilder.DropTable(
                name: "SavedPromptVariations");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "SliderItems");

            migrationBuilder.DropTable(
                name: "UebersichtContent");

            migrationBuilder.DropTable(
                name: "UrheberechtContents");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "FilterCategories");

            migrationBuilder.DropTable(
                name: "HomePageSettings");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "PromptTemplate");

            migrationBuilder.DropTable(
                name: "SavedPrompts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Fakultaet");

            migrationBuilder.DropTable(
                name: "TechAnforderung");

            migrationBuilder.DropTable(
                name: "fachgruppen");
        }
    }
}
