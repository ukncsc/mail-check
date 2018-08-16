module "frontend" {
  source = "../../modules/Microservice-module"

  # Project settings

  vpc-id             = "${aws_vpc.dmarc-env.id}"
  aws-region         = "${var.aws-region}"
  env-name           = "${var.env-name}"
  aws-account-id     = "${var.aws-account-id}"
  availability-zones = "${join(",", values(var.frontend-zone-names))}"
  subnet-ids         = "${join(",", aws_subnet.frontend-subnet.*.id)}"
  subnet-cidr        = "${join(",", values(var.frontend-zone-subnets))}"
  //route53-zone-id           = "${aws_route53_zone.service-zone.id}"
  //admin-subnets             = "${var.build-vpc-cidr-block}"
  cloudwatch-alerts-sns-arn = "${aws_sns_topic.cloudwatch-alerts.arn}"
  # Docker container setttings
  registry-url              = "${var.ecr-aws-account-id}.dkr.ecr.${var.aws-region}.amazonaws.com/${var.env-name}/frontend:${var.frontend-container-githash}"
  service-name              = "frontend"
  server-name               = "${var.web-url}"
  container-memory          = "${var.default-container-memory}"
  default-task-count        = "1"
  prod-stage-task-count     = "3"
  health-check-grace-period = "300"
  docker-environment = ["auth_OIDC_client_id=${var.auth-OIDC-client-id}",
    "auth_OIDC_client_secret=${var.auth-OIDC-client-secret}",
    "auth_OIDC_provider_metadata=${var.auth-OIDC-provider-metadata}",
    "INT_DOMAIN=${var.env-name}.i.mailcheck.service.ncsc.gov.uk",
    "CORS_ORIGIN=${var.cors-origin}",
    "OIDCCryptoPassphrase=${var.frontend-OIDCCryptoPassphrase}",
  ]
  docker-environment-count = "6"
  # Enable Redis cache
  cache-node-type = "cache.t2.micro"

  # Assign to an ECS Cluster

  cluster-id           = "${module.frontend-cluster.cluster-id}"
  ecs-service-role-arn = "${module.api-cluster.ecs-service-role-arn}"

  # Assign to a load balancer

  target-group-arn = "${element(module.loadbalancer-external.target-group-arns, 0)}"
}
