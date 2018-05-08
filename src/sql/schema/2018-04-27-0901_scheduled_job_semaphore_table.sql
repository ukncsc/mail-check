CREATE TABLE IF NOT EXISTS `derived_job_semaphore_20180427` (
  `job_type` enum('top10_senders','weekly_stats','import_providers','derived_domain_tree','derived_aggregate_all_daily_senders','derived_aggregate_daily','derived_aggregate_daily_rollup','derived_aggregate_daily_senders_top10_rollup','import_domains') NOT NULL,
  `start_time` TIMESTAMP NOT NULL,
  `end_time` TIMESTAMP,
  PRIMARY KEY (job_type, start_time)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;