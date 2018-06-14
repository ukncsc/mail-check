DROP TABLE IF EXISTS `derived_weekly_stats`;
CREATE TABLE IF NOT EXISTS `derived_weekly_stats` (
  `week_beginning` DATE NOT NULL,
  `p_monitor` INT UNSIGNED NOT NULL,
  `p_block` INT UNSIGNED NOT NULL,
  `rua_mc` INT UNSIGNED NOT NULL,
  `ruf_mc` INT UNSIGNED NOT NULL,
  `domains` INT UNSIGNED NOT NULL,
  `users` INT UNSIGNED NOT NULL,
  `domains_reporting` INT UNSIGNED NOT NULL,
  `aggregate_report_count` INT UNSIGNED NOT NULL,
  `emails_blocked` INT UNSIGNED NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


DROP PROCEDURE IF EXISTS generate_derived_stats;

delimiter |

CREATE PROCEDURE generate_derived_stats()
BEGIN
DECLARE job_running int;
DECLARE started_at timestamp;

SELECT count(*) INTO @job_running FROM `derived_job_semaphore` where end_time IS NULL AND TIMESTAMPADD(HOUR,3,start_time) > CURRENT_TIMESTAMP;

IF @job_running = 0 THEN 

SELECT CURRENT_TIMESTAMP INTO @started_at;

INSERT INTO `derived_job_semaphore` (job_type, start_time) VALUES ('weekly_stats',@started_at);

truncate table derived_weekly_stats;
insert into derived_weekly_stats  
(select min(dad.effective_date) week_beginning,
 (select count(distinct domain_id) from dns_record_dmarc where start_date<=min(dad.effective_date) and (end_date>=min(dad.effective_date) or end_date is null) and record like '%p=none%') p_monitor,
 (select count(distinct domain_id) from dns_record_dmarc where start_date<=min(dad.effective_date) and (end_date>=min(dad.effective_date) or end_date is null) and (record like '%p=reject%' or record like '%p=quarantine%')) p_block,
 (select count(distinct domain_id) from dns_record_dmarc where start_date<=min(dad.effective_date) and (end_date>=min(dad.effective_date) or end_date is null) and (record like '%dmarc-rua@dmarc.service.gov.uk%')) rua_mc,
 (select count(distinct domain_id) from dns_record_dmarc where start_date<=min(dad.effective_date) and (end_date>=min(dad.effective_date) or end_date is null) and (record like '%dmarc-ruf@dmarc.service.gov.uk%')) ruf_mc,
 (select count(distinct id) from domain where date(created_date) <=date(max(dad.effective_date)) and domain.publish = b'1' or domain.monitor = b'1') domains,
 (select count(distinct id) from user where date(created_date) <=date(max(dad.effective_date))) users,
 count(distinct dad.domain_id) domains_reporting, 
 sum(dad.aggregate_report_count) aggregate_report_count,
 sum(dad.disposition_reject_count)+sum(dad.disposition_quarantine_count) emails_blocked
 from derived_aggregate_daily dad 
 group by yearweek(dad.effective_date));

UPDATE `derived_job_semaphore` SET end_time=CURRENT_TIMESTAMP where `job_type` = 'weekly_stats' and `start_time` = @started_at;

END IF;

END |

delimiter ;

DROP EVENT IF EXISTS generate_stats_daily;

delimiter |

CREATE EVENT generate_stats_daily
  ON SCHEDULE
    EVERY 1 DAY
    STARTS '2018-02-09 05:00:00' ON COMPLETION PRESERVE ENABLE
DO BEGIN
CALL generate_derived_stats();
END |

delimiter ;