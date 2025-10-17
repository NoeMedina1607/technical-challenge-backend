using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseApp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class BDCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identification = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ComercialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaymentScheme = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EconomicActivity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GovernmentBranch = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Identification",
                table: "Companies",
                column: "Identification",
                unique: true);

            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.spCompany_Create','P') IS NOT NULL
                    DROP PROC dbo.spCompany_Create;
                ");

            migrationBuilder.Sql(@"
                CREATE PROC dbo.spCompany_Create
                  @Identification NVARCHAR(11),
                  @Name NVARCHAR(200),
                  @ComercialName NVARCHAR(200),
                  @Category NVARCHAR(100) = NULL,
                  @PaymentScheme NVARCHAR(100),
                  @Status NVARCHAR(50),
                  @EconomicActivity NVARCHAR(200),
                  @GovernmentBranch NVARCHAR(200)
                AS
                BEGIN
                  SET NOCOUNT ON;

                  IF EXISTS(SELECT 1 FROM dbo.Companies WITH (NOLOCK) WHERE Identification=@Identification AND IsDeleted=0)
                  BEGIN
                    RAISERROR('Identification (RNC) ya existe.',16,1);
                    RETURN;
                  END

                  INSERT INTO dbo.Companies
                    (Identification, [Name], ComercialName, Category, PaymentScheme, [Status], EconomicActivity, GovernmentBranch)
                  VALUES
                    (@Identification, @Name, @ComercialName, @Category, @PaymentScheme, @Status, @EconomicActivity, @GovernmentBranch);

                  SELECT CAST(SCOPE_IDENTITY() AS INT) AS CompanyId;
                END
                ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.spCompany_List','P') IS NOT NULL
                    DROP PROC dbo.spCompany_List;
                ");

            migrationBuilder.Sql(@"
                CREATE PROC dbo.spCompany_List
                AS
                BEGIN
                  SET NOCOUNT ON;

                  SELECT
                      Id,
                      Identification,
                      [Name],
                      ComercialName,
                      Category,
                      PaymentScheme,
                      [Status],
                      EconomicActivity,
                      GovernmentBranch,
                      CreatedAt,
                      UpdatedAt
                  FROM dbo.Companies WITH (NOLOCK)
                  WHERE IsDeleted = 0
                  ORDER BY Id DESC;
                END
                ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.spCompany_GetByIdentification','P') IS NOT NULL
                    DROP PROC dbo.spCompany_GetByIdentification;
                ");

            migrationBuilder.Sql(@"
                CREATE PROC dbo.spCompany_GetByIdentification
                  @Identification NVARCHAR(11)
                AS
                BEGIN
                  SET NOCOUNT ON;

                  SELECT TOP(1)
                      Id,
                      Identification,
                      [Name],
                      ComercialName,
                      Category,
                      PaymentScheme,
                      [Status],
                      EconomicActivity,
                      GovernmentBranch,
                      CreatedAt,
                      UpdatedAt
                  FROM dbo.Companies WITH (NOLOCK)
                  WHERE Identification=@Identification AND IsDeleted=0;
                END
                ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.spCompany_GetById','P') IS NOT NULL
                    DROP PROC dbo.spCompany_GetById;
                ");

            migrationBuilder.Sql(@"
                CREATE PROC dbo.spCompany_GetById
                  @Id INT
                AS
                BEGIN
                  SET NOCOUNT ON;

                  SELECT TOP(1)
                      Id,
                      Identification,
                      [Name],
                      ComercialName,
                      Category,
                      PaymentScheme,
                      [Status],
                      EconomicActivity,
                      GovernmentBranch,
                      CreatedAt,
                      UpdatedAt
                  FROM dbo.Companies WITH (NOLOCK)
                  WHERE Id=@Id AND IsDeleted=0;
                END
                ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.spCompany_Update','P') IS NOT NULL
                    DROP PROC dbo.spCompany_Update;
                ");

            migrationBuilder.Sql(@"
                CREATE PROC dbo.spCompany_Update
                  @Id INT,
                  @Name NVARCHAR(200),
                  @ComercialName NVARCHAR(200),
                  @Category NVARCHAR(100) = NULL,
                  @PaymentScheme NVARCHAR(100),
                  @Status NVARCHAR(50),
                  @EconomicActivity NVARCHAR(200),
                  @GovernmentBranch NVARCHAR(200)
                AS
                BEGIN
                  SET NOCOUNT ON;

                  IF NOT EXISTS(SELECT 1 FROM dbo.Companies WITH (NOLOCK) WHERE Id=@Id AND IsDeleted=0)
                  BEGIN
                    RAISERROR('Empresa no encontrada.',16,1);
                    RETURN;
                  END

                  UPDATE dbo.Companies
                     SET [Name] = @Name,
                         ComercialName = @ComercialName,
                         Category = @Category,
                         PaymentScheme = @PaymentScheme,
                         [Status] = @Status,
                         EconomicActivity = @EconomicActivity,
                         GovernmentBranch = @GovernmentBranch,
                         UpdatedAt = GETDATE()
                   WHERE Id=@Id AND IsDeleted=0;

                  SELECT @@ROWCOUNT AS Affected;
                END
                ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.spCompany_Delete','P') IS NOT NULL
                    DROP PROC dbo.spCompany_Delete;
                ");

            migrationBuilder.Sql(@"
                CREATE PROC dbo.spCompany_Delete
                  @Id INT
                AS
                BEGIN
                  SET NOCOUNT ON;

                  UPDATE dbo.Companies
                     SET IsDeleted = 1,
                         UpdatedAt = GETDATE()
                   WHERE Id=@Id AND IsDeleted=0;

                  SELECT @@ROWCOUNT AS Affected;
                END
                ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "CompanyView");

            migrationBuilder.Sql("IF OBJECT_ID('dbo.spCompany_Delete','P') IS NOT NULL DROP PROC dbo.spCompany_Delete;");

            migrationBuilder.Sql("IF OBJECT_ID('dbo.spCompany_Update','P') IS NOT NULL DROP PROC dbo.spCompany_Update;");

            migrationBuilder.Sql("IF OBJECT_ID('dbo.spCompany_GetById','P') IS NOT NULL DROP PROC dbo.spCompany_GetById;");

            migrationBuilder.Sql("IF OBJECT_ID('dbo.spCompany_GetByIdentification','P') IS NOT NULL DROP PROC dbo.spCompany_GetByIdentification;");

            migrationBuilder.Sql("IF OBJECT_ID('dbo.spCompany_List','P') IS NOT NULL DROP PROC dbo.spCompany_List;");

            migrationBuilder.Sql("IF OBJECT_ID('dbo.spCompany_Create','P') IS NOT NULL DROP PROC dbo.spCompany_Create;");
        }
    }
}
