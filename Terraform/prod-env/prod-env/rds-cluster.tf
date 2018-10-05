resource "aws_db_subnet_group" "db-subnets" {
  name        = "${var.env-name}-subnets"
  description = "${var.env-name} subnets"
  subnet_ids  = ["${aws_subnet.dmarc-env-subnet.*.id}"]

  tags {
    Name = "${var.env-name}-subnets"
  }
}

resource "aws_rds_cluster_instance" "cluster_master" {
  identifier              = "${var.env-name}-db"
  cluster_identifier      = "${aws_rds_cluster.rds-cluster.id}"
  engine                  = "aurora-mysql"
  engine_version          = "5.7.12"
  instance_class          = "${var.db-master-size}"
  db_subnet_group_name    = "${aws_db_subnet_group.db-subnets.name}"
  db_parameter_group_name = "${aws_db_parameter_group.dmarc-rds-pg.name}"
  publicly_accessible     = false
}

resource "aws_rds_cluster_instance" "cluster_replicas" {
  count                   = "${var.db-replica-count}"
  promotion_tier          = 2
  identifier              = "${var.env-name}-db-replica${count.index}"
  cluster_identifier      = "${aws_rds_cluster.rds-cluster.id}"
  engine                  = "aurora-mysql"
  engine_version          = "5.7.12"
  instance_class          = "${var.db-replica-size}"
  db_subnet_group_name    = "${aws_db_subnet_group.db-subnets.name}"
  db_parameter_group_name = "${aws_db_parameter_group.dmarc-rds-pg.name}"
  publicly_accessible     = false
}

resource "aws_db_parameter_group" "dmarc-rds-pg" {
  name   = "mailcheck-pg-${var.env-name}"
  family = "aurora-mysql5.7"

  parameter {
    name  = "event_scheduler"
    value = "ON"
  }

  parameter {
    name = "max_connections"
    value = "1000"
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_rds_cluster_parameter_group" "dmarc-rdscluster-pg" {
  name        = "mailcheck-cluster-pg-${var.env-name}"
  family      = "aurora-mysql5.7"
  description = "Mailcheck Aurora cluster parameter group"

  parameter {
    name  = "aws_default_s3_role"
    value = "${aws_iam_role.rds-s3-export.arn}"
  }
}

resource "aws_rds_cluster" "rds-cluster" {
  cluster_identifier = "${var.env-name}-cluster"
  engine             = "aurora-mysql"
  engine_version     = "5.7.12"

  #availability_zones        = ["${values(var.zone-names)}"] removing due to bug #3754
  database_name                   = "${var.db-name}"
  skip_final_snapshot             = false
  final_snapshot_identifier       = "${var.env-name}-db-final"
  snapshot_identifier             = "${var.db-snapshot-to-restore}"
  backup_retention_period         = "7"
  storage_encrypted               = true
  kms_key_id                      = "${var.db-kms-key-id}"
  master_username                 = "${var.db-username}"
  master_password                 = "${var.db-password}"
  iam_roles                       = ["${aws_iam_role.rds-s3-export.arn}"]
  db_cluster_parameter_group_name = "${aws_rds_cluster_parameter_group.dmarc-rdscluster-pg.name}"
  vpc_security_group_ids          = ["${aws_security_group.rds.id}"]

  db_subnet_group_name = "${aws_db_subnet_group.db-subnets.name}"

  lifecycle {
    prevent_destroy = true
  }
}
