data "aws_route53_zone" "service-zone" {
  zone_id      = "${var.route53-zone-id}"
  private_zone = true
  vpc_id       = "${var.vpc-id}"
}

data "aws_acm_certificate" "lb-acm-cert" {
  domain = "${var.certificate-name == "" ? "*.${substr(data.aws_route53_zone.service-zone.name,0,length(data.aws_route53_zone.service-zone.name)-1)}" : "${var.certificate-name}"}"

  statuses = ["ISSUED"]
}
