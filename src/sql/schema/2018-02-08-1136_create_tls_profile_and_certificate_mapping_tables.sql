CREATE TABLE IF NOT EXISTS `dns_record_mx_tls_profile_2` (
  `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `mx_record_id` BIGINT UNSIGNED NOT NULL,
  `start_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `end_date` DATETIME NULL,
  `last_checked` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `failure_count` INT NULL DEFAULT 0,
  `test1_tls_version` INT NULL DEFAULT NULL,
  `test1_cipher_suite` INT NULL DEFAULT NULL,
  `test1_curve_group` INT NULL DEFAULT NULL,
  `test1_signature_hash_alg` INT NULL DEFAULT NULL,
  `test1_error` INT NULL DEFAULT NULL,
  `test2_tls_version` INT NULL DEFAULT NULL,
  `test2_cipher_suite` INT NULL DEFAULT NULL,
  `test2_curve_group` INT NULL DEFAULT NULL,
  `test2_signature_hash_alg` INT NULL DEFAULT NULL,
  `test2_error` INT NULL DEFAULT NULL,
  `test3_tls_version` INT NULL DEFAULT NULL,
  `test3_cipher_suite` INT NULL DEFAULT NULL,
  `test3_curve_group` INT NULL DEFAULT NULL,
  `test3_signature_hash_alg` INT NULL DEFAULT NULL,
  `test3_error` INT NULL DEFAULT NULL,
  `test4_tls_version` INT NULL DEFAULT NULL,
  `test4_cipher_suite` INT NULL DEFAULT NULL,
  `test4_curve_group` INT NULL DEFAULT NULL,
  `test4_signature_hash_alg` INT NULL DEFAULT NULL,
  `test4_error` INT NULL DEFAULT NULL,
  `test5_tls_version` INT NULL DEFAULT NULL,
  `test5_cipher_suite` INT NULL DEFAULT NULL,
  `test5_curve_group` INT NULL DEFAULT NULL,
  `test5_signature_hash_alg` INT NULL DEFAULT NULL,
  `test5_error` INT NULL DEFAULT NULL,
  `test6_tls_version` INT NULL DEFAULT NULL,
  `test6_cipher_suite` INT NULL DEFAULT NULL,
  `test6_curve_group` INT NULL DEFAULT NULL,
  `test6_signature_hash_alg` INT NULL DEFAULT NULL,
  `test6_error` INT NULL DEFAULT NULL,
  `test7_tls_version` INT NULL DEFAULT NULL,
  `test7_cipher_suite` INT NULL DEFAULT NULL,
  `test7_curve_group` INT NULL DEFAULT NULL,
  `test7_signature_hash_alg` INT NULL DEFAULT NULL,
  `test7_error` INT NULL DEFAULT NULL,
  `test8_tls_version` INT NULL DEFAULT NULL,
  `test8_cipher_suite` INT NULL DEFAULT NULL,
  `test8_curve_group` INT NULL DEFAULT NULL,
  `test8_signature_hash_alg` INT NULL DEFAULT NULL,
  `test8_error` INT NULL DEFAULT NULL,
  `test9_tls_version` INT NULL DEFAULT NULL,
  `test9_cipher_suite` INT NULL DEFAULT NULL,
  `test9_curve_group` INT NULL DEFAULT NULL,
  `test9_signature_hash_alg` INT NULL DEFAULT NULL,
  `test9_error` INT NULL DEFAULT NULL,
  `test10_tls_version` INT NULL DEFAULT NULL,
  `test10_cipher_suite` INT NULL DEFAULT NULL,
  `test10_curve_group` INT NULL DEFAULT NULL,
  `test10_signature_hash_alg` INT NULL DEFAULT NULL,
  `test10_error` INT NULL DEFAULT NULL,
  `test11_tls_version` INT NULL DEFAULT NULL,
  `test11_cipher_suite` INT NULL DEFAULT NULL,
  `test11_curve_group` INT NULL DEFAULT NULL,
  `test11_signature_hash_alg` INT NULL DEFAULT NULL,
  `test11_error` INT NULL DEFAULT NULL,
  `test12_tls_version` INT NULL DEFAULT NULL,
  `test12_cipher_suite` INT NULL DEFAULT NULL,
  `test12_curve_group` INT NULL DEFAULT NULL,
  `test12_signature_hash_alg` INT NULL DEFAULT NULL,
  `test12_error` INT NULL DEFAULT NULL,
  `test13_tls_version` INT NULL DEFAULT NULL,
  `test13_cipher_suite` INT NULL DEFAULT NULL,
  `test13_curve_group` INT NULL DEFAULT NULL,
  `test13_signature_hash_alg` INT NULL DEFAULT NULL,
  `test13_error` INT NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `mx_record_id_idx` (`mx_record_id` ASC),
  CONSTRAINT `dns_record_mx_tls_profile$mx_record_id0`
    FOREIGN KEY (`mx_record_id`)
    REFERENCES `dmarc`.`dns_record_mx` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


CREATE TABLE IF NOT EXISTS `certificate_mapping` (
  `sequence` INT UNSIGNED NOT NULL,
  `dns_record_mx_tls_profile_2_id` BIGINT UNSIGNED NOT NULL,
  `certificate_thumb_print` VARCHAR(255) NOT NULL,
  INDEX `fk_certificate_mapping_dns_record_mx_tls_profile_21_idx` (`dns_record_mx_tls_profile_2_id` ASC),
  INDEX `fk_certificate_mapping_certificate1_idx` (`certificate_thumb_print` ASC),
  PRIMARY KEY (`sequence`, `dns_record_mx_tls_profile_2_id`, `certificate_thumb_print`),
  CONSTRAINT `fk_certificate_mapping_dns_record_mx_tls_profile_21`
    FOREIGN KEY (`dns_record_mx_tls_profile_2_id`)
    REFERENCES `dmarc`.`dns_record_mx_tls_profile_2` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_certificate_mapping_certificate1`
    FOREIGN KEY (`certificate_thumb_print`)
    REFERENCES `dmarc`.`certificate` (`thumb_print`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

