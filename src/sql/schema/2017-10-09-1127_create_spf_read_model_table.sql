CREATE TABLE IF NOT EXISTS `dns_record_spf_read_model` (
  `domain_id` INT UNSIGNED NOT NULL,
  `error_count` INT NOT NULL,
  `max_error_severity` ENUM('error', 'warning', 'info') NULL,
  `model` TEXT NOT NULL,
  INDEX `fk_dns_record_spf_read_model_domain1_idx` (`domain_id` ASC),
  PRIMARY KEY (`domain_id`),
  CONSTRAINT `fk_dns_record_spf_read_model_domain1`
    FOREIGN KEY (`domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB