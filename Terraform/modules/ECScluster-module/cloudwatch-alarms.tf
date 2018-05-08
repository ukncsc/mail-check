resource "aws_cloudwatch_metric_alarm" "cpualarm" {
  alarm_name          = "TF-${var.env-name}-${var.cluster-name}-cpu-alarm"
  comparison_operator = "GreaterThanOrEqualToThreshold"
  evaluation_periods  = "2"
  metric_name         = "CPUUtilization"
  namespace           = "AWS/EC2"
  period              = "120"
  statistic           = "Average"
  threshold           = "70"

  dimensions {
    AutoScalingGroupName = "${aws_autoscaling_group.ecs-cluster.name}"
  }

  alarm_description = "This metric monitors ec2 cpu utilization"

  alarm_actions = ["${aws_autoscaling_policy.scale-policy.arn}",
    "${var.cloudwatch-alerts-sns-arn}",
  ]
}
