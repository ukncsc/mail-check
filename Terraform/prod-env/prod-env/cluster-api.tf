module "api-cluster" {
  source                    = "../../modules/ECScluster-module"
  cluster-name              = "api"
  vpc-id                    = "${aws_vpc.dmarc-env.id}"
  ssh-key-name              = "${var.ssh-key-name}"
  availability-zones        = "${join(",", values(var.zone-names))}"
  subnet-ids                = "${join(",", aws_subnet.dmarc-env-subnet.*.id)}"
  subnet-cidr               = "${join(",", values(var.zone-subnets))}"
  container-memory          = "${var.default-container-memory}"
  rds-sg-id                 = "${aws_security_group.rds.id}"
  db-access                 = "true"
  health-check-grace-period = "300"
  parameter-store-access    = "true"
  sns-access = "true"
  default-instance-type     = "t2.medium"
  default-instance-count    = "1"
  prod-stage-instance-type  = "t2.medium"
  prod-stage-instance-count = "3"
  lb-sg-count               = "2"
  lb-sg-id                  = ["${module.loadbalancer-internal.lb-sg1-id}", "${module.loadbalancer-internal.lb-sg2-id}"]
  lb-arn                    = "${module.loadbalancer-internal.lb-arn}"
  lb-arn-count              = "1"

  // route53-zone-id           = "${aws_route53_zone.service-zone.id}"
  aws-region                = "${var.aws-region}"
  env-name                  = "${var.env-name}"
  aws-account-id            = "${var.aws-account-id}"
  admin-subnets             = "${var.build-vpc == "" ? "" : var.build-vpc-cidr-block}"
  cloudwatch-alerts-sns-arn = "${aws_sns_topic.cloudwatch-alerts.arn}"
}

resource "aws_security_group_rule" "api-cluster-to-api-lb" {
  type                     = "ingress"
  from_port                = 443
  to_port                  = 443
  protocol                 = "tcp"
  source_security_group_id = "${module.api-cluster.instance-sg}"
  security_group_id        = "${module.loadbalancer-internal.lb-sg1-id}"
}
