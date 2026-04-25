using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    public partial class PricingRuleDayOfWeekJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'DayOfWeek'
                      AND Object_ID = Object_ID(N'[dbo].[PricingRules]')
                      AND system_type_id = 56
                )
                BEGIN
                    ALTER TABLE [dbo].[PricingRules] ADD [DayOfWeekTmp] nvarchar(max) NULL;

                    UPDATE [dbo].[PricingRules]
                    SET [DayOfWeekTmp] =
                        CASE [DayOfWeek]
                            WHEN 1 THEN N'["MON"]'
                            WHEN 2 THEN N'["TUE"]'
                            WHEN 3 THEN N'["WED"]'
                            WHEN 4 THEN N'["THU"]'
                            WHEN 5 THEN N'["FRI"]'
                            WHEN 6 THEN N'["SAT"]'
                            WHEN 0 THEN N'["SUN"]'
                            ELSE N'[]'
                        END;

                    ALTER TABLE [dbo].[PricingRules] DROP COLUMN [DayOfWeek];
                    EXEC sp_rename N'[dbo].[PricingRules].[DayOfWeekTmp]', N'DayOfWeek', N'COLUMN';
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'DayOfWeek'
                      AND Object_ID = Object_ID(N'[dbo].[PricingRules]')
                      AND system_type_id IN (231, 239, 99, 35)
                )
                BEGIN
                    ALTER TABLE [dbo].[PricingRules] ADD [DayOfWeekTmp] int NULL;

                    UPDATE [dbo].[PricingRules]
                    SET [DayOfWeekTmp] =
                        CASE JSON_VALUE([DayOfWeek], '$[0]')
                            WHEN 'MON' THEN 1
                            WHEN 'TUE' THEN 2
                            WHEN 'WED' THEN 3
                            WHEN 'THU' THEN 4
                            WHEN 'FRI' THEN 5
                            WHEN 'SAT' THEN 6
                            WHEN 'SUN' THEN 0
                            ELSE NULL
                        END;

                    ALTER TABLE [dbo].[PricingRules] DROP COLUMN [DayOfWeek];
                    EXEC sp_rename N'[dbo].[PricingRules].[DayOfWeekTmp]', N'DayOfWeek', N'COLUMN';
                END
                """);
        }
    }
}
