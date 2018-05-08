resource "aws_lb_listener_rule" "static" {
  count        = "${var.lb-type == "application" && var.lb-target-count >1 ? var.lb-target-count: 0}"
  listener_arn = "${aws_lb_listener.443.arn}"
  priority     = "${(count.index+1)*10}"

  action {
    type             = "forward"
    target_group_arn = "${element(aws_lb_target_group.lb-target.*.arn,count.index)}"
  }

  condition {
    field  = "path-pattern"
    values = ["${var.lb-target-path-prefix}/${element(split(",",var.lb-target-paths),count.index)}/*"]
  }
}
