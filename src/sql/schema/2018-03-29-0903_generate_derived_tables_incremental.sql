DROP TABLE IF EXISTS `derived_aggregate_all_daily_senders`;

CREATE TABLE IF NOT EXISTS `derived_aggregate_all_daily_senders` (
  `domain_id` INT UNSIGNED NOT NULL,
  `effective_date` DATE NOT NULL,
  `source_ip` varchar(39) NOT NULL,
  `provider_id` BIGINT UNSIGNED NOT NULL,
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
  PRIMARY KEY (domain_id, effective_date, source_ip, provider_id),
  INDEX (effective_date),
  INDEX (source_ip),
  CONSTRAINT `derived_aggregate_all_daily_senders$provider_id`
  FOREIGN KEY (`provider_id`)
    REFERENCES `dmarc`.`provider` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;



DROP TABLE IF EXISTS `derived_aggregate_daily`;

CREATE TABLE IF NOT EXISTS `derived_aggregate_daily` (
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





DROP PROCEDURE IF EXISTS update_derived_aggregate_daily_senders;
DROP PROCEDURE IF EXISTS update_derived_aggregate_all_daily_senders;

DELIMITER |

CREATE PROCEDURE update_derived_aggregate_all_daily_senders()
BEGIN
DECLARE job_running int;
DECLARE started_at timestamp;

SELECT count(*) INTO @job_running FROM `derived_job_semaphore` where end_time IS NULL AND TIMESTAMPADD(HOUR,3,start_time) > CURRENT_TIMESTAMP;

IF @job_running = 0 THEN 

SELECT CURRENT_TIMESTAMP INTO @started_at;

INSERT INTO `derived_job_semaphore` (job_type, start_time) VALUES ('derived_aggregate_all_daily_senders',@started_at);

REPLACE INTO `derived_aggregate_all_daily_senders` SELECT r.header_from_domain_id domain_id,effective_date,r.source_ip,
provider_id,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') = 'pass' AND r.spf = 'pass') THEN r.count ELSE 0 END),0) `full_compliance_count`, 
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') = 'pass' AND r.spf != 'pass') THEN r.count ELSE 0 END),0) `dkim_only_count`, 
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf = 'pass') THEN r.count ELSE 0 END),0) `spf_only_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') THEN r.count ELSE 0 END),0) `untrusted_email_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') = 'pass' OR r.spf = 'pass') THEN r.count ELSE 0 END),0) `trusted_email_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.disposition,'none') = 'quarantine') THEN r.count ELSE 0 END),0) `disposition_quarantine_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.disposition,'none') = 'reject') THEN r.count ELSE 0 END),0) `disposition_reject_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.disposition,'none') != 'quarantine' and r.disposition != 'reject') THEN r.count ELSE 0 END),0) `disposition_none_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'none') THEN r.count ELSE 0 END),0) `untrusted_pass_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'reject' or coalesce(r.disposition,'none') = 'quarantine') THEN r.count ELSE 0 END),0) `untrusted_block_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'reject') THEN r.count ELSE 0 END),0) `untrusted_reject_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'quarantine') THEN r.count ELSE 0 END),0) `untrusted_quarantine_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') = 'pass' OR r.spf = 'pass') AND (coalesce(r.disposition,'none') = 'none') THEN r.count ELSE 0 END),0) `trusted_pass_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') = 'pass' OR r.spf = 'pass') AND (coalesce(r.disposition,'none') = 'reject' or coalesce(r.disposition,'none') = 'quarantine') THEN r.count ELSE 0 END),0) `trusted_block_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') = 'pass' OR r.spf = 'pass') AND (coalesce(r.disposition,'none') = 'reject') THEN r.count ELSE 0 END),0) `trusted_reject_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') = 'pass' OR r.spf = 'pass') AND (coalesce(r.disposition,'none') = 'quarantine') THEN r.count ELSE 0 END),0) `trusted_quarantine_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'none') AND EXISTS (select 1 from policy_override_reason por where r.id = por.record_id) THEN r.count ELSE 0 END),0) `untrusted_override_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') = 'pass' OR r.spf = 'pass') AND (coalesce(r.disposition,'none') = 'reject' or coalesce(r.disposition,'none') = 'quarantine') AND (select 1 from policy_override_reason por where r.id = por.record_id) THEN r.count ELSE 0 END),0) `trusted_override_count`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'none') AND EXISTS (select 1 from policy_override_reason por where r.id = por.record_id AND lower(por.comment) LIKE "%arc%") THEN r.count ELSE 0 END),0) `override_arc`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'none') AND EXISTS (select 1 from policy_override_reason por where r.id = por.record_id AND por.policy_override = "forwarded") THEN r.count ELSE 0 END),0) `override_forwarded`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'none') AND EXISTS (select 1 from policy_override_reason por where r.id = por.record_id AND por.policy_override = "trusted forwarder") THEN r.count ELSE 0 END),0) `override_trusted_forwarder`,
COALESCE(SUM(CASE WHEN (coalesce(r.dkim,'fail') != 'pass' AND r.spf != 'pass') AND (coalesce(r.disposition,'none') = 'none') AND EXISTS (select 1 from policy_override_reason por where r.id = por.record_id AND por.policy_override = "sampled_out") THEN r.count ELSE 0 END),0) `override_sampled_out`,
SUM(r.count) `total_email_count`,
count(distinct ar.id) `aggregate_report_count`,
count(distinct r.id) `aggregate_report_record_count`
FROM aggregate_report ar 
JOIN record r ON ar.id = r.aggregate_report_id  
WHERE ar.provider_id IS NOT NULL
AND effective_date BETWEEN
COALESCE((SELECT DATE_SUB(max(effective_date), INTERVAL 2 DAY) FROM derived_aggregate_all_daily_senders),
(SELECT MIN(effective_date) FROM aggregate_report))
AND
COALESCE((SELECT effective_date FROM aggregate_report  WHERE effective_date >= COALESCE((SELECT DATE_SUB(max(effective_date), INTERVAL 2 DAY) FROM derived_aggregate_all_daily_senders),
(SELECT MIN(effective_date) FROM aggregate_report)) ORDER BY effective_date ASC LIMIT 120000,1), CURRENT_TIMESTAMP)
GROUP BY effective_date,r.header_from_domain_id,r.source_ip, ar.provider_id;

UPDATE `derived_job_semaphore` SET end_time=CURRENT_TIMESTAMP WHERE `job_type` = 'derived_aggregate_all_daily_senders' AND `start_time` = @started_at;

END IF;

END |


DROP PROCEDURE IF EXISTS replace_derived_aggregate_daily;
DROP PROCEDURE IF EXISTS update_derived_aggregate_daily;

CREATE PROCEDURE update_derived_aggregate_daily()
BEGIN
DECLARE job_running int;
DECLARE started_at timestamp;

SELECT count(*) INTO @job_running FROM `derived_job_semaphore` where end_time IS NULL AND TIMESTAMPADD(HOUR,3,start_time) > CURRENT_TIMESTAMP;

IF @job_running = 0 THEN 

SELECT CURRENT_TIMESTAMP INTO @started_at;

INSERT INTO `derived_job_semaphore` (job_type, start_time) VALUES ('derived_aggregate_daily',@started_at);


REPLACE INTO `derived_aggregate_daily` 
SELECT domain_id,effective_date,
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
FROM `derived_aggregate_all_daily_senders` daads
WHERE effective_date >=
COALESCE((SELECT DATE_SUB(max(effective_date), INTERVAL 2 DAY) FROM derived_aggregate_daily),
(SELECT MIN(effective_date) FROM aggregate_report))
group by domain_id,effective_date;


UPDATE `derived_job_semaphore` SET end_time=CURRENT_TIMESTAMP where `job_type` = 'derived_aggregate_daily' and `start_time` = @started_at;
END IF;

END |

delimiter ;

