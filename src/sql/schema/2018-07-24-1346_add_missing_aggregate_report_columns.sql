ALTER TABLE `aggregate_report`
ADD COLUMN `version` DOUBLE NULL DEFAULT NULL AFTER `attachment_filename`,
ADD COLUMN `fo` VARCHAR(255) NULL DEFAULT NULL AFTER `pct`;

ALTER TABLE `record`
ADD COLUMN `envelope_from` VARCHAR(255) NULL DEFAULT NULL AFTER `envelope_to`;

ALTER TABLE `dkim_auth_result` 
ADD COLUMN `selector` VARCHAR(255) NULL DEFAULT NULL AFTER `domain`;

ALTER TABLE `spf_auth_result` 
ADD COLUMN `scope` ENUM('helo','mfrom') NULL DEFAULT NULL AFTER `domain`;

