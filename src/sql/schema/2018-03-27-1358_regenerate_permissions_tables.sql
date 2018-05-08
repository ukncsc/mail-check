
DROP EVENT IF EXISTS regenerate_permissions;
DROP EVENT IF EXISTS clear_permissions_daily;


DROP PROCEDURE IF EXISTS regenerate_permissions;
DROP PROCEDURE IF EXISTS clear_permissions_daily;

DROP TABLE IF EXISTS `derived_user_domain_permissions`;
DROP TABLE IF EXISTS `derived_user_domain_rollup_permissions`;

CREATE TABLE IF NOT EXISTS `derived_user_domain_permissions` (
  `user_id` INT UNSIGNED NOT NULL,
  `domain_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (user_id,domain_id)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE IF NOT EXISTS `derived_user_domain_rollup_permissions` (
  `user_id` INT UNSIGNED NOT NULL,
  `domain_id` INT UNSIGNED NOT NULL,
  `rollup` INT(1) NOT NULL,
  PRIMARY KEY (user_id,domain_id)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;




delimiter |

CREATE PROCEDURE regenerate_permissions()
BEGIN

REPLACE INTO derived_user_domain_permissions SELECT user.id user_id, domain.id domain_id
FROM user, domain
WHERE (domain.monitor = b'1' OR domain.publish = b'1') AND (user.global_admin = b'1'
OR EXISTS (SELECT 1
FROM group_user_mapping gum 
JOIN group_domain_mapping gdm ON gum.group_id = gdm.group_id
 WHERE  user.id=gum.user_id AND domain.id=gdm.domain_id));


REPLACE INTO derived_user_domain_rollup_permissions 
 SELECT user_id, domain_id, (SELECT domain_id NOT IN
 (SELECT ddt.child_id FROM derived_user_domain_permissions dp2
  JOIN derived_domain_tree ddt ON ddt.parent_id = dp2.domain_id 
  WHERE ddt.parent_id <> ddt.child_id AND dp2.user_id = dudp.user_id)) `rollup`
 from
 derived_user_domain_permissions dudp;

END |

CREATE EVENT regenerate_permissions
  ON SCHEDULE
    EVERY 5 MINUTE
DO BEGIN
CALL regenerate_permissions();
END |




CREATE PROCEDURE clear_permissions_daily()
BEGIN

TRUNCATE derived_user_domain_permissions;
TRUNCATE derived_user_domain_rollup_permissions;
call regenerate_permissions();

END |

CREATE EVENT clear_permissions_daily
  ON SCHEDULE
    EVERY 1 DAY
    STARTS '2018-02-09 00:00:00' ON COMPLETION PRESERVE ENABLE
DO BEGIN
CALL clear_permissions_daily();
END |


delimiter ;