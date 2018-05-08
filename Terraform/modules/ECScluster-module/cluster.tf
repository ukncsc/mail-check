# Create ECS Cluster

resource "aws_ecs_cluster" "microservice-cluster" {
  name = "TF-${var.env-name}-${var.cluster-name}"
}

resource "aws_cloudwatch_log_group" "microservice-cluster-log-group" {
  name = "TF-${var.env-name}-ECS-${var.cluster-name}"
}

