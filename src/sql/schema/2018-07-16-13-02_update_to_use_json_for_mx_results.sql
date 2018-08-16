
CREATE TABLE IF NOT EXISTS `dmarc`.`dns_record_mx_tls_profile_results` (
  `id` BIGINT(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `mx_record_id` BIGINT(20) UNSIGNED NOT NULL,
  `start_date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `end_date` DATETIME NULL DEFAULT NULL,
  `last_checked` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `failure_count` INT(11) NULL DEFAULT '0',
  `data` JSON NOT NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `dns_record_mx_tls_profile_results$mx_record_id0`
    FOREIGN KEY (`mx_record_id`)
    REFERENCES `dmarc`.`dns_record_mx` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = latin1;

CREATE INDEX `mx_record_id_idx2` ON `dmarc`.`dns_record_mx_tls_profile_results` (`mx_record_id` ASC);

ALTER TABLE `dmarc`.`certificate_mapping` 
RENAME TO  `dmarc`.`certificate_mapping_old2` ;

CREATE TABLE `certificate_mapping` (
  `sequence` int(10) unsigned NOT NULL,
  `dns_record_mx_tls_profile_id` bigint(20) unsigned NOT NULL,
  `certificate_thumb_print` varchar(255) NOT NULL,
  PRIMARY KEY (`sequence`,`dns_record_mx_tls_profile_id`,`certificate_thumb_print`),
  KEY `fk_cert_mapping_dns_record_mx_tls_profile_results1_idx` (`dns_record_mx_tls_profile_id`),
  KEY `fk_cert_mapping_idx` (`certificate_thumb_print`),
  CONSTRAINT `fk_cert_mapping_cert_thump_print1` 
  FOREIGN KEY (`certificate_thumb_print`) REFERENCES `certificate` (`thumb_print`) 
	ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_cert_mapping_dns_record_mx_tls_profile_results1_new` 
  FOREIGN KEY (`dns_record_mx_tls_profile_id`) REFERENCES `dns_record_mx_tls_profile_results` (`id`) 
	ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE `dmarc`.`dns_record_mx_tls_evaluator_results` 
RENAME TO  `dmarc`.`dns_record_mx_tls_evaluator_results_old` ;

CREATE TABLE `dns_record_mx_tls_evaluator_results` (
  `mx_record_id` bigint(20) unsigned NOT NULL,
  `last_checked` datetime DEFAULT NULL,
  `tls_status` int(11) GENERATED ALWAYS AS (coalesce(greatest(CAST(json_extract(`data`,'$.tls12AvailableWithBestCipherSuiteSelected.Result') as signed),
  CAST(json_extract(`data`,'$.tls12AvailableWithBestCipherSuiteSelectedFromReverseList.Result') as signed),
  CAST(json_extract(`data`,'$.tls12AvailableWithSha2HashFunctionSelected.Result') as signed),
  CAST(json_extract(`data`,'$.tls12AvailableWithWeakCipherSuiteNotSelected.Result') as signed),
  CAST(json_extract(`data`,'$.tls11AvailableWithBestCipherSuiteSelected.Result') as signed),
  CAST(json_extract(`data`,'$.tls11AvailableWithWeakCipherSuiteNotSelected.Result') as signed),
  CAST(json_extract(`data`,'$.tls10AvailableWithBestCipherSuiteSelected.Result') as signed),
  CAST(json_extract(`data`,'$.tls10AvailableWithWeakCipherSuiteNotSelected.Result') as signed),
  CAST(json_extract(`data`,'$.ssl3FailsWithBadCipherSuite.Result') as signed),
  CAST(json_extract(`data`,'$.tlsSecureEllipticCurveSelected.Result') as signed),
  CAST(json_extract(`data`,'$.tlsSecureDiffieHellmanGroupSelected.Result') as signed),
  CAST(json_extract(`data`,'$.tlsWeakCipherSuitesRejected.Result') as signed),0))) STORED,  
  `data` json DEFAULT NULL,
  PRIMARY KEY (`mx_record_id`),
  UNIQUE KEY `mx_record_id` (`mx_record_id`),
  KEY `mx_record_id_idx` (`mx_record_id`),
  CONSTRAINT `dns_record_mx_tls_evaluator_results` FOREIGN KEY (`mx_record_id`) REFERENCES `dns_record_mx` (`id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

  
INSERT INTO dns_record_mx_tls_evaluator_results(mx_record_id, last_checked, data) 
SELECT mx_record_id, last_checked, json_object('tls12AvailableWithBestCipherSuiteSelected', json_object('Result', COALESCE(test1_result, -1), 'Description', test1_description), 
					"tls12AvailableWithBestCipherSuiteSelectedFromReverseList", json_object('Result', COALESCE(test2_result, -1), 'Description', test2_description),
					"tls12AvailableWithSha2HashFunctionSelected", json_object('Result', COALESCE(test3_result, -1), 'Description', test3_description),
					"tls12AvailableWithWeakCipherSuiteNotSelected", json_object('Result', COALESCE(test4_result, -1), 'Description', test4_description),
					"tls11AvailableWithBestCipherSuiteSelected", json_object('Result', COALESCE(test5_result, -1), 'Description', test5_description),
					"tls11AvailableWithWeakCipherSuiteNotSelected", json_object('Result', COALESCE(test6_result, -1), 'Description', test6_description),
					"tls10AvailableWithBestCipherSuiteSelected", json_object('Result', COALESCE(test7_result, -1), 'Description', test7_description),
					"tls10AvailableWithWeakCipherSuiteNotSelected", json_object('Result', COALESCE(test8_result, -1), 'Description', test8_description),
					"ssl3FailsWithBadCipherSuite", json_object('Result', COALESCE(test9_result, -1), 'Description', test9_description),
					"tlsSecureEllipticCurveSelected", json_object('Result', COALESCE(test10_result, -1), 'Description', test10_description),
					"tlsSecureDiffieHellmanGroupSelected", json_object('Result', COALESCE(test11_result, -1), 'Description', test11_description),
					"tlsWeakCipherSuitesRejected", json_object('Result', COALESCE(test12_result, -1), 'Description', test12_description)) FROM dns_record_mx_tls_evaluator_results_old;