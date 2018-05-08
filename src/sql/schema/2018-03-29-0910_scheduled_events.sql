
DROP EVENT IF EXISTS daily_generate_derived_tables;
DROP EVENT IF EXISTS generate_daily_derived_tables;
DROP EVENT IF EXISTS generate_hourly_derived_tables;



DROP PROCEDURE IF EXISTS generate_hourly_derived_tables;

delimiter |

CREATE PROCEDURE generate_hourly_derived_tables()
BEGIN
CALL import_new_providers();
CALL update_derived_aggregate_all_daily_senders();
CALL update_derived_aggregate_daily();
CALL update_derived_aggregate_daily_senders_top10();
CALL generate_derived_domain_tree();
CALL update_derived_aggregate_daily_rollup();
CALL update_derived_aggregate_daily_senders_top10_rollup();
END |

CREATE EVENT generate_hourly_derived_tables
  ON SCHEDULE
    EVERY 1 HOUR
DO BEGIN
CALL generate_hourly_derived_tables();
END |

delimiter ;