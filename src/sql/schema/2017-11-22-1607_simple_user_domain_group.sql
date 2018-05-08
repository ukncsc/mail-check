DROP TABLE IF EXISTS `domain_permission`;
DROP TABLE IF EXISTS `tag_attach`;
DROP TABLE IF EXISTS `tag`;
DROP VIEW IF EXISTS domain_per_user_view;


ALTER TABLE `user` ADD COLUMN `global_admin` BIT NOT NULL DEFAULT 0;

-- -----------------------------------------------------
-- Table `dmarc`.`group`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`group` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `dmarc`.`group_domain_mapping`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`group_domain_mapping` (
  `domain_id` INT UNSIGNED NOT NULL,
  `group_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`domain_id`, `group_id`),
  INDEX `group_domain_mapping!group_idx` (`group_id` ASC),
  CONSTRAINT `group_domain_mapping!domain`
    FOREIGN KEY (`domain_id`)
    REFERENCES `dmarc`.`domain` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `group_domain_mapping!group`
    FOREIGN KEY (`group_id`)
    REFERENCES `dmarc`.`group` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `dmarc`.`group_user_mapping`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `dmarc`.`group_user_mapping` (
  `user_id` INT UNSIGNED NOT NULL,
  `group_id` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`user_id`, `group_id`),
  INDEX `group_user_mapping!group_idx` (`group_id` ASC),
  CONSTRAINT `group_user_mapping!user`
    FOREIGN KEY (`user_id`)
    REFERENCES `dmarc`.`user` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `group_user_mapping!group`
    FOREIGN KEY (`group_id`)
    REFERENCES `dmarc`.`group` (`id`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;
