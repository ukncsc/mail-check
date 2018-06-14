module "domainstatusapi" {
  source = "../../modules/Microservice-module"

  # Project settings

  vpc-id             = "${aws_vpc.dmarc-env.id}"
  aws-region         = "${var.aws-region}"
  env-name           = "${var.env-name}"
  aws-account-id     = "${var.aws-account-id}"
  availability-zones = "${join(",", values(var.zone-names))}"
  subnet-ids         = "${join(",", aws_subnet.dmarc-env-subnet.*.id)}"
  subnet-cidr        = "${join(",", values(var.zone-subnets))}"
  //route53-zone-id           = "${aws_route53_zone.service-zone.id}"
  //admin-subnets             = "${data.aws_vpc.build-vpc.cidr_block}"
  cloudwatch-alerts-sns-arn = "${aws_sns_topic.cloudwatch-alerts.arn}"

  # Docker container setttings

  registry-url              = "727873987145.dkr.ecr.eu-west-2.amazonaws.com/${var.env-name}/domainstatusapi:${var.dotnet-container-githash}"
  command                   = "Dmarc.DomainStatus.Api.dll"
  service-name              = "domainstatusapi"
  container-memory          = "${var.default-container-memory}"
  connection-string         = "Server = ${aws_rds_cluster.rds-cluster.endpoint}; Port = 3306; Database = ${var.db-name}; Uid = ${var.env-name}_${lookup(var.db-users,"statusapi")};Connection Timeout=5;"
  default-task-count        = "1"
  prod-stage-task-count     = "3"
  health-check-grace-period = "300"
  docker-environment = ["ASPNETCORE_URLS=http://+:80",
    "ReverseDnsApiEndpoint=https://api.${var.env-name}.i.mailcheck.service.ncsc.gov.uk/api/reverse-dns/",
  ]
  docker-environment-count = "2"
  # Assign to an ECS Cluster
  cluster-id           = "${module.api-cluster.cluster-id}"
  ecs-service-role-arn = "${module.api-cluster.ecs-service-role-arn}"

  # Assign to a load balancer

  target-group-arn = "${element(module.loadbalancer-internal.target-group-arns, index(module.loadbalancer-internal.target-group-paths, "domainstatus"))}"
}
