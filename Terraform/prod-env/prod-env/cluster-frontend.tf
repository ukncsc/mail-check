module "frontend-cluster" {
  source             = "../../modules/ECScluster-module"
  vpc-id             = "${aws_vpc.dmarc-env.id}"
  cluster-name       = "frontend"
  ssh-key-name       = "${var.ssh-key-name}"
  availability-zones = "${join(",", values(var.frontend-zone-names))}"
  subnet-ids         = "${join(",", aws_subnet.frontend-subnet.*.id)}"
  subnet-cidr        = "${join(",", values(var.frontend-zone-subnets))}"
  container-memory   = "${var.default-container-memory}"
  rds-sg-id          = "${aws_security_group.rds.id}"

  default-instance-type     = "t2.small"
  default-instance-count    = "1"
  prod-stage-instance-type  = "t2.medium"
  prod-stage-instance-count = "3"
  lb-sg-count               = "2"
  lb-sg-id                  = ["${module.loadbalancer-external.lb-sg1-id}", "${module.loadbalancer-external.lb-sg2-id}"]
  lb-arn                    = "${module.loadbalancer-external.lb-arn}"
  lb-arn-count              = "1"

  // route53-zone-id           = "${aws_route53_zone.service-zone.id}"
  health-check-grace-period = "300"
  parameter-store-access    = "false"
  aws-region                = "${var.aws-region}"
  env-name                  = "${var.env-name}"
  aws-account-id            = "${var.aws-account-id}"
  admin-subnets             = "${var.build-vpc == "" ? "" : var.build-vpc-cidr-block}"
  cloudwatch-alerts-sns-arn = "${aws_sns_topic.cloudwatch-alerts.arn}"
}

resource "aws_security_group_rule" "frontend-to-api-lb" {
  type                     = "ingress"
  from_port                = 443
  to_port                  = 443
  protocol                 = "tcp"
  source_security_group_id = "${module.frontend-cluster.instance-sg}"
  security_group_id        = "${module.loadbalancer-internal.lb-sg1-id}"
}
