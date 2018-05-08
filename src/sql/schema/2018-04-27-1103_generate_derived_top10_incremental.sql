

CREATE TABLE IF NOT EXISTS `derived_aggregate_top10_daily_threshold_20180427` (
  `domain_id` INT UNSIGNED NOT NULL,
  `effective_date` DATE NOT NULL,
  `full_compliance` INT UNSIGNED NOT NULL,
  `dkim_only` INT UNSIGNED NOT NULL,
  `spf_only` INT UNSIGNED NOT NULL,
  `untrusted_email` INT UNSIGNED NOT NULL,
  PRIMARY KEY (domain_id, effective_date)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE IF NOT EXISTS `derived_aggregate_daily_senders_top10_20180427` (
  `domain_id` INT UNSIGNED NOT NULL,
  `effective_date` DATE NOT NULL,
  `source_ip` varchar(39) NOT NULL,
  `full_compliance_top10` BIT NOT NULL,
  `dkim_only_top10` BIT NOT NULL,
  `spf_only_top10` BIT NOT NULL,
  `untrusted_email_top10` BIT NOT NULL,
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
  PRIMARY KEY (domain_id, effective_date, source_ip)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

delimiter |

CREATE PROCEDURE update_derived_aggregate_daily_senders_top10_20180427()
BEGIN
DECLARE job_running int;
DECLARE started_at timestamp;

SELECT count(*) INTO @job_running FROM `derived_job_semaphore_20180427` where end_time IS NULL AND TIMESTAMPADD(HOUR,3,start_time) > CURRENT_TIMESTAMP;

IF @job_running = 0 THEN 

SELECT CURRENT_TIMESTAMP INTO @started_at;

INSERT INTO `derived_job_semaphore_20180427` (job_type, start_time) VALUES ('top10_senders',@started_at);


REPLACE INTO derived_aggregate_top10_daily_threshold_20180427 
SELECT
d1.domain_id domain_id,
d1.effective_date effective_date,
CAST(COALESCE((SELECT GREATEST(SUBSTRING_INDEX(SUBSTRING_INDEX(GROUP_CONCAT(d2.full_compliance_count ORDER BY d2.full_compliance_count DESC SEPARATOR ','), ',',10),',',-1),1)
   FROM derived_aggregate_all_daily_senders_20180427 d2 
   WHERE d1.domain_id=d2.domain_id and d1.effective_date = d2.effective_date and d2.full_compliance_count >0
   ),1) AS UNSIGNED INTEGER)  `full_compliance`,
CAST(COALESCE((SELECT GREATEST(SUBSTRING_INDEX(SUBSTRING_INDEX(GROUP_CONCAT(d2.dkim_only_count ORDER BY d2.dkim_only_count DESC SEPARATOR ','), ',',10),',',-1),1)
   FROM derived_aggregate_all_daily_senders_20180427 d2
   WHERE d1.domain_id=d2.domain_id and d1.effective_date = d2.effective_date and d2.dkim_only_count >0
   ),1) AS UNSIGNED INTEGER) `dkim_only`,
CAST(COALESCE((SELECT GREATEST(SUBSTRING_INDEX(SUBSTRING_INDEX(GROUP_CONCAT(d2.spf_only_count ORDER BY d2.spf_only_count DESC SEPARATOR ','), ',',10),',',-1),1)
   FROM derived_aggregate_all_daily_senders_20180427 d2
   WHERE d1.domain_id=d2.domain_id and d1.effective_date = d2.effective_date and d2.spf_only_count >0
   ),1) AS UNSIGNED INTEGER) `spf_only`,
CAST(COALESCE((SELECT GREATEST(SUBSTRING_INDEX(SUBSTRING_INDEX(GROUP_CONCAT(d2.untrusted_email_count ORDER BY d2.untrusted_email_count DESC SEPARATOR ','), ',',10),',',-1),1)
   FROM derived_aggregate_all_daily_senders_20180427 d2 
   WHERE d1.domain_id=d2.domain_id and d1.effective_date = d2.effective_date and d2.untrusted_email_count >0
   ),1) AS UNSIGNED INTEGER) `untrusted_email`
FROM `derived_aggregate_all_daily_senders_20180427`  d1
WHERE effective_date >= COALESCE((SELECT DATE_SUB(max(effective_date), INTERVAL 2 DAY) FROM derived_aggregate_top10_daily_threshold_20180427),
(SELECT MIN(effective_date) FROM aggregate_report))
GROUP BY effective_date, domain_id;


REPLACE INTO `derived_aggregate_daily_senders_top10_20180427` 
SELECT
d1.domain_id domain_id,
d1.effective_date effective_date,
d1.source_ip source_ip,
CASE WHEN d1.full_compliance_count >= full_compliance THEN b'1' ELSE b'0' END full_compliance_top10,
CASE WHEN d1.dkim_only_count >= dkim_only THEN b'1' ELSE b'0' END dkim_only_top10,
CASE WHEN d1.spf_only_count >= spf_only THEN b'1' ELSE b'0' END spf_only_top10,
CASE WHEN d1.untrusted_email_count >= untrusted_email THEN b'1' ELSE b'0' END untrusted_email_top10,
`full_compliance_count` `full_compliance_count`, 
`dkim_only_count` `dkim_only_count`, 
`spf_only_count` `spf_only_count`,
`untrusted_email_count` `untrusted_email_count`,
`trusted_email_count` `trusted_email_count`,
`disposition_quarantine_count` `disposition_quarantine_count`,
`disposition_reject_count` `disposition_reject_count`,
`disposition_none_count` `disposition_none_count`,
`untrusted_pass_count` `untrusted_pass_count`,
`untrusted_block_count` `untrusted_block_count`,
`untrusted_quarantine_count` `untrusted_quarantine_count`,
`untrusted_reject_count` `untrusted_reject_count`,
`trusted_pass_count` `trusted_pass_count`,
`trusted_block_count` `trusted_block_count`,
`trusted_quarantine_count` `trusted_quarantine_count`,
`trusted_reject_count` `trusted_reject_count`,
`untrusted_override_count` `untrusted_override_count`,
`trusted_override_count` `trusted_override_count`,
`override_arc` `override_arc`,
`override_forwarded` `override_forwarded`,
`override_trusted_forwarder` `override_trusted_forwarder`,
`override_sampled_out` `override_sampled_out`,
`total_email_count` `total_email_count`,
`aggregate_report_count` `aggregate_report_count`,
`aggregate_report_record_count` `aggregate_report_record_count`
FROM `derived_aggregate_all_daily_senders_20180427`  d1
JOIN `derived_aggregate_top10_daily_threshold_20180427` using(effective_date,domain_id) 
where effective_date >= COALESCE((SELECT DATE_SUB(max(effective_date), INTERVAL 2 DAY) FROM derived_aggregate_daily_senders_top10_20180427),
(SELECT MIN(effective_date) FROM aggregate_report)) 
AND (d1.full_compliance_count >= full_compliance
or d1.dkim_only_count >= dkim_only
or d1.spf_only_count >= spf_only
or d1.untrusted_email_count >= untrusted_email);


UPDATE `derived_job_semaphore_20180427` SET end_time=CURRENT_TIMESTAMP where `job_type` = 'top10_senders' and `start_time` = @started_at;

END IF;

END |