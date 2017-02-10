data "aws_route53_zone" "dmarc-report-zone2" {
  name = "${var.report-2nd-receiving-domain}"
}

resource "aws_route53_record" "dmarc-report-domain-record2" {
  count = "${length(split(",",var.reporting-domains))}"
  zone_id = "${data.aws_route53_zone.dmarc-report-zone2.zone_id}"
  name = "${trimspace(element(split(",", var.reporting-domains),count.index))}._report._dmarc"
  type = "TXT"
  ttl = "36000"
  records = ["v=DMARC1"]
}

resource "aws_route53_record" "dmarc-report-subdomain-record2" {
  count = "${length(split(",",var.reporting-domains))}"
  zone_id = "${data.aws_route53_zone.dmarc-report-zone2.zone_id}"
  name = "*.${trimspace(element(split(",", var.reporting-domains),count.index))}._report._dmarc"
  type = "TXT"
  ttl = "36000"
  records = ["v=DMARC1"]
}


