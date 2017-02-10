

resource "aws_api_gateway_resource" "aggregate-api-resource" {
  rest_api_id = "${aws_api_gateway_rest_api.aggregate-report-api.id}"
  parent_id = "${aws_api_gateway_rest_api.aggregate-report-api.root_resource_id}"
  path_part = "aggregated"
}

