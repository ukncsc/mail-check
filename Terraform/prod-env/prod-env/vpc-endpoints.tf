resource "aws_vpc_endpoint" "private-s3" {
    vpc_id = "${aws_vpc.dmarc-env.id}"
    service_name = "com.amazonaws.${var.aws-region}.s3"
}

resource "aws_vpc_endpoint_route_table_association" "private_s3" {
    count                   = "${var.zone-count}"
    vpc_endpoint_id = "${aws_vpc_endpoint.private-s3.id}"
    route_table_id = "${element(aws_route_table.private.*.id,count.index)}"
}

