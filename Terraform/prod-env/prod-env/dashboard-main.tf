module "microservice-dashboard" {
  source             = "git@github.com:ukncsc/MailCheck.Common//infrastructure/microservice-modules/dashboard?ref=9c0050b"
  env-name           = "${var.env-name}"
  microservice-name  = "Main"
  sqs-queue-count    = "0"
  sqs-queues         = []
  database-count     = "2"
  databases          = ["${var.env-name}-db", "${var.env-name}-db-replica0"]
  target-group-count = "0"
  target-groups      = []
  container-count    = "0"
  containers         = []
  lambda-count       = "0"
  lambdas            = []
  log-group-count    = "0"
  log-groups         = []
  alerts-topic-arn   = "${aws_sns_topic.cloudwatch-alerts.arn}"
}
