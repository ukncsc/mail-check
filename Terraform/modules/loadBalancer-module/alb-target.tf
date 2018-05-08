resource "aws_lb_target_group" "lb-target" {
  count    = "${var.lb-type == "application"? var.lb-target-count: 0}"
  name     = "TF-${var.env-name}-${var.balancer-name}-${element(split(",",var.lb-target-paths),count.index)}"
  port     = 80
  protocol = "HTTP"
  vpc_id   = "${var.vpc-id}"

  health_check {
    interval = 30
    path     = "${var.healthcheck-path}"
    protocol = "${var.healthcheck-protocol}"
    matcher  = "200"
  }
}
