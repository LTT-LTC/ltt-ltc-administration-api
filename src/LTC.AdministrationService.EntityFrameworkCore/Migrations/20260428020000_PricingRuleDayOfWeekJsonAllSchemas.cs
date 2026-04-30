using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LTC.AdministrationService.Migrations
{
    public partial class PricingRuleDayOfWeekJsonAllSchemas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @schemaName sysname;
                DECLARE schema_cursor CURSOR FOR
                SELECT s.name
                FROM sys.schemas s
                WHERE EXISTS (
                    SELECT 1
                    FROM sys.tables t
                    WHERE t.schema_id = s.schema_id
                      AND t.name = N'PricingRules'
                );

                OPEN schema_cursor;
                FETCH NEXT FROM schema_cursor INTO @schemaName;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    DECLARE @sql nvarchar(max) = N'
                    IF EXISTS (
                        SELECT 1
                        FROM sys.columns c
                        INNER JOIN sys.tables t ON c.object_id = t.object_id
                        INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                        WHERE s.name = N''' + @schemaName + N'''
                          AND t.name = N''PricingRules''
                          AND c.name = N''DayOfWeek''
                          AND c.system_type_id = 56
                    )
                    BEGIN
                        ALTER TABLE [' + @schemaName + N'].[PricingRules] ADD [DayOfWeekTmp] nvarchar(max) NULL;

                        UPDATE [' + @schemaName + N'].[PricingRules]
                        SET [DayOfWeekTmp] =
                            CASE [DayOfWeek]
                                WHEN 1 THEN N''[""MON""]''
                                WHEN 2 THEN N''[""TUE""]''
                                WHEN 3 THEN N''[""WED""]''
                                WHEN 4 THEN N''[""THU""]''
                                WHEN 5 THEN N''[""FRI""]''
                                WHEN 6 THEN N''[""SAT""]''
                                WHEN 0 THEN N''[""SUN""]''
                                ELSE N''[]''
                            END;

                        ALTER TABLE [' + @schemaName + N'].[PricingRules] DROP COLUMN [DayOfWeek];
                        EXEC sp_rename N''[' + @schemaName + N'].[PricingRules].[DayOfWeekTmp]'', N''DayOfWeek'', N''COLUMN'';
                    END';

                    EXEC sp_executesql @sql;
                    FETCH NEXT FROM schema_cursor INTO @schemaName;
                END

                CLOSE schema_cursor;
                DEALLOCATE schema_cursor;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @schemaName sysname;
                DECLARE schema_cursor CURSOR FOR
                SELECT s.name
                FROM sys.schemas s
                WHERE EXISTS (
                    SELECT 1
                    FROM sys.tables t
                    WHERE t.schema_id = s.schema_id
                      AND t.name = N'PricingRules'
                );

                OPEN schema_cursor;
                FETCH NEXT FROM schema_cursor INTO @schemaName;

                WHILE @@FETCH_STATUS = 0
                BEGIN
                    DECLARE @sql nvarchar(max) = N'
                    IF EXISTS (
                        SELECT 1
                        FROM sys.columns c
                        INNER JOIN sys.tables t ON c.object_id = t.object_id
                        INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                        WHERE s.name = N''' + @schemaName + N'''
                          AND t.name = N''PricingRules''
                          AND c.name = N''DayOfWeek''
                          AND c.system_type_id IN (231, 239, 99, 35)
                    )
                    BEGIN
                        ALTER TABLE [' + @schemaName + N'].[PricingRules] ADD [DayOfWeekTmp] int NULL;

                        UPDATE [' + @schemaName + N'].[PricingRules]
                        SET [DayOfWeekTmp] =
                            CASE JSON_VALUE([DayOfWeek], ''$[0]'')
                                WHEN ''MON'' THEN 1
                                WHEN ''TUE'' THEN 2
                                WHEN ''WED'' THEN 3
                                WHEN ''THU'' THEN 4
                                WHEN ''FRI'' THEN 5
                                WHEN ''SAT'' THEN 6
                                WHEN ''SUN'' THEN 0
                                ELSE NULL
                            END;

                        ALTER TABLE [' + @schemaName + N'].[PricingRules] DROP COLUMN [DayOfWeek];
                        EXEC sp_rename N''[' + @schemaName + N'].[PricingRules].[DayOfWeekTmp]'', N''DayOfWeek'', N''COLUMN'';
                    END';

                    EXEC sp_executesql @sql;
                    FETCH NEXT FROM schema_cursor INTO @schemaName;
                END

                CLOSE schema_cursor;
                DEALLOCATE schema_cursor;
                """);
        }
    }
}
