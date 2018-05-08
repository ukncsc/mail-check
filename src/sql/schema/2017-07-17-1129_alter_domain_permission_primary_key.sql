ALTER TABLE `dmarc`.`domain_permission` 
DROP PRIMARY KEY,
ADD COLUMN `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT FIRST,
ADD PRIMARY KEY (`id`),
DROP FOREIGN KEY `domain_permission$domain_id`,
MODIFY `domain_id` INT UNSIGNED NULL,
MODIFY `granted_on` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

ALTER TABLE `dmarc`.`domain_permission` 
ADD CONSTRAINT `domain_permission$domain_id`
    FOREIGN KEY (`domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION;

