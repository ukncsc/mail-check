
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
