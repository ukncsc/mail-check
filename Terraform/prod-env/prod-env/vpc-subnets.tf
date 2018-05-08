# CREATE VPC

resource "aws_vpc" "dmarc-env" {
  cidr_block           = "${var.vpc-cidr-block}"
  enable_dns_support   = true
  enable_dns_hostnames = true

  tags {
    Name = "TF-${var.env-name} App VPC"
  }
}

# CREATE GATEWAY AND DEFAULT ROUTE

resource "aws_internet_gateway" "dmarc-env" {
  vpc_id = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "TF-${var.env-name} Gateway"
  }
}

resource "aws_route" "internet_access" {
  route_table_id         = "${aws_vpc.dmarc-env.main_route_table_id}"
  destination_cidr_block = "0.0.0.0/0"
  gateway_id             = "${aws_internet_gateway.dmarc-env.id}"
}

# Create public subnet in each AZ

resource "aws_subnet" "dmarc-env-public-subnet" {
  count                   = "${var.public-zone-count}"
  vpc_id                  = "${aws_vpc.dmarc-env.id}"
  availability_zone       = "${lookup(var.public-zone-names, format("zone%d", count.index))}"
  cidr_block              = "${lookup(var.public-zone-subnets, format("zone%d", count.index))}"
  map_public_ip_on_launch = true

  tags {
    Name = "TF-${var.env-name} Public -AZ: ${lookup(var.zone-names, format("zone%d", count.index))}"
  }
}

resource "aws_route_table_association" "public-rt-assoc" {
  count          = "${var.public-zone-count}"
  subnet_id      = "${element(aws_subnet.dmarc-env-public-subnet.*.id,count.index)}"
  route_table_id = "${aws_vpc.dmarc-env.main_route_table_id}"
}

# CREATE NAT GATEWAY IN EACH AZ

resource "aws_eip" "nat" {
  count = "${var.natgw-count}"
  vpc   = true
}

resource "aws_nat_gateway" "natgw" {
  count         = "${var.natgw-count}"
  allocation_id = "${element(aws_eip.nat.*.id,count.index)}"
  subnet_id     = "${element(aws_subnet.dmarc-env-public-subnet.*.id,count.index)}"
  depends_on    = ["aws_internet_gateway.dmarc-env"]
}

# Set DHCP options for service lookup

resource "aws_vpc_dhcp_options" "vpc-options" {
  domain_name         = "${aws_route53_zone.service-zone.name}"
  domain_name_servers = ["AmazonProvidedDNS"]
}

# Attach the DHCP options to the VPC

resource "aws_vpc_dhcp_options_association" "vpc-options-attach" {
  vpc_id          = "${aws_vpc.dmarc-env.id}"
  dhcp_options_id = "${aws_vpc_dhcp_options.vpc-options.id}"
}

# CREATE PRIVATE SUBNET IN EACH AZ FOR DMARC APP

resource "aws_subnet" "dmarc-env-subnet" {
  count                   = "${var.zone-count}"
  vpc_id                  = "${aws_vpc.dmarc-env.id}"
  availability_zone       = "${lookup(var.zone-names, format("zone%d", count.index))}"
  cidr_block              = "${lookup(var.zone-subnets, format("zone%d", count.index))}"
  map_public_ip_on_launch = false

  tags {
    Name = "TF-${var.env-name} - AZ: ${lookup(var.zone-names, format("zone%d", count.index))}"
  }
}

# CREATE PRIVATE SUBNET IN EACH AZ FOR AUTH PROXY

resource "aws_subnet" "frontend-subnet" {
  count                   = "${var.zone-count}"
  vpc_id                  = "${aws_vpc.dmarc-env.id}"
  availability_zone       = "${lookup(var.frontend-zone-names, format("zone%d", count.index))}"
  cidr_block              = "${lookup(var.frontend-zone-subnets, format("zone%d", count.index))}"
  map_public_ip_on_launch = false

  tags {
    Name = "TF-${var.env-name} Frontend - AZ: ${lookup(var.frontend-zone-names, format("zone%d", count.index))}"
  }
}

resource "aws_route_table" "frontend" {
  count  = "${var.zone-count}"
  vpc_id = "${aws_vpc.dmarc-env.id}"

  route {
    cidr_block     = "0.0.0.0/0"
    nat_gateway_id = "${element(aws_nat_gateway.natgw.*.id,count.index)}"
  }

  tags {
    Name = "${var.env-name} ${lookup(var.zone-names, format("zone%d", count.index))} Frontend NAT GW"
  }
}

resource "aws_route_table_association" "frontend-assoc" {
  count          = "${var.zone-count}"
  subnet_id      = "${element(aws_subnet.frontend-subnet.*.id,count.index)}"
  route_table_id = "${element(aws_route_table.frontend.*.id,count.index)}"
}

# Routing table for private subnet, uses build vpc data source for vpc peering.

resource "aws_route_table" "private" {
  count  = "${var.zone-count}"
  vpc_id = "${aws_vpc.dmarc-env.id}"

  tags {
    Name = "${var.env-name} ${lookup(var.zone-names, format("zone%d", count.index))} Private NAT GW"
  }
}

resource "aws_route" "private-default" {
  count                  = "${var.zone-count}"
  route_table_id         = "${element(aws_route_table.private.*.id,count.index)}"
  destination_cidr_block = "0.0.0.0/0"
  nat_gateway_id         = "${element(aws_nat_gateway.natgw.*.id,count.index)}"
  depends_on             = ["aws_route_table.private"]
}

resource "aws_route_table_association" "private-rt-assoc" {
  count          = "${var.zone-count}"
  subnet_id      = "${element(aws_subnet.dmarc-env-subnet.*.id,count.index)}"
  route_table_id = "${element(aws_route_table.private.*.id,count.index)}"
}
