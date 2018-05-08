
delimiter |

CREATE PROCEDURE generate_hourly_derived_tables_20180427()
BEGIN
CALL import_new_providers_20180427();
CALL import_new_domains_20180427();
CALL update_derived_aggregate_all_daily_senders_20180427();
CALL update_derived_aggregate_daily_20180427();
CALL update_derived_aggregate_daily_senders_top10_20180427();
CALL generate_derived_domain_tree_20180427();
CALL update_derived_aggregate_daily_rollup_20180427();
CALL update_derived_aggregate_daily_senders_top10_rollup_20180427();
END |

CREATE EVENT generate_hourly_derived_tables_20180427
  ON SCHEDULE
    EVERY 10 MINUTE
DO BEGIN
CALL generate_hourly_derived_tables_20180427();
END |

delimiter ;