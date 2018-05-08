ALTER TABLE aggregate_report ADD COLUMN `effective_date` date AFTER `extra_contact_info`;
UPDATE aggregate_report SET `effective_date` = DATE(TIMESTAMPADD(SECOND,TIMESTAMPDIFF(SECOND,begin_date, end_date)/2,begin_date)) WHERE `effective_date` is NULL;
ALTER TABLE aggregate_report MODIFY COLUMN `effective_date` DATE NOT NULL;
CREATE INDEX `effective_date_idx` ON `aggregate_report` (`effective_date`);
