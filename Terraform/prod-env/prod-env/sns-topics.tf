resource "aws_sns_topic" "selector-seen-in-aggregate-report" {
  name = "TF-${var.env-name}-selector-seen-in-aggregate-report"
}
