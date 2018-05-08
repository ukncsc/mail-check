
-- -----------------------------------------------------
-- Table `dmarc`.`ip_org`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`ip_org` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `handle` VARCHAR(255) NULL DEFAULT NULL,
  `name` VARCHAR(255) NULL DEFAULT NULL,
  `address` VARCHAR(255) NULL DEFAULT NULL,
  `city` VARCHAR(255) NULL DEFAULT NULL,
  `country` CHAR(2) NULL DEFAULT NULL,
  PRIMARY KEY (`id`));

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`ip_subnet`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`ip_subnet` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `org_id` BIGINT UNSIGNED NOT NULL,
  `address` VARCHAR(255) NULL DEFAULT NULL,
  `name` VARCHAR(255) NULL DEFAULT NULL,
  `organisation` VARCHAR(255) NULL DEFAULT NULL,
  `retrieved` DATE NULL DEFAULT NULL,
  `binary_address` VARBINARY(16) NULL DEFAULT NULL,
  `netmask` VARBINARY(16) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `ip_subnet$ip_org` (`org_id` ASC),
  CONSTRAINT `ip_subnet$ip_org`
    FOREIGN KEY (`org_id`)
    REFERENCES `dmarc`.`ip_org` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`ip_address`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`ip_address` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `address` VARCHAR(255) NULL DEFAULT NULL,
  `binary_address` VARBINARY(16) NULL DEFAULT NULL,
  `subnet_id` BIGINT UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX (`address` ASC),
  INDEX `ip_address$ip_subnet` (`subnet_id` ASC),
  CONSTRAINT `ip_address$ip_subnet`
    FOREIGN KEY (`subnet_id`)
    REFERENCES `dmarc`.`ip_subnet` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`dkim_auth_result`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`dkim_auth_result` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `record_id` INT(10) UNSIGNED NOT NULL,
  `domain` VARCHAR(255) NOT NULL,
  `dkim_result` ENUM('none', 'pass', 'fail', 'policy', 'neutral', 'temperror', 'permerror') NULL DEFAULT NULL,
  `human_result` VARCHAR(500) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `dkim_auth_result$record` (`record_id` ASC),
  CONSTRAINT `dkim_auth_result$record`
    FOREIGN KEY (`record_id`)
    REFERENCES `dmarc`.`record` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`spf_auth_result`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`spf_auth_result` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `record_id` INT(10) UNSIGNED NOT NULL,
  `domain` VARCHAR(255) NOT NULL,
  `spf_result` ENUM('none', 'neutral', 'pass', 'fail', 'softfail', 'temperror', 'permerror') NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `spf_auth_result$record` (`record_id` ASC),
  CONSTRAINT `spf_auth_result$record`
    FOREIGN KEY (`record_id`)
    REFERENCES `dmarc`.`record` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_report`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_report` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `original_uri` VARCHAR(255) NOT NULL,
  `feedback_type` ENUM('Abuse', 'Fraud', 'Other', 'Virus', 'AuthFailure') NULL DEFAULT NULL,
  `user_agent` VARCHAR(255) NULL DEFAULT NULL,
  `version` VARCHAR(255) NULL DEFAULT NULL,
  `auth_failure` ENUM('Adsp', 'BodyHash', 'Revoked', 'Signature', 'Spf', 'Dmarc') NULL DEFAULT NULL,
  `original_envelope_id` VARCHAR(255) NULL DEFAULT NULL,
  `arrival_date` DATETIME NULL DEFAULT NULL,
  `reporting_mta` VARCHAR(255) NULL DEFAULT NULL,
  `source_ip_id` BIGINT UNSIGNED NOT NULL,
  `incidents` INT UNSIGNED NULL DEFAULT NULL,
  `delivery_result` ENUM('Delivered', 'Spam', 'Policy', 'Reject', 'Other') NULL DEFAULT NULL,
  `provider_message_id` VARCHAR(255) NOT NULL,
  `message_id` VARCHAR(255) NULL,
  `dkim_domain` VARCHAR(255) NULL DEFAULT NULL,
  `dkim_identity` VARCHAR(255) NULL DEFAULT NULL,
  `dkim_selector` VARCHAR(255) NULL DEFAULT NULL,
  `dkim_canonicalized_header` TEXT NULL DEFAULT NULL,
  `spf_dns` VARCHAR(255) NULL DEFAULT NULL,
  `authentication_results` VARCHAR(255) NULL DEFAULT NULL,
  `reported_domain` VARCHAR(255) NULL DEFAULT NULL,
  `created_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `request_id` VARCHAR(255) NOT NULL,
  `dkim_canonicalized_body` MEDIUMTEXT NULL,
  PRIMARY KEY (`id`),
  INDEX (`reported_domain` ASC),
  INDEX `forensic_report$ip_address` (`source_ip_id` ASC),
  UNIQUE INDEX `original_uri_UNIQUE` (`original_uri` ASC),
  UNIQUE INDEX `provider_message_id_UNIQUE` (`provider_message_id` ASC),
  CONSTRAINT `forensic_report$ip_address`
    FOREIGN KEY (`source_ip_id`)
    REFERENCES `dmarc`.`ip_address` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`email_address`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`email_address` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `address` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `address_UNIQUE` (`address` ASC));

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_report_rcpt_to`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_report_rcpt_to` (
  `report_id` BIGINT UNSIGNED NOT NULL,
  `rcpt_to_id` BIGINT UNSIGNED NOT NULL,
  INDEX `rcpt_to_address_idx` (`rcpt_to_id` ASC),
  PRIMARY KEY (`report_id`, `rcpt_to_id`),
  CONSTRAINT `forensic_report_rcpt_to$forensic_report`
    FOREIGN KEY (`report_id`)
    REFERENCES `dmarc`.`forensic_report` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `forensic_report_rcpt_to$email_address`
    FOREIGN KEY (`rcpt_to_id`)
    REFERENCES `dmarc`.`email_address` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_text`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_text` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `body` MEDIUMTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id`));

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_text_hash`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_text_hash` (
  `text_id` BIGINT UNSIGNED NOT NULL,
  `type` ENUM('MD5', 'SHA-1', 'SHA-256', 'SHA-512', 'SHA3-256', 'SHA3-512') NOT NULL,
  `hash` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`hash`, `type`),
  INDEX `forensic_text_hash$forensic_text` (`text_id` ASC),
  CONSTRAINT `forensic_text_hash$forensic_text`
    FOREIGN KEY (`text_id`)
    REFERENCES `dmarc`.`forensic_text` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_binary`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_binary` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `attachment` BLOB NOT NULL,
  PRIMARY KEY (`id`));

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_binary_hash`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_binary_hash` (
  `binary_id` BIGINT UNSIGNED NOT NULL,
  `type` ENUM('MD5', 'SHA-1', 'SHA-256', 'SHA-512', 'SHA3-256', 'SHA3-512') NOT NULL,
  `hash` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`hash`, `type`),
  INDEX `forensic_binary_hash$forensic_binary` (`binary_id` ASC),
  CONSTRAINT `forensic_binary_hash$forensic_binary`
    FOREIGN KEY (`binary_id`)
    REFERENCES `dmarc`.`forensic_binary` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_binary_report`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_binary_report` (
  `binary_id` BIGINT UNSIGNED NOT NULL,
  `source` VARCHAR(255) NULL DEFAULT NULL,
  `detail` TEXT NULL DEFAULT NULL,
  `found_on` DATE NULL DEFAULT NULL,
  `type` ENUM('Malware') NULL DEFAULT NULL,
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`),
  INDEX `forensic_binary_report$forensic_binary_idx` (`binary_id` ASC),
  CONSTRAINT `forensic_binary_report$forensic_binary`
    FOREIGN KEY (`binary_id`)
    REFERENCES `dmarc`.`forensic_binary` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_uri`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_uri` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `uri` TEXT NOT NULL,
  `uri_hash_sha256` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uri_hash_UNIQUE` (`uri_hash_sha256` ASC));

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_uri_report`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_uri_report` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `uri_id` BIGINT UNSIGNED NOT NULL,
  `source` VARCHAR(255) NULL DEFAULT NULL,
  `detail` TEXT NULL DEFAULT NULL,
  `found_on` DATE NULL DEFAULT NULL,
  `type` ENUM('Phishing', 'Malware') NULL DEFAULT NULL,
  INDEX `forensic_uri_report$forensic_uri` (`uri_id` ASC),
  PRIMARY KEY (`id`),
  CONSTRAINT `forensic_uri_report$forensic_uri`
    FOREIGN KEY (`uri_id`)
    REFERENCES `dmarc`.`forensic_uri` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_uri_match`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_uri_match` (
  `forensic_text_id` BIGINT UNSIGNED NOT NULL,
  `uri_id` BIGINT UNSIGNED NOT NULL,
  INDEX `forensic_uri_match$forensic_uri` (`uri_id` ASC),
  INDEX `_idx` (`forensic_text_id` ASC),
  PRIMARY KEY (`forensic_text_id`, `uri_id`),
  CONSTRAINT `forensic_uri_match$forensic_text`
    FOREIGN KEY (`forensic_text_id`)
    REFERENCES `dmarc`.`forensic_text` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `forensic_uri_match$forensic_uri`
    FOREIGN KEY (`uri_id`)
    REFERENCES `dmarc`.`forensic_uri` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`content_type`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`content_type` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_text_match`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_text_match` (
  `report_id` BIGINT UNSIGNED NOT NULL,
  `body_id` BIGINT UNSIGNED NOT NULL,
  `order` TINYINT UNSIGNED NULL,
  `depth` TINYINT UNSIGNED NULL,
  `content_type_id` INT UNSIGNED NULL,
  INDEX `forensic_text_match$forensic_report` (`report_id` ASC),
  INDEX `forensic_text_match$forensic_text` (`body_id` ASC),
  INDEX `content_type_idx` (`content_type_id` ASC),
  PRIMARY KEY (`report_id`, `body_id`, `order`),
  CONSTRAINT `forensic_text_match$forensic_report`
    FOREIGN KEY (`report_id`)
    REFERENCES `dmarc`.`forensic_report` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `forensic_text_match$forensic_text`
    FOREIGN KEY (`body_id`)
    REFERENCES `dmarc`.`forensic_text` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `forensic_text_match$content_type`
    FOREIGN KEY (`content_type_id`)
    REFERENCES `dmarc`.`content_type` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_binary_match`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_binary_match` (
  `report_id` BIGINT UNSIGNED NOT NULL,
  `binary_id` BIGINT UNSIGNED NOT NULL,
  `filename` VARCHAR(255) NULL,
  `extension` VARCHAR(255) NULL,
  `disposition` ENUM('inline', 'attachment') NULL,
  `content_type_id` INT UNSIGNED NULL,
  `order` TINYINT UNSIGNED NULL,
  `depth` TINYINT UNSIGNED NULL,
  INDEX `forensic_binary_match$forensic_report` (`report_id` ASC),
  INDEX `forensic_binary_match$forensic_binary` (`binary_id` ASC),
  INDEX `content_type_idx` (`content_type_id` ASC),
  PRIMARY KEY (`report_id`, `binary_id`),
  CONSTRAINT `forensic_binary_match$forensic_report`
    FOREIGN KEY (`report_id`)
    REFERENCES `dmarc`.`forensic_report` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `forensic_binary_match$forensic_binary`
    FOREIGN KEY (`binary_id`)
    REFERENCES `dmarc`.`forensic_binary` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `forensic_binary_match$content_type`
    FOREIGN KEY (`content_type_id`)
    REFERENCES `dmarc`.`content_type` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`hostname`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`hostname` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NOT NULL,
  `ip_address_id` BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  INDEX `ip_address_idx` (`ip_address_id` ASC),
  CONSTRAINT `hostname$ip_address`
    FOREIGN KEY (`ip_address_id`)
    REFERENCES `dmarc`.`ip_address` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`rfc822_header_fields`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`rfc822_header_fields` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NULL,
  `value_type` ENUM('text', 'email', 'ip', 'hostname', 'date') NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`rfc822_header_text_values`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`rfc822_header_text_values` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `value` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`value` ASC))
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`rfc822_header_set`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`rfc822_header_set` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `report_id` BIGINT UNSIGNED NOT NULL,
  `order` TINYINT UNSIGNED NOT NULL,
  `depth` TINYINT UNSIGNED NOT NULL,
  `content_type_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `report_id_idx` (`report_id` ASC),
  INDEX `content_type_idx` (`content_type_id` ASC),
  CONSTRAINT `rfc822_header_set$forensic_report`
    FOREIGN KEY (`report_id`)
    REFERENCES `dmarc`.`forensic_report` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `rfc822_header_set$content_type`
    FOREIGN KEY (`content_type_id`)
    REFERENCES `dmarc`.`content_type` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`rfc822_header_mapping`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`rfc822_header_mapping` (
  `set_id` BIGINT UNSIGNED NOT NULL,
  `field_id` INT UNSIGNED NOT NULL,
  `order_in_set` TINYINT UNSIGNED NOT NULL,
  `text_value_id` BIGINT UNSIGNED NULL,
  `email_address_id` BIGINT UNSIGNED NULL,
  `ip_address_id` BIGINT UNSIGNED NULL,
  `hostname_id` BIGINT UNSIGNED NULL,
  `date` DATETIME NULL,
  `processed` BIT NOT NULL DEFAULT 0,
  INDEX `value_idx` (`text_value_id` ASC),
  INDEX `field_idx` (`field_id` ASC),
  INDEX `email_idx` (`email_address_id` ASC),
  INDEX `ip_idx` (`ip_address_id` ASC),
  INDEX `hostname_idx` (`hostname_id` ASC),
  INDEX `header_set_idx` (`set_id` ASC),
  PRIMARY KEY (`set_id`, `field_id`, `order_in_set`),
  CONSTRAINT `rfc822_header_mapping$rfc822_header_text_values`
    FOREIGN KEY (`text_value_id`)
    REFERENCES `dmarc`.`rfc822_header_text_values` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `rfc822_header_mapping$rfc822_header_fields`
    FOREIGN KEY (`field_id`)
    REFERENCES `dmarc`.`rfc822_header_fields` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `rfc822_header_mapping$email_address`
    FOREIGN KEY (`email_address_id`)
    REFERENCES `dmarc`.`email_address` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `rfc822_header_mapping$ip_address`
    FOREIGN KEY (`ip_address_id`)
    REFERENCES `dmarc`.`ip_address` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `rfc822_header_mapping$hostname`
    FOREIGN KEY (`hostname_id`)
    REFERENCES `dmarc`.`hostname` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `rfc822_header_mapping$rfc822_header_set`
    FOREIGN KEY (`set_id`)
    REFERENCES `dmarc`.`rfc822_header_set` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_report_mail_from`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_report_mail_from` (
  `report_id` BIGINT UNSIGNED NOT NULL,
  `original_mail_from_id` BIGINT UNSIGNED NOT NULL,
  INDEX `mail_from_idx` (`original_mail_from_id` ASC),
  INDEX `report_id_idx` (`report_id` ASC),
  PRIMARY KEY (`report_id`, `original_mail_from_id`),
  CONSTRAINT `forensic_report_mail_from$email_address`
    FOREIGN KEY (`original_mail_from_id`)
    REFERENCES `dmarc`.`email_address` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `forensic_report_mail_from$forensic_report`
    FOREIGN KEY (`report_id`)
    REFERENCES `dmarc`.`forensic_report` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION);

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `dmarc`.`forensic_reported_uri`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`forensic_reported_uri` (
  `uri_id` BIGINT UNSIGNED NOT NULL,
  `report_id` BIGINT UNSIGNED NOT NULL,
  PRIMARY KEY (`uri_id`, `report_id`),
  INDEX `report_idx` (`report_id` ASC),
  CONSTRAINT `forensic_reported_uri$forensic_uri`
    FOREIGN KEY (`uri_id`)
    REFERENCES `dmarc`.`forensic_uri` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `forensic_reported_uri$forensic_report`
    FOREIGN KEY (`report_id`)
    REFERENCES `dmarc`.`forensic_report` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

SHOW WARNINGS;

