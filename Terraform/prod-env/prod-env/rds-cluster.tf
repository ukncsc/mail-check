resource "aws_db_subnet_group" "db-subnets" {
  name        = "${var.env-name}-subnets"
  description = "${var.env-name} subnets"
  subnet_ids  = [ "${aws_subnet.dmarc-env-subnet.*.id}" ]
  tags {
    Name = "${var.env-name}-subnets"
  }
}


resource "aws_rds_cluster_instance" "cluster_instances" {
  count              = "${var.db-count}"
  identifier         = "${var.env-name}-db${count.index}"
  cluster_identifier = "${aws_rds_cluster.rds-cluster.id}"
  instance_class     = "${var.db-size}"
  db_subnet_group_name = "${aws_db_subnet_group.db-subnets.name}"
  publicly_accessible  = false
}

resource "aws_rds_cluster" "rds-cluster" {
  cluster_identifier = "${var.env-name}-cluster"
  availability_zones = [ "${values(var.zone-names)}" ]
  database_name      = "${var.db-name}"
  skip_final_snapshot = false
  final_snapshot_identifier = "${var.env-name}-db-final"
  backup_retention_period = "7"
  storage_encrypted  = true
  master_username    = "${var.db-username}"
  master_password    = "${var.db-password}"
  vpc_security_group_ids = [ "${aws_security_group.rds.id}" ]
  db_subnet_group_name = "${aws_db_subnet_group.db-subnets.name}"
}

