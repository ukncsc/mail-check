resource "aws_security_group_rule" "quicksight-to-rds-ingress" {
  type                     = "ingress"
  from_port                = 3306
  to_port                  = 3306
  protocol                 = "tcp"
  source_security_group_id = "${aws_security_group.quicksight.id}"
  security_group_id        = "${aws_security_group.rds.id}"
}

resource "aws_security_group" "quicksight" {
  name_prefix = "TF-${var.env-name}-quicksight-eni"
  description = "Allow quicksight to access RDS"
  vpc_id      = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} Quicksight security group to attach to ENI"
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group_rule" "quicksight-to-rds-egress" {
  type                     = "egress"
  from_port                = 3306
  to_port                  = 3306
  protocol                 = "tcp"
  source_security_group_id = "${aws_security_group.rds.id}"
  security_group_id        = "${aws_security_group.quicksight.id}"
}

resource "aws_security_group_rule" "quicksight-return-traffic" {
  type                     = "ingress"
  from_port                = 0
  to_port                  = 65535
  protocol                 = "tcp"
  source_security_group_id = "${aws_security_group.rds.id}"
  security_group_id        = "${aws_security_group.quicksight.id}"
}
