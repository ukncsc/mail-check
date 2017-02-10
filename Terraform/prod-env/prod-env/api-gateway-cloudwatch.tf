resource "aws_api_gateway_account" "api-gateway-logging" {
  cloudwatch_role_arn = "${aws_iam_role.apigw-cloudwatch-role.arn}"
}

resource "aws_iam_role" "apigw-cloudwatch-role" {
    name = "TF-${var.env-name}-api_gateway_cloudwatch_global"
    assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "",
      "Effect": "Allow",
      "Principal": {
        "Service": "apigateway.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
}
EOF
}

resource "aws_iam_role_policy" "apigw-cloudwatch-policy" {
    name = "TF-${var.env-name}-apigw-cloudwatch"
    role = "${aws_iam_role.apigw-cloudwatch-role.id}"
    policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "logs:CreateLogGroup",
                "logs:CreateLogStream",
                "logs:DescribeLogGroups",
                "logs:DescribeLogStreams",
                "logs:PutLogEvents",
                "logs:GetLogEvents",
                "logs:FilterLogEvents"
            ],
            "Resource": "*"
        }
    ]
}
EOF
}
