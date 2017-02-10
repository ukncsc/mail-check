CREATE TABLE `VersionInfo` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Version` bigint(20) NOT NULL,
  `AppliedOn` datetime NOT NULL,
  `Description` text NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

INSERT INTO `VersionInfo` (`Id`,`Version`,`AppliedOn`,`Description`) VALUES (1,201612201316,'2017-01-06 15:13:02','InitialMigration');

CREATE TABLE `aggregate_report` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `request_id` varchar(36) NOT NULL,
  `original_uri` varchar(150) NOT NULL,
  `attachment_filename` varchar(200) NOT NULL,
  `org_name` varchar(100) NOT NULL,
  `email` varchar(256) NOT NULL,
  `report_id` varchar(250) NOT NULL,
  `extra_contact_info` varchar(250) DEFAULT NULL,
  `begin_date` datetime NOT NULL,
  `end_date` datetime NOT NULL,
  `domain` varchar(256) NOT NULL,
  `adkim` enum('r','s') DEFAULT NULL,
  `aspf` enum('r','s') DEFAULT NULL,
  `p` enum('none','quarantine','reject') NOT NULL,
  `sp` enum('none','quarantine','reject') DEFAULT NULL,
  `pct` int(3) unsigned DEFAULT NULL,
  `created_date` datetime NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  UNIQUE KEY `report_id_UNIQUE` (`report_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9717 DEFAULT CHARSET=latin1;

CREATE TABLE `record` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `aggregate_report_id` int(10) unsigned NOT NULL,
  `source_ip` varchar(39) NOT NULL,
  `count` int(10) unsigned NOT NULL,
  `disposition` enum('none','quarantine','reject') DEFAULT NULL,
  `dkim` enum('pass','fail') DEFAULT NULL,
  `spf` enum('pass','fail') NOT NULL,
  `envelope_to` varchar(256) DEFAULT NULL,
  `header_from` varchar(256) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_aggregate_report$record_idx` (`aggregate_report_id`),
  CONSTRAINT `fk_aggregate_report$record` FOREIGN KEY (`aggregate_report_id`) REFERENCES `aggregate_report` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=556964 DEFAULT CHARSET=latin1;

CREATE TABLE `policy_override_reason` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `record_id` int(10) unsigned NOT NULL,
  `policy_override` enum('forwarded','sampled_out','trusted_forwarder','other') DEFAULT NULL,
  `comment` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_record$policy_override_reason_idx` (`id`,`record_id`),
  KEY `fk_record$policy_override_reason_idx1` (`record_id`),
  CONSTRAINT `fk_record$policy_override_reason` FOREIGN KEY (`record_id`) REFERENCES `record` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=23024 DEFAULT CHARSET=latin1;

CREATE TABLE `dkim_auth_result` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `record_id` int(10) unsigned NOT NULL,
  `domain` varchar(256) NOT NULL,
  `dkim_result` enum('none','pass','fail','policy','neutral','temperror','permerror') DEFAULT NULL,
  `human_result` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_record$dkim_auth_result_idx` (`record_id`),
  CONSTRAINT `fk_record$dkim_auth_result` FOREIGN KEY (`record_id`) REFERENCES `record` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=288481 DEFAULT CHARSET=latin1;

CREATE TABLE `spf_auth_result` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `record_id` int(10) unsigned NOT NULL,
  `domain` varchar(256) NOT NULL,
  `spf_result` enum('none','neutral','pass','fail','softfail','temperror','permerror') DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_record$spf_auth_result_idx` (`record_id`),
  CONSTRAINT `fk_record$spf_auth_result` FOREIGN KEY (`record_id`) REFERENCES `record` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=559345 DEFAULT CHARSET=latin1;

CREATE TABLE `forensic_report` (
	`id` bigint unsigned NOT NULL AUTO_INCREMENT,
	`source_ip` char(15) NOT NULL,
	`feedback_type` enum('auth-failure') DEFAULT NULL,
	`header_from` varchar(256) NOT NULL,
	`arrival_date` datetime NOT NULL,
	`envelope_from` varchar(256) DEFAULT NULL,
	`envelope_to` varchar(256) DEFAULT NULL,
	`envelope_subject` varchar(256) DEFAULT NULL,
)

CREATE TABLE `forensic_headers` (
	`report_id` bigint unsigned NOT NULL
    `element_name` varchar(256) NOT NULL
	`element_value` varchar(256) NOT NULL
	CONSTRAINT `fk_forensic_report$forensic_headers` FOREIGN KEY (`report_id`) REFERENCES `forensic_report` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
)

CREATE TABLE `body_report_matches` (
 	`report_id` bigint unsigned NOT NULL
    `body_id` bigint NOT NULL
	CONSTRAINT `fk_forensic_report$body_report_matches` FOREIGN KEY (`report_id`) REFERENCES `forensic_report` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
	CONSTRAINT `fk_forensic_body$body_report_matches` FOREIGN KEY (`body_id`) REFERENCES `forensic_report` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
)
CREATE TABLE `forensic_body` (
 `id` biginnt unsigned NOT NULL
 `sha1` char(40 NOT NULL
 `body` text
 
	CONSTRAINT `fk_forensic_report$forensic_headers` FOREIGN KEY (`report_id`) REFERENCES `forensic_report` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
)

CREATE TABLE `forensic_attachment` (
	`id` bigint unsigned NOT NULL AUTO_INCREMENT,
	`sha1`  
)


	
