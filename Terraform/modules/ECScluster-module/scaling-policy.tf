resource "aws_autoscaling_policy" "scale-policy" {
  name                   = "TF-${var.env-name}-${var.cluster-name}-scale-policy"
  scaling_adjustment     = 1
  adjustment_type        = "ChangeInCapacity"
  cooldown               = 300
  autoscaling_group_name = "${aws_autoscaling_group.ecs-cluster.name}"
}
