module "lambdaAPI-daily-disposition" {
  source	      	= "./lambdaAPI-module"
  env-name            	= "${var.env-name}"
  aws-region          	= "${var.aws-region}"
  aws-account-id	= "${var.aws-account-id}"
  parent-id	      	= "${aws_api_gateway_resource.daily-api-resource.id}"
  api-path	      	= "disposition"
  authorizer-id       	= "${aws_api_gateway_authorizer.app-api-authorizer.id}"
  rest-api-id	      	= "${aws_api_gateway_rest_api.aggregate-report-api.id}"
  lambda-filename     	= "AggregateReportApi.zip"
  source-code-hash	= "${base64sha256(file("AggregateReportApi.zip"))}"
  lambda-function-name	= "TF-${var.env-name}-AggregateReportApi_GetDailyDispositionStatistics"
  handler		= "Dmarc.AggregateReport.Api::Dmarc.AggregateReport.Api.AggregateReportApi::GetDailyDispositionStatistics"
  lambda-role		= "${aws_iam_role.lambda-aggregate-reports.arn}"
  connection-string	= "Server = ${aws_rds_cluster.rds-cluster.reader_endpoint}; Port = 3306; Database = ${var.db-name}; Uid = ${var.db-api-uname}; Pwd = ${var.db-api-pwd};Connection Timeout=5;"
  subnet-ids		= "${join(",", aws_subnet.dmarc-env-subnet.*.id)}"
  security-group-ids	=  "${aws_security_group.lambda-aggregate.id}" 	
  request-parameters    =  {
		"method.response.header.Access-Control-Allow-Origin" = "${var.cors-origin}"
			}
  path-parameter	=  "{domain}"	 

}  
