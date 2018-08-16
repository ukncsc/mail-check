ALTER TABLE aggregate_report ADD UNIQUE `dedup_reports`(`report_id`,`domain`,`org_name`);
ALTER TABLE aggregate_report DROP INDEX `report_id_UNIQUE`;