
CREATE TABLE IF NOT EXISTS `derived_aggregate_daily_rollup` (
  `domain_id` INT UNSIGNED NOT NULL,
  `effective_date` DATE NOT NULL,
   `full_compliance_count`INT UNSIGNED NOT NULL,
  `dkim_only_count` INT UNSIGNED NOT NULL,
  `spf_only_count` INT UNSIGNED NOT NULL,
  `untrusted_email_count` INT UNSIGNED NOT NULL,
  `trusted_email_count` INT UNSIGNED NOT NULL,
  `disposition_quarantine_count` INT UNSIGNED NOT NULL,
  `disposition_reject_count` INT UNSIGNED NOT NULL,
  `disposition_none_count` INT UNSIGNED NOT NULL,
  `untrusted_pass_count` INT UNSIGNED NOT NULL,
  `untrusted_block_count` INT UNSIGNED NOT NULL,
  `untrusted_quarantine_count` INT UNSIGNED NOT NULL,
  `untrusted_reject_count` INT UNSIGNED NOT NULL,
  `trusted_pass_count`INT UNSIGNED NOT NULL,
  `trusted_block_count` INT UNSIGNED NOT NULL,
  `trusted_quarantine_count` INT UNSIGNED NOT NULL,
  `trusted_reject_count` INT UNSIGNED NOT NULL,
  `untrusted_override_count`INT UNSIGNED NOT NULL,
  `trusted_override_count` INT UNSIGNED NOT NULL,
  `override_arc`INT UNSIGNED NOT NULL,
  `override_forwarded` INT UNSIGNED NOT NULL,
  `override_trusted_forwarder` INT UNSIGNED NOT NULL,
  `override_sampled_out` INT UNSIGNED NOT NULL,
  `total_email_count` INT UNSIGNED NOT NULL,
  `aggregate_report_count` INT UNSIGNED NOT NULL,
  `aggregate_report_record_count` INT UNSIGNED NOT NULL,
  PRIMARY KEY (domain_id, effective_date),
  INDEX (effective_date)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


delimiter |

CREATE PROCEDURE update_derived_aggregate_daily_rollup()
BEGIN
DECLARE job_running int;
DECLARE started_at timestamp;

SELECT count(*) INTO @job_running FROM `derived_job_semaphore` where end_time IS NULL AND TIMESTAMPADD(HOUR,3,start_time) > CURRENT_TIMESTAMP;

IF @job_running = 0 THEN 

SELECT CURRENT_TIMESTAMP INTO @started_at;

INSERT INTO `derived_job_semaphore` (job_type, start_time) VALUES ('derived_aggregate_daily_rollup',@started_at);




REPLACE INTO `derived_aggregate_daily_rollup` 
SELECT ddt.parent_id domain_id,effective_date,
SUM(`full_compliance_count`) `full_compliance_count`, 
SUM(`dkim_only_count`) `dkim_only_count`, 
SUM(`spf_only_count`) `spf_only_count`,
SUM(`untrusted_email_count`) `untrusted_email_count`,
SUM(`trusted_email_count`) `trusted_email_count`,
SUM(`disposition_quarantine_count`) `disposition_quarantine_count`,
SUM(`disposition_reject_count`) `disposition_reject_count`,
SUM(`disposition_none_count`) `disposition_none_count`,
SUM(`untrusted_pass_count`) `untrusted_pass_count`,
SUM(`untrusted_block_count`) `untrusted_block_count`,
SUM(`untrusted_quarantine_count`) `untrusted_quarantine_count`,
SUM(`untrusted_reject_count`) `untrusted_reject_count`,
SUM(`trusted_pass_count`) `trusted_pass_count`,
SUM(`trusted_block_count`) `trusted_block_count`,
SUM(`trusted_quarantine_count`) `trusted_quarantine_count`,
SUM(`trusted_reject_count`) `trusted_reject_count`,
SUM(`untrusted_override_count`) `untrusted_override_count`,
SUM(`trusted_override_count`) `trusted_override_count`,
SUM(`override_arc`) `override_arc`,
SUM(`override_forwarded`) `override_forwarded`,
SUM(`override_trusted_forwarder`) `override_trusted_forwarder`,
SUM(`override_sampled_out`) `override_sampled_out`,
SUM(`total_email_count`) `total_email_count`,
SUM(`aggregate_report_count`) `aggregate_report_count`,
SUM(`aggregate_report_record_count`) `aggregate_report_record_count`
FROM `derived_aggregate_daily` dad
JOIN derived_domain_tree ddt ON ddt.child_id = dad.domain_id
where effective_date >= COALESCE((SELECT DATE_SUB(max(effective_date), INTERVAL 2 DAY) FROM derived_aggregate_daily_rollup),
(SELECT MIN(effective_date) FROM aggregate_report)) 
group by ddt.parent_id,effective_date;

UPDATE `derived_job_semaphore` SET end_time=CURRENT_TIMESTAMP where `job_type` = 'derived_aggregate_daily_rollup' and `start_time` = @started_at;
END IF;

END |