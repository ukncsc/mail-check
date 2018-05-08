resource "aws_security_group_rule" "access-to-db" {
  count                    = "${var.db-access == "true" ? 1 : 0}"
  type                     = "ingress"
  from_port                = "${var.db-port}"
  to_port                  = "${var.db-port}"
  protocol                 = "tcp"
  source_security_group_id = "${aws_security_group.service-instance.id}"
  security_group_id        = "${var.rds-sg-id}"
}

resource "aws_security_group" "service-instance" {
  name_prefix = "TF-${var.env-name}-${var.cluster-name}-instance"
  description = "Allow traffic to and from the instance"
  vpc_id      = "${var.vpc-id}"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group_rule" "elb-packet-too-big" {
  type              = "egress"
  from_port         = "3"
  to_port           = "4"
  protocol          = "icmp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-custom-internet" {
  count             = "${var.egress-internet-custom-ports-count}"
  type              = "egress"
  from_port         = "${element(split(",",var.egress-internet-custom-ports),count.index)}"
  to_port           = "${element(split(",",var.egress-internet-custom-ports),count.index)}"
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-custom1" {
  count             = "${var.egress-custom-port1 == "" ? 0 : 1}"
  type              = "egress"
  from_port         = "${var.egress-custom-port1}"
  to_port           = "${var.egress-custom-port1}"
  protocol          = "tcp"
  cidr_blocks       = ["${var.egress-custom-destination1}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-custom2" {
  count             = "${var.egress-custom-port2 == "" ? 0 : 1}"
  type              = "egress"
  from_port         = "${var.egress-custom-port2}"
  to_port           = "${var.egress-custom-port2}"
  protocol          = "tcp"
  cidr_blocks       = ["${var.egress-custom-destination2}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-custom3" {
  count             = "${var.egress-custom-port3 == "" ? 0 : 1}"
  type              = "egress"
  from_port         = "${var.egress-custom-port3}"
  to_port           = "${var.egress-custom-port3}"
  protocol          = "tcp"
  cidr_blocks       = ["${var.egress-custom-destination3}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-custom4" {
  count             = "${var.egress-custom-port4 == "" ? 0 : 1}"
  type              = "egress"
  from_port         = "${var.egress-custom-port4}"
  to_port           = "${var.egress-custom-port4}"
  protocol          = "tcp"
  cidr_blocks       = ["${var.egress-custom-destination4}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-custom5" {
  count             = "${var.egress-custom-port5 == "" ? 0 : 1}"
  type              = "egress"
  from_port         = "${var.egress-custom-port5}"
  to_port           = "${var.egress-custom-port5}"
  protocol          = "tcp"
  cidr_blocks       = ["${var.egress-custom-destination5}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-to-postgres-db" {
  count             = "${var.db-access == "true" ? 1 : 0}"
  type              = "egress"
  from_port         = 5432
  to_port           = 5432
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.subnet-cidr)}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-to-mysql-db" {
  count             = "${var.db-access == "true" ? 1 : 0}"
  type              = "egress"
  from_port         = 3306
  to_port           = 3306
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.subnet-cidr)}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-to-https" {
  type              = "egress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-to-http" {
  type              = "egress"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-instance-egress" {
  count             = "${var.allow-inter-instance-traffic == "true" ? var.cluster-ingress-ports-count : 0}"
  type              = "egress"
  from_port         = "${element(split(",",var.cluster-ingress-ports),count.index)}"
  to_port           = "${element(split(",",var.cluster-ingress-ports),count.index)}"
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.subnet-cidr)}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-instance-ingress" {
  count             = "${var.allow-inter-instance-traffic == "true" ? var.cluster-ingress-ports-count : 0}"
  type              = "ingress"
  from_port         = "${element(split(",",var.cluster-ingress-ports),count.index)}"
  to_port           = "${element(split(",",var.cluster-ingress-ports),count.index)}"
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.subnet-cidr)}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "instance-egress-to-redis" {
  type              = "egress"
  from_port         = 6379
  to_port           = 6379
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.subnet-cidr)}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "ssh-to-instance" {
  count             = "${var.admin-subnets == ""? 0: 1}"
  type              = "ingress"
  from_port         = 22
  to_port           = 22
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.admin-subnets)}"]
  security_group_id = "${aws_security_group.service-instance.id}"
}

resource "aws_security_group_rule" "elb-to-instance" {
  count                    = "${var.lb-sg-count}"
  type                     = "ingress"
  from_port                = 0
  to_port                  = 65535
  protocol                 = "tcp"
  source_security_group_id = "${element(var.lb-sg-id,count.index)}"
  security_group_id        = "${aws_security_group.service-instance.id}"
}
