resource "aws_route53_record" "service-elb" {
  zone_id = "${var.route53-zone-id}"
  name    = "${var.balancer-fqdn}"
  type    = "CNAME"
  ttl     = "300"
  records = ["${aws_lb.lb.dns_name}"]
}

resource "aws_route53_record" "service-elb1" {
  zone_id = "${var.route53-zone-id}"
  name    = "lb1-${var.balancer-fqdn}"
  type    = "CNAME"
  ttl     = "300"
  records = ["${aws_lb.lb.dns_name}"]
}

resource "aws_route53_record" "service-elb2" {
  zone_id = "${var.route53-zone-id}"
  name    = "lb2-${var.balancer-fqdn}"
  type    = "CNAME"
  ttl     = "300"
  records = ["${aws_lb.lb.dns_name}"]
}
