resource "aws_security_group" "lb-http-sg1" {
  name_prefix = "TF-${var.env-name}-${var.balancer-name}-http-elb1"
  description = "Allow http traffic to the elb"
  vpc_id      = "${var.vpc-id}"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group" "lb-http-sg2" {
  name_prefix = "TF-${var.env-name}-${var.balancer-name}-http-elb2"
  description = "Allow http traffic to the elb"
  vpc_id      = "${var.vpc-id}"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group" "lb-https-sg1" {
  name_prefix = "TF-${var.env-name}-${var.balancer-name}-https-elb1"
  description = "Allow http traffic to the elb"
  vpc_id      = "${var.vpc-id}"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group" "lb-https-sg2" {
  name_prefix = "TF-${var.env-name}-${var.balancer-name}-https-elb2"
  description = "Allow http traffic to the elb"
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
  security_group_id = "${aws_security_group.lb-http-sg1.id}"
}

resource "aws_security_group_rule" "elb-to-instances-sg1" {
  type              = "egress"
  from_port         = "0"
  to_port           = "65535"
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.instance-subnet-cidr)}"]
  security_group_id = "${aws_security_group.lb-http-sg1.id}"
}

resource "aws_security_group_rule" "http-to-lb-sg1" {
  count             = "${var.lb-ingress-subnets1 != "" && var.http-listener == "true" ? 1 : 0}"
  type              = "ingress"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.lb-ingress-subnets1)}"]
  security_group_id = "${aws_security_group.lb-http-sg1.id}"
}

resource "aws_security_group_rule" "http-to-lb-sg2" {
  count             = "${var.lb-ingress-subnets2 != ""  && var.http-listener == "true" ? 1 : 0}"
  type              = "ingress"
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.lb-ingress-subnets2)}"]
  security_group_id = "${aws_security_group.lb-http-sg2.id}"
}

resource "aws_security_group_rule" "https-to-lb-sg1" {
  count             = "${var.lb-ingress-subnets1 == ""  ? 0 : 1}"
  type              = "ingress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.lb-ingress-subnets1)}"]
  security_group_id = "${aws_security_group.lb-https-sg1.id}"
}

resource "aws_security_group_rule" "https-to-lb-sg2" {
  count             = "${var.lb-ingress-subnets2 == ""  ? 0 : 1}"
  type              = "ingress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.lb-ingress-subnets2)}"]
  security_group_id = "${aws_security_group.lb-https-sg2.id}"
}

resource "aws_security_group_rule" "admin-https-to-lb" {
  count             = "${var.admin-subnets == ""  ? 0 : 1}"
  type              = "ingress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = ["${split(",", var.admin-subnets)}"]
  security_group_id = "${aws_security_group.lb-https-sg1.id}"
}
