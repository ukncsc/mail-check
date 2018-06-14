


delimiter |

CREATE PROCEDURE import_new_domains_20180427()
BEGIN
DECLARE job_running int;
DECLARE started_at timestamp;

SELECT count(*) INTO @job_running FROM `derived_job_semaphore_20180427` where end_time IS NULL AND TIMESTAMPADD(HOUR,3,start_time) > CURRENT_TIMESTAMP;

IF @job_running = 0 THEN 

SELECT CURRENT_TIMESTAMP INTO @started_at;

INSERT INTO `derived_job_semaphore_20180427` (job_type, start_time) VALUES ('import_domains',@started_at);

INSERT INTO domain (name,monitor) 
 SELECT DISTINCT lcase(header_from) AS name ,b'0' AS monitor FROM record r LEFT JOIN domain ON domain.name = lcase(r.header_from) WHERE r.header_from_domain_id is null and domain.id IS NULL AND r.header_from IS NOT NULL;
UPDATE record r SET header_from_domain_id = (SELECT id FROM domain WHERE name = lcase(r.header_from)) WHERE header_from_domain_id IS NULL;


UPDATE `derived_job_semaphore_20180427` SET end_time=CURRENT_TIMESTAMP where `job_type` = 'import_domains' and `start_time` = @started_at;
END IF;

END |

delimiter ;

