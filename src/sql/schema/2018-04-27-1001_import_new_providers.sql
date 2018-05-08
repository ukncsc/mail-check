
delimiter |

CREATE PROCEDURE import_new_providers_20180427()
BEGIN
DECLARE job_running int;
DECLARE started_at timestamp;

SELECT count(*) INTO @job_running FROM `derived_job_semaphore_20180427` where end_time IS NULL AND TIMESTAMPADD(HOUR,3,start_time) > CURRENT_TIMESTAMP;

IF @job_running = 0 THEN 

SELECT CURRENT_TIMESTAMP INTO @started_at;

INSERT INTO `derived_job_semaphore_20180427` (job_type, start_time) VALUES ('import_providers',@started_at);

INSERT INTO provider (org_name) 
 SELECT DISTINCT org_name FROM aggregate_report ar LEFT JOIN provider p USING (org_name) WHERE ar.provider_id is null and p.id IS NULL AND ar.org_name IS NOT NULL;

UPDATE aggregate_report ar SET provider_id = (SELECT id FROM provider WHERE org_name = ar.org_name) WHERE provider_id IS NULL;

UPDATE `derived_job_semaphore_20180427` SET end_time=CURRENT_TIMESTAMP where `job_type` = 'import_providers' and `start_time` = @started_at;
END IF;

END |

delimiter ;



