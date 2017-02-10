resource "aws_vpc_peering_connection" "build-peering" {
    peer_vpc_id = "${var.build-vpc}"
    vpc_id = "${aws_vpc.dmarc-env.id}"
    auto_accept = "true"
    accepter {
      allow_remote_vpc_dns_resolution = true
    }

    requester {
      allow_remote_vpc_dns_resolution = true
    }
}



resource "aws_route" "build-route" {
  route_table_id = "${var.build-route-table}"
  destination_cidr_block = "${aws_vpc.dmarc-env.cidr_block}"
  vpc_peering_connection_id = "${aws_vpc_peering_connection.build-peering.id}"
}
