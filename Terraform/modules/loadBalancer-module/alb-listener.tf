resource "aws_lb_listener" "443" {
  count             = "${var.lb-type == "application"? 1: 0}"
  load_balancer_arn = "${aws_lb.lb.arn}"
  port              = "443"
  protocol          = "HTTPS"
  ssl_policy        = "ELBSecurityPolicy-TLS-1-2-2017-01"
  certificate_arn   = "${data.aws_acm_certificate.lb-acm-cert.arn}"

  default_action {
    target_group_arn = "${element(aws_lb_target_group.lb-target.*.arn,0)}"
    type             = "forward"
  }
}

resource "aws_lb_listener" "80" {
  count             = "${var.http-listener == "true"? 1: 0}"
  load_balancer_arn = "${aws_lb.lb.arn}"
  port              = "80"
  protocol          = "HTTP"

  default_action {
    target_group_arn = "${element(aws_lb_target_group.lb-target.*.arn,0)}"
    type             = "forward"
  }
}
