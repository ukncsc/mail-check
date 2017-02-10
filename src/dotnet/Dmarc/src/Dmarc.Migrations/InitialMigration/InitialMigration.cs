using SimpleMigrations;

namespace Dmarc.Migrations.InitialMigration
{
    [Migration(201612201316)]
    public class InitialMigration : Migration
    {
        public override void Up()
        {
            Execute(InititalMigrationResources.CreateAggregateReportTable);
            Execute(InititalMigrationResources.CreateRecordTable);
            Execute(InititalMigrationResources.CreatePolicyOverrideReasonTable);
            Execute(InititalMigrationResources.CreateDkimAuthResultTable);
            Execute(InititalMigrationResources.CreateSpfAuthResultTable);
        }

        public override void Down()
        {
            Execute("DROP TABLE aggregate_report;");
            Execute("DROP TABLE record;");
            Execute("DROP TABLE policy_override_reason;");
            Execute("DROP TABLE dkim_auth_result;");
            Execute("DROP TABLE spf_auth_result;");
        }
    }
}
