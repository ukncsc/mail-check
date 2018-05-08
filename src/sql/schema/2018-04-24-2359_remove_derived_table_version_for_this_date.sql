DROP TABLE IF EXISTS `derived_job_semaphore_20180424`;
DROP PROCEDURE IF EXISTS  import_new_domains_20180424;
DROP PROCEDURE IF EXISTS  import_new_providers_20180424;
DROP TABLE IF EXISTS  `derived_aggregate_all_daily_senders_20180424`;
DROP TABLE IF EXISTS  `derived_aggregate_daily_20180424`;
DROP TABLE IF EXISTS  `derived_aggregate_top10_daily_threshold_20180424`;
DROP TABLE IF EXISTS  `derived_aggregate_daily_senders_top10_20180424`;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_senders_top10_20180424;
DROP TABLE IF EXISTS  `derived_user_domain_rollup_permissions_20180424`;
DROP PROCEDURE IF EXISTS  generate_derived_domain_tree_20180424;
DROP TABLE IF EXISTS  domain_elements_20180424;
DROP TABLE IF EXISTS  derived_domain_tree_20180424;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_rollup_20180424;
DROP TABLE IF EXISTS  `derived_aggregate_daily_rollup_20180424`;
DROP TABLE IF EXISTS  `derived_aggregate_daily_senders_top10_rollup_20180424`;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_senders_top10_rollup_20180424;
DROP PROCEDURE IF EXISTS  generate_hourly_derived_tables_20180424;
DROP EVENT IF EXISTS  generate_hourly_derived_tables_20180424;


