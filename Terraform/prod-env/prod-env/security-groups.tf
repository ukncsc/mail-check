resource "aws_security_group" "lambda-parser" {
  name_prefix = "TF-${var.env-name}-lambda-parser"
  description = "Allow traffic to/from the lambda function"
  vpc_id      = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} Lambda acl for aggregate reports"
  }

  egress {
    from_port   = 3306
    to_port     = 3306
    protocol    = "tcp"
    cidr_blocks = ["${values(var.zone-subnets)}"]
  }

  egress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group_rule" "parser-to-db" {
  type                     = "ingress"
  from_port                = 3306
  to_port                  = 3306
  protocol                 = "tcp"
  source_security_group_id = "${aws_security_group.lambda-parser.id}"
  security_group_id        = "${aws_security_group.rds.id}"
}

resource "aws_security_group_rule" "dns-parser-to-db" {
  type                     = "ingress"
  from_port                = 3306
  to_port                  = 3306
  protocol                 = "tcp"
  source_security_group_id = "${aws_security_group.lambda-domaininformation-processor.id}"
  security_group_id        = "${aws_security_group.rds.id}"
}

resource "aws_security_group" "lambda-domaininformation-processor" {
  name_prefix = "TF-${var.env-name}-lambda-domaininformation-processor"
  description = "Allow traffic to/from the lambda function"
  vpc_id      = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} Lambda acl for domaininformation processor"
  }

  egress {
    from_port   = 3306
    to_port     = 3306
    protocol    = "tcp"
    cidr_blocks = ["${values(var.zone-subnets)}"]
  }

  egress {
    from_port   = 53
    to_port     = 53
    protocol    = "udp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 53
    to_port     = 53
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 25
    to_port     = 25
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group" "rds" {
  name_prefix = "TF-${var.env-name}-rds-in"
  description = "Allow traffic to the database"
  vpc_id      = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} RDS acl"
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_security_group_rule" "rds-to-s3" {
  type              = "egress"
  from_port         = 443
  to_port           = 443
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
  security_group_id = "${aws_security_group.rds.id}"
}




resource "aws_security_group_rule" "build-to-rds" {
  type              = "ingress"
  from_port         = 3306
  to_port           = 3306
  protocol          = "tcp"
  cidr_blocks       = ["${var.build-vpc-cidr-block}"]
  security_group_id = "${aws_security_group.rds.id}"
}
