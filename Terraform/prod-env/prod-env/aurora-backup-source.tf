module "aurora-backup" {
  source             = "../../modules/auroraBackup-module"
  env-name           = "${var.env-name}"
  aws-region         = "${var.aws-region}"
  backup-account     = "${var.backup-account}"
  source-or-dest     = "SOURCE"
  ClusterNamePattern = "${var.env-name}"
  BackupInterval     = "24"
  DestinationAccount = "${var.backup-account}"
  DestinationRegion  = "eu-west-2"
  KmsKeySource       = "${var.KmsKeySource}"
}
