module "securitytester" {
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
  registry-url              = "${var.ecr-aws-account-id}.dkr.ecr.${var.aws-region}.amazonaws.com/${var.env-name}/securitytester:${var.dotnet-container-githash}"
  command                   = "Dmarc.MxSecurityTester.dll"
  service-name              = "mxsecuritytester"
  container-memory          = "200"
  connection-string         = "Server = ${aws_rds_cluster.rds-cluster.endpoint}; Port = 3306; Database = ${var.db-name}; Uid = ${var.env-name}_${lookup(var.db-users,"securitytester")};Connection Timeout=5;"
  default-task-count        = "1"
  prod-stage-task-count     = "1"
  health-check-grace-period = "300"

  docker-environment = ["MxRecordLimit=5",
    "RefreshIntervalSeconds=86400",
    "FailureRefreshIntervalSeconds=1800",
    "SchedulerRunIntervalSeconds=60",
    "TlsTestTimeoutSeconds=10",
    "SmtpHostName=gateway1.${substr(data.aws_route53_zone.dmarc-zone.name,0,length(data.aws_route53_zone.dmarc-zone.name)-1)}",
    "SnsCertsTopicArn=${aws_sns_topic.securitytester-certificates.arn}",
  ]

  docker-environment-count = "7"

  # Enable Redis cache
  cache-node-type = "cache.t2.micro"

  #Assign to an ECS Cluster
  cluster-id           = "${module.processor-cluster.cluster-id}"
  ecs-service-role-arn = "${module.processor-cluster.ecs-service-role-arn}"

  # Do not assign a load balancer  

  processor-only = "true"
}

resource "aws_sns_topic" "securitytester-certificates" {
  name = "TF-${var.env-name}-securitytester-certificates"
}
