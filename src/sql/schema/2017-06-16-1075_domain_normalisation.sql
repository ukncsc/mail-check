
CREATE TABLE IF NOT EXISTS `dmarc`.`user` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `firstname` VARCHAR(255) NULL,
  `lastname` VARCHAR(255) NULL,
  `email` VARCHAR(255) NULL,
  `disabled_by` INT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `email_UNIQUE` (`email` ASC),
  INDEX `user$disabled_by_idx` (`disabled_by` ASC),
  CONSTRAINT `user$disabled_by`
    FOREIGN KEY (`disabled_by`)
    REFERENCES `dmarc`.`user` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;



CREATE TABLE IF NOT EXISTS `dmarc`.`domain` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `marked_dead_by` INT UNSIGNED NULL,
  `name` VARCHAR(255) NOT NULL,
  `exdirectory` BIT NOT NULL DEFAULT 0,
  `publish` BIT NOT NULL DEFAULT 1,
  `created_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by` INT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  INDEX `marked_dead_by_idx` (`marked_dead_by` ASC),
  CONSTRAINT `domain$marked_dead_by`
    FOREIGN KEY (`marked_dead_by`)
    REFERENCES `dmarc`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `dmarc`.`dns_record_mx` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `domain_id` INT UNSIGNED NOT NULL,
  `preference` INT NOT NULL,
  `hostname` VARCHAR(255) NULL,
  `start_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `end_date` DATETIME NULL,
  `last_checked` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `failure_count` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `result_code` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_dns_record_mx_idx` (`domain_id` ASC),
  INDEX `idx_dns_record_ns_end_date` (`end_date` ASC),
  INDEX `idx_dns_record_ns_last_success` (`last_checked` ASC),
  CONSTRAINT `dns_record_mx$domain_id`
    FOREIGN KEY (`domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `dmarc`.`dns_record_ns` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `domain_id` INT UNSIGNED NOT NULL,
  `hostname` VARCHAR(255) NULL,
  `start_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `end_date` DATETIME NULL,
  `last_checked` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `failure_count` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `result_code` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_dns_record_ns_idx` (`domain_id` ASC),
  INDEX `idx_dns_record_ns_end_date` (`end_date` ASC),
  INDEX `idx_dns_record_ns_last_success` (`last_checked` ASC),
  CONSTRAINT `dns_record_ns$domain_id`
    FOREIGN KEY (`domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `dmarc`.`dns_record_dmarc` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `domain_id` INT UNSIGNED NOT NULL,
  `record` TEXT NULL,
  `start_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `end_date` DATETIME NULL,
  `last_checked` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `failure_count` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `result_code` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_dns_record_dmarc_idx` (`domain_id` ASC),
  INDEX `idx_dns_record_dmarc_end_date` (`end_date` ASC),
  INDEX `idx_dns_record_dmarc_last_success` (`last_checked` ASC),
  CONSTRAINT `dns_record_dmarc$domain_id`
    FOREIGN KEY (`domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `dmarc`.`dns_record_spf` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `domain_id` INT UNSIGNED NOT NULL,
  `record` TEXT NULL,
  `start_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `end_date` DATETIME NULL,
  `last_checked` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
  `failure_count` TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `result_code` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_dns_record_spf_idx` (`domain_id` ASC),
  INDEX `idx_dns_record_spf_end_date` (`end_date` ASC),
  INDEX `idx_dns_record_spf_last_success` (`last_checked` ASC),
  CONSTRAINT `dns_record_spf$domain_id`
    FOREIGN KEY (`domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

ALTER TABLE `dmarc`.`aggregate_report` 
ADD COLUMN `domain_id` INT UNSIGNED AFTER `domain`;

ALTER TABLE `dmarc`.`aggregate_report` 
ADD CONSTRAINT `aggregate_report$domain_id`
    FOREIGN KEY (`domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION;


INSERT INTO `dmarc`.`domain` (`name`)
 SELECT DISTINCT `domain` FROM `aggregate_report`;



