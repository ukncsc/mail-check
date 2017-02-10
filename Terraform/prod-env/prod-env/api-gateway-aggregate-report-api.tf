resource "aws_api_gateway_rest_api" "aggregate-report-api" {
  name = "${var.env-name}-AggregateReportApi"
}
resource "aws_api_gateway_deployment" "aggregate-report-deployment" {
  rest_api_id = "${aws_api_gateway_rest_api.aggregate-report-api.id}"
  stage_name = "api"

  variables = {
  }
}


