resource "aws_sns_topic" "topicBackupsFailed" {
  count = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name  = "TF-${var.env-name}-backups_failed_aurora"
}

resource "aws_sns_topic" "topicShareFailed" {
  count = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name  = "TF-${var.env-name}-share_failed_aurora"
}

resource "aws_sns_topic" "topicDeleteOldFailed" {
  count = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  name  = "TF-${var.env-name}-delete_old_failed_aurora"
}

resource "aws_sns_topic" "topicCopyFailedDest" {
  count = "${var.source-or-dest == "DEST" ? 1 :0}"
  name  = "TF-${var.env-name}-copy_failed_dest_aurora"
}

resource "aws_sns_topic" "topicDeleteOldDestFailed" {
  count = "${var.source-or-dest == "DEST" ? 1 :0}"
  name  = "TF-${var.env-name}-delete_old_failed_dest_aurora"
}
