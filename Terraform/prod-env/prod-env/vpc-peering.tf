# VPC data source for the build environment
data "aws_vpc" "build-vpc" {
  count = "${var.build-vpc == "" ? 0 :1}"
  id    = "${var.build-vpc}"
}

resource "aws_vpc_peering_connection" "build-peering" {
  count       = "${var.build-vpc == "" ? 0 :1}"
  peer_vpc_id = "${var.build-vpc}"
  vpc_id      = "${aws_vpc.dmarc-env.id}"
  auto_accept = "true"

  accepter {
    allow_remote_vpc_dns_resolution = true
  }

  requester {
    allow_remote_vpc_dns_resolution = true
  }
}

# Add route to this environment routing table
resource "aws_route" "private-peering" {
  count                     = "${var.build-vpc == "" ? 0 :var.zone-count}"
  route_table_id            = "${element(aws_route_table.private.*.id,count.index)}"
  destination_cidr_block    = "${data.aws_vpc.build-vpc.cidr_block}"
  vpc_peering_connection_id = "${aws_vpc_peering_connection.build-peering.id}"
  depends_on                = ["aws_route_table.private"]
}

# Add return route to build VPC routing table
resource "aws_route" "build-route" {
  count                     = "${var.build-vpc == "" ? 0 :1}"
  route_table_id            = "${var.build-route-table}"
  destination_cidr_block    = "${aws_vpc.dmarc-env.cidr_block}"
  vpc_peering_connection_id = "${aws_vpc_peering_connection.build-peering.id}"
}
