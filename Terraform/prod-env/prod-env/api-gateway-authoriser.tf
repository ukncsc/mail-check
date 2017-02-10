resource "aws_api_gateway_authorizer" "app-api-authorizer" {
  name = "${var.env-name}-API-Authorizer"
  rest_api_id = "${aws_api_gateway_rest_api.aggregate-report-api.id}"
  authorizer_uri = "arn:aws:apigateway:${var.aws-region}:lambda:path/2015-03-31/functions/${aws_lambda_function.authorizer.arn}/invocations"
  authorizer_credentials = "${aws_iam_role.auth_invocation_role.arn}"
  authorizer_result_ttl_in_seconds = "300"
}

resource "aws_lambda_function" "authorizer" {
  filename = "AggregateReportApi.zip"
  source_code_hash = "${base64sha256(file("AggregateReportApi.zip"))}"
  function_name = "TF-${var.env-name}-AggregateReportApi_Authorize"
  role = "${aws_iam_role.auth_lambda.arn}"
  handler = "Dmarc.AggregateReport.Api::Dmarc.AggregateReport.Api.Auth.AggregateReportApiAuth::Authorize"
  runtime = "dotnetcore1.0"
  timeout  = "30"
 environment {
        variables = {
		ApiKey = "${var.api-key}"
		ApiArn = "arn:aws:execute-api:${var.aws-region}:${var.aws-account-id}:*"
                    }
              }


}

#resource "aws_lambda_permission" "allow_api_gateway_auth" {
#    function_name = "${aws_lambda_function.authorizer.arn}"
#    statement_id = "AllowExecutionFromApiGateway"
#    action = "lambda:InvokeFunction"
#    principal = "apigateway.amazonaws.com"
#   source_account = "${var.aws-account-id}"
#    source_arn = "arn:aws:execute-api:${var.aws-region}:${var.aws-account-id}:*/*"
#}

