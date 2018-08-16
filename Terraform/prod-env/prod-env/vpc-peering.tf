resource "aws_vpc_peering_connection" "build-peering" {
  peer_owner_id = "${var.build-account-id}"
  count         = "${var.build-vpc == "" ? 0 :1}"
  peer_vpc_id   = "${var.build-vpc}"
  vpc_id        = "${aws_vpc.dmarc-env.id}"

  // auto_accept   = "true"

  requester {
    allow_remote_vpc_dns_resolution = true
  }
  tags {
    Name = "TF-${var.env-name}-${aws_vpc.dmarc-env.cidr_block}"
  }
}

# Add route to this environment routing table
resource "aws_route" "private-peering" {
  count                     = "${var.build-vpc == "" ? 0 :var.zone-count}"
  route_table_id            = "${element(aws_route_table.private.*.id,count.index)}"
  destination_cidr_block    = "${var.build-vpc-cidr-block}"
  vpc_peering_connection_id = "${aws_vpc_peering_connection.build-peering.id}"
  depends_on                = ["aws_route_table.private"]
}

resource "aws_route" "frontend-peering" {
  count                     = "${var.build-vpc == "" ? 0 :var.zone-count}"
  route_table_id            = "${element(aws_route_table.frontend.*.id,count.index)}"
  destination_cidr_block    = "${var.build-vpc-cidr-block}"
  vpc_peering_connection_id = "${aws_vpc_peering_connection.build-peering.id}"
  depends_on                = ["aws_route_table.frontend"]
}

# Add return route to build VPC routing table
#resource "aws_route" "build-route" {
#  count                     = "${var.build-vpc == "" ? 0 :1}"
#  route_table_id            = "${var.build-route-table}"
#  destination_cidr_block    = "${aws_vpc.dmarc-env.cidr_block}"
#  vpc_peering_connection_id = "${aws_vpc_peering_connection.build-peering.id}"
#}

