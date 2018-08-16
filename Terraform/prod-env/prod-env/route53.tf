data "aws_route53_zone" "dmarc-zone" {
  name = "${var.parent-zone}"
}

resource "aws_route53_record" "db" {
  zone_id = "${data.aws_route53_zone.dmarc-zone.zone_id}"
  name    = "dmdbrwep"
  type    = "CNAME"
  ttl     = "600"
  records = ["${aws_rds_cluster.rds-cluster.endpoint}"]
}

resource "aws_route53_record" "gateway" {
  count   = "${var.natgw-count}"
  zone_id = "${data.aws_route53_zone.dmarc-zone.zone_id}"
  name    = "gateway${count.index+1}"
  type    = "A"
  ttl     = "600"
  records = ["${element(aws_nat_gateway.natgw.*.public_ip,count.index)}"]
}

resource "aws_route53_record" "mx" {
  zone_id = "${data.aws_route53_zone.dmarc-zone.zone_id}"
  name    = ""
  type    = "MX"
  ttl     = "600"
  records = ["5 inbound-smtp.eu-west-1.amazonaws.com.","10 gateway${count.index+1}"]
}


resource "aws_route53_record" "spf" {
  zone_id = "${data.aws_route53_zone.dmarc-zone.zone_id}"
  name    = ""
  type    = "TXT"
  ttl     = "600"
  records = ["v=spf1 mx -all"]
}

resource "aws_route53_zone" "service-zone" {
  name          = "${var.env-name}.i.mailcheck.service.ncsc.gov.uk"
  vpc_id        = "${aws_vpc.dmarc-env.id}"
  force_destroy = true
}
