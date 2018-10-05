module "spf" {
  source                 = "git@github.com:ukncsc/MailCheck.Spf//infrastructure/modules/spf-microservice?ref=27ce83c"
  artefact-version       = "27ce83c"
  vpc-id                 = "${aws_vpc.dmarc-env.id}"
  aws-account-id         = "${var.aws-account-id}"
  aws-region             = "${var.aws-region}"
  subnet-ids             = ["${aws_subnet.dmarc-env-subnet.*.id}"]
  subnet-cidr            = "${join(",", values(var.zone-subnets))}"
  env-name               = "${var.env-name}"
  db-password            = "${var.db-password}"
  db-name                = "spf"
  db-replica-count       = "${var.db-replica-count}"
  db-snapshot-to-restore = "${var.db-microservice-spf-snapshot-to-restore}"
  db-master-size         = "db.t2.small"
  db-replica-size        = "db.t2.small"
  db-kms-key-id          = "${var.db-kms-key-id}"
  db-username            = "${var.db-username}"

  # api ecs cluster
  api-ecs-cluster-id       = "${module.api-cluster.cluster-id}"
  api-ecs-service-role-arn = "${module.api-cluster.ecs-service-role-arn}"

  # processor ecs cluster
  processor-ecs-cluster-id = "${module.processor-cluster.cluster-id}"

  # Assign to a load balancer
  target-group-arn  = "${element(module.loadbalancer-internal.target-group-arns, index(module.loadbalancer-internal.target-group-paths, "spf"))}"
  load-balancer-arn = "${module.loadbalancer-internal.lb-arn}"

  # Subscribe to topics
  input-queue-subscriptions      = "${module.adminapi.sns-arn},${aws_sns_topic.selector-seen-in-aggregate-report.arn}"
  input-queue-subscription-count = "2"

  # SNS topic for monitoring alerts
  alerts-topic-arn = "${aws_sns_topic.cloudwatch-alerts.arn}"
}

resource "aws_security_group_rule" "api-cluster-access-to-spf-db" {
  type                     = "ingress"
  from_port                = "3306"
  to_port                  = "3306"
  protocol                 = "tcp"
  source_security_group_id = "${module.api-cluster.instance-sg}"
  security_group_id        = "${module.spf.db-security-group-id}"
}

resource "aws_security_group_rule" "processor-cluster-access-to-spf-db" {
  type                     = "ingress"
  from_port                = "3306"
  to_port                  = "3306"
  protocol                 = "tcp"
  source_security_group_id = "${module.processor-cluster.instance-sg}"
  security_group_id        = "${module.spf.db-security-group-id}"
}

resource "aws_security_group_rule" "build-access-to-spf-db" {
  type              = "ingress"
  from_port         = "3306"
  to_port           = "3306"
  protocol          = "tcp"
  cidr_blocks       = ["${var.build-vpc-cidr-block}"]
  security_group_id = "${module.spf.db-security-group-id}"
}
