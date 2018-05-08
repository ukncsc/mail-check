resource "aws_autoscaling_group" "ecs-cluster" {
  availability_zones        = ["${split(",", var.availability-zones)}"]
  vpc_zone_identifier       = ["${split(",", var.subnet-ids)}"]
  name                      = "TF-${var.env-name}-${var.cluster-name}"
  min_size                  = "${contains(var.prod-environments, var.env-name) ? var.prod-stage-instance-count -1 : 1}"
  max_size                  = "${contains(var.prod-environments, var.env-name) ? var.prod-stage-instance-count * var.prod-stage-scaling-multiplier : var.default-instance-count }"
  desired_capacity          = "${contains(var.prod-environments, var.env-name) ? var.prod-stage-instance-count : var.default-instance-count }"
  health_check_type         = "EC2"
  launch_configuration      = "${aws_launch_configuration.ecs.name}"
  health_check_grace_period = "${var.health-check-grace-period}"

  lifecycle {
    create_before_destroy = true
  }

  tag {
    key                 = "env"
    value               = "${var.env-name}"
    propagate_at_launch = true
  }

  tag {
    key                 = "service"
    value               = "${var.cluster-name}"
    propagate_at_launch = true
  }

  tag {
    key                 = "Name"
    value               = "TF-ECS-${var.env-name}-${var.cluster-name}"
    propagate_at_launch = true
  }
}
