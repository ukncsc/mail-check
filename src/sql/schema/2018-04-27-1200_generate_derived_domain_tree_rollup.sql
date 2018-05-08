CREATE TABLE IF NOT EXISTS `derived_user_domain_rollup_permissions_20180427` (
  `user_id` INT UNSIGNED NOT NULL,
  `domain_id` INT UNSIGNED NOT NULL,
  `rollup` INT(1) NOT NULL,
  PRIMARY KEY (user_id,domain_id)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;




DELIMITER |

CREATE PROCEDURE generate_derived_domain_tree_20180427()
BEGIN
DECLARE job_running int;
DECLARE started_at timestamp;

SELECT count(*) INTO @job_running FROM `derived_job_semaphore_20180427` where end_time IS NULL AND TIMESTAMPADD(HOUR,3,start_time) > CURRENT_TIMESTAMP;

IF @job_running = 0 THEN 

SELECT CURRENT_TIMESTAMP INTO @started_at;

INSERT INTO `derived_job_semaphore_20180427` (job_type, start_time) VALUES ('derived_domain_tree',@started_at);

DROP TABLE domain_elements_20180427;

CREATE TABLE domain_elements_20180427 AS 
SELECT id,SUBSTRING_INDEX(domain.name,'.',-2) element from domain 
UNION
SELECT id,SUBSTRING_INDEX(domain.name,'.',-3) element from domain 
UNION
SELECT id,SUBSTRING_INDEX(domain.name,'.',-4) element from domain 
UNION
SELECT id,SUBSTRING_INDEX(domain.name,'.',-5) element from domain 
UNION
SELECT id,SUBSTRING_INDEX(domain.name,'.',-6) element from domain 
UNION
SELECT id,SUBSTRING_INDEX(domain.name,'.',-7) element from domain 
UNION
SELECT id,SUBSTRING_INDEX(domain.name,'.',-8) element from domain
UNION
SELECT id,SUBSTRING_INDEX(domain.name,'.',-9) element from domain
UNION
SELECT id,SUBSTRING_INDEX(domain.name,'.',-10) element from domain;

ALTER TABLE domain_elements_20180427 add PRIMARY KEY (id,element);
ALTER TABLE domain_elements_20180427 add index idx_element (element);

DROP TABLE derived_domain_tree_20180427;

CREATE TABLE derived_domain_tree_20180427 AS SELECT cd.id child_id, cd.name child_name,IF(cd.monitor=b'1' OR cd.publish=b'1',b'1',b'0') child_visible, pd.id parent_id, pd.name parent_name, IF(pd.monitor=b'1' OR pd.publish=b'1',b'1',b'0') parent_visible,
pd.name regexp '^[a-z0-9]+\.(((ac|co|gov|ltd|me|net|nhs|org|plc|police|sch)\.uk)|com|org)$' parent_orgdomain
FROM domain pd
JOIN domain_elements_20180427 child_elements ON child_elements.element = pd.name
JOIN domain cd ON child_elements.id=cd.id;

ALTER TABLE derived_domain_tree_20180427 add PRIMARY KEY (child_id,parent_id);



UPDATE `derived_job_semaphore_20180427` SET end_time=CURRENT_TIMESTAMP where `job_type` = 'derived_domain_tree' and `start_time` = @started_at;

END IF;

END |

DELIMITER ;