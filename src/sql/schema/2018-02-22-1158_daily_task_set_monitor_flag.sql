

DROP EVENT IF EXISTS set_monitor_flag;


DROP PROCEDURE IF EXISTS set_monitor_flag;

delimiter |

CREATE PROCEDURE set_monitor_flag()
BEGIN

update domain set monitor=b'1' where monitor=b'0' and (exists 
(select 1 from (select domain_id, datediff(current_timestamp, min(effective_date)) started, count(distinct effective_date) days, sum(total_email_count) emails from derived_aggregate_daily_rollup group by domain_id) s where s.domain_id = domain.id and days > 30 and round(100*days/started) > 20 and emails  >500)
or exists
(select 1 from dns_record_mx mx where domain.id = mx.domain_id and mx.hostname is not null and mx.end_date is null));

END |


CREATE EVENT set_monitor_flag
  ON SCHEDULE
    EVERY 1 DAY
    STARTS '2018-02-09 03:00:00' ON COMPLETION PRESERVE ENABLE
DO BEGIN
CALL set_monitor_flag();
END |


delimiter ;
CALL set_monitor_flag();