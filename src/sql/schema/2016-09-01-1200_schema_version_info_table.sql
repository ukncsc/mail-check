-- -----------------------------------------------------
-- Table `dmarc`.`VersionInfo`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`VersionInfo` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Version` BIGINT(20) NOT NULL,
  `AppliedOn` DATETIME NOT NULL,
  `Description` TEXT NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = latin1;

SHOW WARNINGS;
