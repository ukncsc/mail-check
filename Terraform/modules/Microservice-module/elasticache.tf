resource "aws_elasticache_subnet_group" "elasticache-subnet-group" {
  count      = "${var.cache-node-type == "" ? 0 : 1}"
  name       = "TF-${var.env-name}-${var.service-name}-cache-subnet"
  subnet_ids = ["${split(",", var.subnet-ids)}"]
}

resource "aws_elasticache_replication_group" "cache-rg" {
  count                         = "${var.cache-node-type == "" ? 0 : 1}"
  replication_group_id          = "${substr("${var.env-name}-${var.service-name}",0,length("${var.env-name}-${var.service-name}") >20 ? 20: -1)}"
  count                         = "${var.cache-node-type == "" ? 0 : 1}"
  replication_group_description = "${var.env-name}-${var.service-name} replication group"
  node_type                     = "${var.cache-node-type}"
  engine_version                = "3.2.4"
  number_cache_clusters         = "${var.cache-node-failover == "true" ? 2 : 1}"
  port                          = 6379
  parameter_group_name          = "default.redis3.2"
  subnet_group_name             = "${aws_elasticache_subnet_group.elasticache-subnet-group.name}"
  automatic_failover_enabled    = "${var.cache-node-failover}"
  security_group_ids            = ["${aws_security_group.redis-cache.id}"]
}

resource "aws_security_group" "redis-cache" {
  name_prefix = "TF-${var.env-name}-${var.service-name}-cache"
  description = "Allow traffic to the cache"
  vpc_id      = "${var.vpc-id}"

  tags {
    Name = "TF-${var.env-name}-${var.service-name}-cache"
  }

  ingress {
    from_port   = 6379
    to_port     = 6379
    protocol    = "tcp"
    cidr_blocks = ["${split(",", var.subnet-cidr)}"]
  }

  lifecycle {
    create_before_destroy = true
  }
}