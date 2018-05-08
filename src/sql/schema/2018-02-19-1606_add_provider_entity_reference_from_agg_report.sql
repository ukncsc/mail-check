CREATE TABLE IF NOT EXISTS `dmarc`.`provider` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `org_name` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX (`org_name` ASC)
  ) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `dmarc`.`aggregate_report` ADD COLUMN provider_id BIGINT UNSIGNED AFTER `org_name`;
ALTER TABLE `dmarc`.`aggregate_report` ADD CONSTRAINT `aggregate_report$provider_id`
    FOREIGN KEY (`provider_id`)
    REFERENCES `dmarc`.`provider` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION;
