module "reports" {
  source                   = "git@github.com:ukncsc/MailCheck.Reports//infrastructure/modules/reports-microservice?ref=4e34140"
  artefact-version         = "4e34140"
  vpc-id                   = "${aws_vpc.dmarc-env.id}"
  aws-account-id           = "${var.aws-account-id}"
  aws-region               = "${var.aws-region}"
  subnet-ids               = ["${aws_subnet.dmarc-env-subnet.*.id}"]
  subnet-cidr              = "${join(",", values(var.zone-subnets))}"
  env-name                 = "${var.env-name}"
  report-output-s3-bucket  = "${aws_s3_bucket.db-export-bucket.id}"
  main-db-user-name        = "${var.env-name}_${lookup(var.db-users,"reports")}"
  main-db-name             = "${var.db-name}"
  main-db-cluster-endpoint = "${aws_rds_cluster.rds-cluster.endpoint}"

  # SNS topic for monitoring alerts
  alerts-topic-arn = "${aws_sns_topic.cloudwatch-alerts.arn}"

  db-security-group-id = "${aws_security_group.rds.id}"
}
