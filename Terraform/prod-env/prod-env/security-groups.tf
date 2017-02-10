resource "aws_security_group" "lambda-aggregate" {
  name        = "TF-lambda-aggregate"
  description = "Allow traffic to/from the lambda function"
  vpc_id      = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} Lambda acl for aggregate reports"
  }
  egress {
    from_port   = 3306
    to_port     = 3306
    protocol    = "tcp"
    cidr_blocks = [ "${values(var.zone-subnets)}" ]
  }
}

resource "aws_security_group" "lambda-parser" {
  name        = "TF-lambda-parser"
  description = "Allow traffic to/from the lambda function"
  vpc_id      = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} Lambda acl for aggregate reports"
  }
  egress {
    from_port   = 3306
    to_port     = 3306
    protocol    = "tcp"
    cidr_blocks = [ "${values(var.zone-subnets)}" ]
  }
  egress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = [ "0.0.0.0/0" ]
  }
}

resource "aws_security_group" "rds" {
  name        = "TF-rds-in"
  description = "Allow traffic to the database"
  vpc_id      = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} RDS acl"
  }
  ingress {
    from_port   = 3306
    to_port     = 3306
    protocol    = "tcp"
    security_groups = [ "${aws_security_group.lambda-aggregate.id}", "${aws_security_group.bastion.id}" , "${aws_security_group.lambda-parser.id}"]
  }
}


resource "aws_security_group" "bastion" {
  name        = "TF-bastion-sg"
  description = "Allow traffic to/from the bastion host"
  vpc_id      = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} bastion acl"
  }
  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = [ "${data.aws_vpc.build-vpc.cidr_block}" ]

  }
  egress {
    from_port   = 3306
    to_port     = 3306
    protocol    = "tcp"
    cidr_blocks = [ "${values(var.zone-subnets)}" ]

  }

}

