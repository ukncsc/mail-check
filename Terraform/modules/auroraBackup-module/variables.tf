variable "env-name" {}

variable "aws-region" {}

variable "backup-account" {
  type    = "string"
  default = ""
}

variable "lambda-memory" {
  type    = "string"
  default = "128"
}

variable "lambda-timeout" {
  type    = "string"
  default = "300"
}

variable "source-or-dest" {
  type        = "string"
  default     = "SOURCE"
  description = "This module should be called twice, once in the source account and once in the destination account setting SOURCE or DEST"
}

variable "ClusterNamePattern" {
  type        = "string"
  default     = "ALL_CLUSTERS"
  description = "Python regex for matching cluster identifiers to backup. Use \"ALL_CLUSTERS\" to back up every Aurora cluster in the region."
}

variable "BackupInterval" {
  type        = "string"
  default     = "24"
  description = "Interval for backups in hours. Default is 24"
}

variable "DestinationAccount" {
  type        = "string"
  description = "Destination account with no dashes."
}

variable "ShareSnapshots" {
  type    = "string"
  default = "TRUE"
}

variable "BackupSchedule" {
  type        = "string"
  default     = "0 1 * * ? *"
  description = "Backup schedule in Cloudwatch Event cron format. Needs to run at least once for every Interval. The default value runs once every at 1AM UTC. More information: http://docs.aws.amazon.com/AmazonCloudWatch/latest/events/ScheduledEvents.html"
}

variable "RetentionDays" {
  type        = "string"
  default     = "7"
  description = "Number of days to keep snapshots in retention before deleting them"
}

variable "LogLevel" {
  type        = "string"
  default     = "ERROR"
  description = "Log level for Lambda functions (DEBUG, INFO, WARN, ERROR, CRITICAL are valid values)."
}

variable "SourceRegionOverride" {
  type        = "string"
  default     = "NO"
  description = "Set to the region where your Aurora clusters run, only if such region does not support Step Functions. Leave as NO otherwise"
}

variable "DeleteOldSnapshots" {
  type        = "string"
  default     = "TRUE"
  description = "Set to TRUE to enable deletion of snapshot based on RetentionDays. Set to FALSE to disable"
}

variable "SnapshotPattern" {
  type        = "string"
  default     = "ALL_SNAPSHOTS"
  description = "Python regex for matching cluster identifiers to backup. Use ALL_SNAPSHOTS to back up every Aurora cluster in the region."
}

variable "DestinationRegion" {
  type        = "string"
  description = "Destination region for snapshots."
}

variable "KmsKeySource" {
  type        = "string"
  default     = "None"
  description = "Set to the ARN for the KMS key in the Source region to decrypt encrypted snapshots. Leave None if you are not using encryption"
}

variable "KmsKeyDestination" {
  type        = "string"
  default     = "None"
  description = "Set to the ARN for the KMS key in the destination region to re-encrypt encrypted snapshots. Leave None if you are not using encryption"
}

variable "lambda-filename-lambdaTakeSnapshotsAurora" {
  type    = "string"
  default = "../../modules/auroraBackup-module/lambda-packages/take_snapshots_aurora.zip"
}

variable "lambda-filename-lambdaShareSnapshotsAurora" {
  type    = "string"
  default = "../../modules/auroraBackup-module/lambda-packages/share_snapshots_aurora.zip"
}

variable "lambda-filename-lambdaDeleteOldSnapshotsAurora" {
  type    = "string"
  default = "../../modules/auroraBackup-module/lambda-packages/delete_old_snapshots_aurora.zip"
}

variable "lambda-filename-lambdaCopySnapshotsAurora" {
  type    = "string"
  default = "../../modules/auroraBackup-module/lambda-packages/copy_snapshots_dest_aurora.zip"
}

variable "lambda-filename-lambdaDeleteOldSnapshotsDestAurora" {
  type    = "string"
  default = "../../modules/auroraBackup-module/lambda-packages/delete_old_snapshots_dest_aurora.zip"
}
