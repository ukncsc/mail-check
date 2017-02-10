SET GLOBAL event_scheduler = ON;

DROP EVENT daily_generate_derived_tables;
DROP PROCEDURE generate_derived_tables;
DROP PROCEDURE generate_derived_policy_history;
DROP PROCEDURE generate_derived_domain;
DROP PROCEDURE generate_derived_source;
DROP PROCEDURE generate_derived_aggregate_daily;
DROP PROCEDURE generate_derived_aggregate_weekly;

delimiter |

CREATE PROCEDURE generate_derived_policy_history()
BEGIN

# DMARC Policy History
# This query finds the most common DMARC policy for a given day per domain, then finds the last day a that policy was active.

DROP TABLE IF EXISTS derived_policy_history;
SET @p_domain = "-";
CREATE TABLE derived_policy_history AS
SELECT domain, (IF(domain_changed_flag = 1,NULL ,@p_ed)) 'date_to', (@p_ed := effective_date) date_from,adkim,aspf,p,sp,pct from 
	(select @p_domain 'previous_domain',(@dcf := @p_domain != domain) 'domain_changed_flag',        
	(@p_adkim != adkim or @p_aspf != aspf or @p_p != p or @p_sp != sp or @p_pct != pct) 'policy_changed_flag',       
	(@p_domain :=domain) domain,        
	effective_date,          
	(@p_adkim := adkim) 'adkim',        
	(@p_aspf := aspf) 'aspf',         
	(@p_p := p) 'p',         
	(@p_sp := sp) 'sp',        
	(@p_pct := pct) 'pct'         
	from         
	(select  domain, effective_date as 'effective_date', adkim, aspf, p, sp, pct
        from aggregate_report ar1                  
	where effective_date <= date_sub(current_date, interval 2 day) and
	(adkim, aspf, p, sp, pct) =                         
	(select adkim,aspf,p,sp,pct from aggregate_report ar2                        
	where ar1.domain=ar2.domain and ar1.effective_date = ar2.effective_date                        
	group by adkim,aspf,p,sp,pct order by count(*) desc limit 1)                    
	order by domain, effective_date desc) results ) results2  where policy_changed_flag or domain_changed_flag;


alter table derived_policy_history add primary key (domain, date_from);

END |



CREATE PROCEDURE generate_derived_aggregate_daily()
BEGIN
DROP TABLE IF EXISTS derived_aggregate_daily;

CREATE TABLE derived_aggregate_daily as SELECT domain,effective_date, 
COALESCE(SUM(CASE WHEN (r.dkim = 'pass' AND r.spf = 'pass') THEN r.count ELSE 0 END),0) 'full_compliance_count', 
COALESCE(SUM(CASE WHEN (r.dkim = 'pass' AND r.spf != 'pass') THEN r.count ELSE 0 END),0) 'dkim_only_count', 
COALESCE(SUM(CASE WHEN (r.dkim != 'pass' AND r.spf = 'pass') THEN r.count ELSE 0 END),0) 'spf_only_count',
COALESCE(SUM(CASE WHEN (r.dkim != 'pass' AND r.spf != 'pass') THEN r.count ELSE 0 END),0) as 'untrusted_email_count',
COALESCE(SUM(CASE WHEN (r.dkim = 'pass' OR r.spf = 'pass') THEN r.count ELSE 0 END),0) as 'trusted_email_count',
COALESCE(SUM(CASE WHEN (r.disposition = 'quarantine') THEN r.count ELSE 0 END),0) 'disposition_quarantine_count',
COALESCE(SUM(CASE WHEN (r.disposition = 'reject') THEN r.count ELSE 0 END),0) 'disposition_reject_count',
COALESCE(SUM(CASE WHEN (r.disposition = 'none') THEN r.count ELSE 0 END),0) 'disposition_none_count',
SUM(r.count) 'total_email_count',
count(distinct ar.id) 'aggregate_report_count',
count(distinct r.id) 'aggregate_report_record_count'
FROM aggregate_report ar JOIN record r ON ar.id = r.aggregate_report_id  
group by domain,effective_date;

ALTER TABLE derived_aggregate_daily ADD PRIMARY KEY (domain, effective_date);


CREATE OR REPLACE VIEW aggregate_daily_view as 
select dd.id 'domainId',
dd.domain 'domain', 
dad.effective_date 'effective_date',
dad.full_compliance_count  'full_compliance_count',
dad.dkim_only_count 'dkim_only_count',
dad.spf_only_count 'spf_only_count',
dad.untrusted_email_count 'untrusted_email_count',
dad.trusted_email_count 'trusted_email_count',
dad.disposition_quarantine_count 'disposition_quarantine_count',
dad.disposition_reject_count 'disposition_reject_count',
dad.disposition_none_count 'disposition_none_count',
dad.total_email_count 'total_email_count',
dad.aggregate_report_count 'aggregate_report_count',
dad.aggregate_report_record_count 'aggregate_report_record_count'
from derived_aggregate_daily dad, derived_domain dd
where effective_date < date_sub(current_date, interval 2 day) 
and dad.domain = dd.domain
union 
SELECT dd.id 'domainId',dd.domain 'domain',effective_date, 
COALESCE(SUM(CASE WHEN (r.dkim = 'pass' AND r.spf = 'pass') THEN r.count ELSE 0 END),0) 'full_compliance_count', 
COALESCE(SUM(CASE WHEN (r.dkim = 'pass' AND r.spf != 'pass') THEN r.count ELSE 0 END),0) 'dkim_only_count', 
COALESCE(SUM(CASE WHEN (r.dkim != 'pass' AND r.spf = 'pass') THEN r.count ELSE 0 END),0) 'spf_only_count',
COALESCE(SUM(CASE WHEN (r.dkim != 'pass' AND r.spf != 'pass') THEN r.count ELSE 0 END),0) as 'untrusted_email_count',
COALESCE(SUM(CASE WHEN (r.dkim = 'pass' OR r.spf = 'pass') THEN r.count ELSE 0 END),0) as 'trusted_email_count',
COALESCE(SUM(CASE WHEN (r.disposition = 'quarantine') THEN r.count ELSE 0 END),0) 'disposition_quarantine_count',
COALESCE(SUM(CASE WHEN (r.disposition = 'reject') THEN r.count ELSE 0 END),0) 'disposition_reject_count',
COALESCE(SUM(CASE WHEN (r.disposition = 'none') THEN r.count ELSE 0 END),0) 'disposition_none_count',
SUM(r.count) 'total_email_count',
count(distinct ar.id) 'aggregate_report_count',
count(distinct r.id) 'aggregate_report_record_count'
FROM derived_domain dd JOIN aggregate_report ar on dd.domain=ar.domain JOIN record r ON ar.id = r.aggregate_report_id
AND ar.domain = dd.domain
WHERE ar.effective_date >= date_sub(current_date, interval 2 day) 
GROUP BY ar.domain,ar.effective_date;

END |

CREATE PROCEDURE generate_derived_aggregate_weekly()
BEGIN

DROP TABLE IF EXISTS derived_aggregate_weekly;

CREATE TABLE derived_aggregate_weekly as SELECT domain,STR_TO_DATE(CONCAT(YEARWEEK(effective_date),' Monday'), '%X%V %W') as effective_date, 
sum(full_compliance_count) 'full_compliance_count', 
sum(dkim_only_count) 'dkim_only_count', 
sum(spf_only_count) 'spf_only_count',
sum(untrusted_email_count) 'untrusted_email_count',
sum(trusted_email_count) 'trusted_email_count',
sum(disposition_quarantine_count) 'disposition_quarantine_count',
sum(disposition_reject_count) 'disposition_reject_count',
sum(disposition_none_count) 'disposition_none_count',
sum(total_email_count) 'total_email_count',
sum(aggregate_report_count) 'aggregate_report_count',
sum(aggregate_report_record_count) 'aggregate_report_record_count'
FROM aggregate_daily_view  
group by domain,yearweek(effective_date);

ALTER TABLE derived_aggregate_weekly ADD PRIMARY KEY (domain, effective_date);

CREATE OR REPLACE VIEW aggregate_weekly_view as select * from derived_aggregate_weekly, derived_domain 
where yearweek(effective_date) < yearweek(date_sub(current_date, interval 2 day))
and derived_aggregate_weekly.domain = derived_domain.domain
union 
SELECT domain,STR_TO_DATE(CONCAT(YEARWEEK(effective_date),' Monday'), '%X%V %W') as effective_date, 
sum(full_compliance_count) 'full_compliance_count', 
sum(dkim_only_count) 'dkim_only_count', 
sum(spf_only_count) 'spf_only_count',
sum(untrusted_email_count) 'untrusted_email_count',
sum(trusted_email_count) 'trusted_email_count',
sum(disposition_quarantine_count) 'disposition_quarantine_count',
sum(disposition_reject_count) 'disposition_reject_count',
sum(disposition_none_count) 'disposition_none_count',
sum(total_email_count) 'total_email_count',
sum(aggregate_report_count) 'aggregate_report_count',
sum(aggregate_report_record_count) 'aggregate_report_record_count'
FROM aggregate_daily_view, derived_domain where yearweek(effective_date) >= yearweek(date_sub(current_date, interval 2 day))
AND derived_domain.domain = aggregate_daily_view.domain
group by domain,yearweek(effective_date);

END |

CREATE PROCEDURE generate_derived_domain()
BEGIN
#Table to give list of domains, used for autocomplete

CREATE TABLE IF NOT EXISTS derived_domain (id INT UNSIGNED NOT NULL AUTO_INCREMENT,domain VARCHAR(256), PRIMARY KEY(domain), UNIQUE INDEX(id));
INSERT IGNORE INTO derived_domain (domain) (SELECT distinct(domain) from aggregate_weekly_view);

END |


CREATE PROCEDURE generate_derived_source()
BEGIN

DROP TABLE IF EXISTS derived_bad_source;
CREATE TABLE derived_source as 
	select distinct source_ip, domain 
	from record, aggregate_report 
	where record.aggregate_report_id = aggregate_report.id 
	and aggregate_report.aspf != "pass" 
	and aggregate_report.adkim != "pass";


END |

CREATE PROCEDURE generate_derived_aspf_source()
BEGIN
DROP TABLE IF EXISTS derived_aspf_source;
CREATE TABLE derived_aspf_source as
	select distinct source_ip, domain 
	from aggregate_report,record 
	where aggregate_report.id = record.aggregate_report_id 
	and spf="pass";

END |
	
CREATE PROCEDURE generate_derived_tables()
BEGIN
CALL generate_derived_aggregate_daily();
CALL generate_derived_aggregate_weekly();
CALL generate_derived_domain();
CALL generate_derived_source();
CALL generate_derived_policy_history();
CALL generate_derived_aspf_source();
END |


CREATE EVENT daily_generate_derived_tables
  ON SCHEDULE
    EVERY 1 DAY
DO BEGIN
CALL generate_derived_tables();
END |

delimiter ;
