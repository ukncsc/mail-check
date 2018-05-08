CREATE TABLE IF NOT EXISTS `dmarc`.`certificate` (
  `thumb_print` VARCHAR(255) NOT NULL,
  `issuer` VARCHAR(255) NOT NULL,
  `subject` VARCHAR(255) NOT NULL,
  `start_date` DATETIME NOT NULL,
  `end_date` DATETIME NOT NULL,
  `key_length` INT NOT NULL,
  `algorithm` VARCHAR(255) NOT NULL,
  `serial_number` VARCHAR(255) NOT NULL,
  `version` INT NOT NULL,
  `valid` BIT NOT NULL,
  PRIMARY KEY (`thumb_print`))
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `dmarc`.`dns_record_mx_tls_profile` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `mx_record_id` BIGINT UNSIGNED NOT NULL,
  `certificate_thumb_print` VARCHAR(255) NULL,
  `start_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `end_date` DATETIME NULL,
  `last_checked` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `failure_count` INT NULL DEFAULT 0,
  `sslv3_support` BIT NOT NULL,
  `sslv3_cipher` SMALLINT UNSIGNED NULL,
  `tlsv1_support` BIT NOT NULL,
  `tlsv1_cipher` SMALLINT UNSIGNED NULL,
  `tlsv11_support` BIT NOT NULL,
  `tlsv11_cipher` SMALLINT UNSIGNED NULL,
  `tlsv12_support` BIT NOT NULL,
  `tlsv12_cipher` SMALLINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  INDEX `mx_record_id_idx` (`mx_record_id` ASC),
  INDEX `dns_record_mx_tls_profile$certificate_thumb_print_idx` (`certificate_thumb_print` ASC),
  CONSTRAINT `dns_record_mx_tls_profile$mx_record_id`
    FOREIGN KEY (`mx_record_id`)
    REFERENCES `dmarc`.`dns_record_mx` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `dns_record_mx_tls_profile$certificate_thumb_print`
    FOREIGN KEY (`certificate_thumb_print`)
    REFERENCES `dmarc`.`certificate` (`thumb_print`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;