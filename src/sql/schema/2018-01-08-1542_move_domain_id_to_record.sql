ALTER TABLE `dmarc`.`record` ADD COLUMN header_from_domain_id INT UNSIGNED AFTER `header_from`;
ALTER TABLE `dmarc`.`aggregate_report` DROP FOREIGN KEY `aggregate_report$domain_id`;
ALTER TABLE `dmarc`.`aggregate_report` DROP COLUMN domain_id;
ALTER TABLE `dmarc`.`record` ADD CONSTRAINT `record$header_from_domain_id`
    FOREIGN KEY (`header_from_domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION;