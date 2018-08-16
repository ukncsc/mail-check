module "spfrecordevaluator" {
  source = "../../modules/Microservice-module"

  # Project settings
  vpc-id             = "${aws_vpc.dmarc-env.id}"
  aws-region         = "${var.aws-region}"
  env-name           = "${var.env-name}"
  aws-account-id     = "${var.aws-account-id}"
  availability-zones = "${join(",", values(var.zone-names))}"
  subnet-ids         = "${join(",", aws_subnet.dmarc-env-subnet.*.id)}"
  subnet-cidr        = "${join(",", values(var.zone-subnets))}"

  // route53-zone-id           = "${aws_route53_zone.service-zone.id}"
  //admin-subnets             = "${var.build-vpc-cidr-block}"
  cloudwatch-alerts-sns-arn = "${aws_sns_topic.cloudwatch-alerts.arn}"

  # Docker container setttings
  registry-url              = "${var.ecr-aws-account-id}.dkr.ecr.${var.aws-region}.amazonaws.com/${var.env-name}/recordevaluator:${var.dotnet-container-githash}"
  command                   = "Dmarc.DnsRecord.Evaluator.dll"
  parameter                 = "SPF"
  service-name              = "spfrecordevaluator"
  container-memory          = "200"
  connection-string         = "Server = ${aws_rds_cluster.rds-cluster.endpoint}; Port = 3306; Database = ${var.db-name}; Uid = ${var.env-name}_${lookup(var.db-users,"dnsrecordevaluator")};Connection Timeout=5;"
  default-task-count        = "1"
  prod-stage-task-count     = "1"
  health-check-grace-period = "300"

  #Assign to an ECS Cluster
  cluster-id           = "${module.processor-cluster.cluster-id}"
  ecs-service-role-arn = "${module.processor-cluster.ecs-service-role-arn}"

  # Do not assign a load balancer  

  processor-only = "true"
  # Subscribe the input SQS queue to topics
  input-queue-subscriptions      = "${module.dnsrecord-spf-processor.sns-arn}"
  input-queue-subscription-count = "1"
}
