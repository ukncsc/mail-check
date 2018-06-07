

DROP EVENT IF EXISTS set_monitor_flag;


DROP PROCEDURE IF EXISTS set_monitor_flag;

delimiter |

CREATE PROCEDURE set_monitor_flag()
BEGIN

update domain set monitor=b'1' where monitor=b'0' 
and (
name like '%gov.uk' 
or name like '%gov.uk'
or name like '%mil.uk'
or name like '%slc.co.uk'
or name like '%gov.scot'
or name like '%parliament.uk'
or name like '%nhs.uk'
or name like '%nhs.net'
or name like '%police.uk'
or name like '%naturalengland.org.uk'
or name like '%ofcom.org.uk'
or name like '%ico.org.uk'
or name like '%hmcts.net'
or name like '%atos.net'
or name like '%scotent.co.uk'
or name like '%assembly.wales'
or name like '%cqc.org.uk'
or name like '%bl.uk'
or name like '%stfc.ac.uk'
or name like '%bbsrc.ac.uk'
or name like '%acas.org.uk'
or name like '%gov.wales'
or name like '%llyw.cymru'
or name like '%biglotteryfund.org.uk'
or name like '%parliament.scot'
or name like '%nhs.scot'
or name like '%highwaysengland.co.uk'
or name like '%sds.co.uk'
or name like '%scotent.co.uk'
or name like '%hient.co.uk'
or name like '%glasgowprestwick.com'
or name like '%revenue.scot'
)
and (exists 
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