resource "aws_cloudwatch_dashboard" "queue" {
  dashboard_name = "TF-${var.env-name}-Processor-Queues"

  dashboard_body = <<EOF
   {
    "widgets": [
        {
            "type": "metric",
            "x": 0,
            "y": 0,
            "width": 8,
            "height": 6,
            "properties": {
                "view": "timeSeries",
                "stacked": false,
                "metrics": [
                    [ "AWS/SQS", "ApproximateNumberOfMessagesVisible", "QueueName", "${aws_sqs_queue.aggregate-report-queue1.name}" ],
                    [ "AWS/SQS", "NumberOfMessagesSent", "QueueName", "${aws_sqs_queue.aggregate-report-queue1.name}"]
                ],
                "region": "${var.aws-region}",
                "title": "${aws_sqs_queue.aggregate-report-queue1.name}-Visible/Sent",
                "period": 300
            }
        },
        {
            "type": "metric",
            "x": 8,
            "y": 0,
            "width": 8,
            "height": 6,
            "properties": {
                "view": "timeSeries",
                "stacked": false,
                "metrics": [
                    [ "AWS/SQS", "ApproximateNumberOfMessagesVisible", "QueueName", "${aws_sqs_queue.aggregate-report-queue2.name}" ],
                    [ "AWS/SQS", "NumberOfMessagesSent", "QueueName", "${aws_sqs_queue.aggregate-report-queue2.name}"]
                ],
                "region": "${var.aws-region}",
                "title": "${aws_sqs_queue.aggregate-report-queue2.name}-Visible/Sent",
                "period": 300
            }
        },
        {
            "type": "metric",
            "x": 16,
            "y": 0,
            "width": 8,
            "height": 6,
            "properties": {
                "view": "timeSeries",
                "stacked": false,
                "metrics": [
                    [ "AWS/SQS", "ApproximateNumberOfMessagesVisible", "QueueName", "${aws_sqs_queue.aggregate-report-deadletter.name}"],
                    [ "AWS/SQS", "NumberOfMessagesSent", "QueueName", "${aws_sqs_queue.aggregate-report-deadletter.name}"]

                ],
                "region": "${var.aws-region}",
                "title": "${aws_sqs_queue.aggregate-report-deadletter.name}-Visible/Sent",
                "period": 300
            }
        }
    ]
   } 
EOF
}
