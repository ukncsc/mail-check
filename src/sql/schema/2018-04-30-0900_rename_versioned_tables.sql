
DROP EVENT IF EXISTS  generate_hourly_derived_tables_20180427;
DROP PROCEDURE IF EXISTS  import_new_domains_20180427;
DROP PROCEDURE IF EXISTS  import_new_providers_20180427;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_all_daily_senders_20180427;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_20180427;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_senders_top10_20180427;
DROP PROCEDURE IF EXISTS  generate_derived_domain_tree_20180427;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_rollup_20180427;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_senders_top10_rollup_20180427;
DROP PROCEDURE IF EXISTS  generate_hourly_derived_tables_20180427;

DROP EVENT IF EXISTS  generate_hourly_derived_tables;
DROP EVENT IF EXISTS import_new_domains_daily;
DROP PROCEDURE IF EXISTS  import_new_domains;
DROP PROCEDURE IF EXISTS  import_new_providers;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_all_daily_senders;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_senders_top10;
DROP PROCEDURE IF EXISTS  generate_derived_domain_tree;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_rollup;
DROP PROCEDURE IF EXISTS  update_derived_aggregate_daily_senders_top10_rollup;
DROP PROCEDURE IF EXISTS  generate_hourly_derived_tables;

DROP TABLE IF EXISTS `domain_elements`;
DROP TABLE IF EXISTS `derived_domain_tree`;

RENAME TABLE    `derived_job_semaphore` TO `derived_job_semaphore_old`,
                `derived_job_semaphore_20180427` TO `derived_job_semaphore`,
                `derived_aggregate_all_daily_senders` TO `derived_aggregate_all_daily_senders_old`,
                `derived_aggregate_all_daily_senders_20180427` TO `derived_aggregate_all_daily_senders`,
                `derived_aggregate_daily` TO `derived_aggregate_daily_old`,
                `derived_aggregate_daily_20180427` TO `derived_aggregate_daily`,
                `derived_aggregate_top10_daily_threshold` TO `derived_aggregate_top10_daily_threshold_old`,
                `derived_aggregate_top10_daily_threshold_20180427` TO `derived_aggregate_top10_daily_threshold`,
                `derived_aggregate_daily_senders_top10` TO `derived_aggregate_daily_senders_top10_old`,
                `derived_aggregate_daily_senders_top10_20180427` TO `derived_aggregate_daily_senders_top10`,
                `derived_user_domain_rollup_permissions` TO `derived_user_domain_rollup_permissions_old`,
                `derived_user_domain_rollup_permissions_20180427` TO `derived_user_domain_rollup_permissions`,
                `derived_aggregate_daily_rollup` TO `derived_aggregate_daily_rollup_old`,
                `derived_aggregate_daily_rollup_20180427` TO `derived_aggregate_daily_rollup`,
                `derived_aggregate_daily_senders_top10_rollup` TO `derived_aggregate_daily_senders_top10_rollup_old`,
                `derived_aggregate_daily_senders_top10_rollup_20180427` TO `derived_aggregate_daily_senders_top10_rollup`;

