ALTER TABLE `dmarc`.`domain_permission` ADD COLUMN (
  `revoked_by` INT UNSIGNED,
  `granted_on` DATETIME,
  `revoked_on` DATETIME);
ALTER TABLE `dmarc`.`domain_permission` ADD  CONSTRAINT `domain_permission$revoked_by`
    FOREIGN KEY (`revoked_by`)
    REFERENCES `dmarc`.`user` (`id`); 
