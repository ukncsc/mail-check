resource "aws_cloudwatch_dashboard" "s3-buckets" {
  dashboard_name = "TF-${var.env-name}-S3-Buckets"

  dashboard_body = <<EOF
   {
    "widgets": [
            {
            "type": "metric",
            "x": 0,
            "y": 6,
            "width": 12,
            "height": 6,
            "properties": {
                "view": "timeSeries",
                "stacked": false,
                "metrics": [   
                    [ "AWS/S3", "BucketSizeBytes", "StorageType", "StandardStorage","BucketName",  "${aws_s3_bucket.aggregate-bucket.id}" ]
                ],
                "region": "${var.aws-region}",
                "title": "${aws_s3_bucket.aggregate-bucket.id}-Size",
                "period": 86400
            }
        },
        {
            "type": "metric",
            "x": 12,
            "y": 6,
            "width": 12,
            "height": 6,
            "properties": {
                "view": "timeSeries",
                "stacked": false,
                "metrics": [
                    [ "AWS/S3", "NumberOfObjects", "StorageType", "AllStorageTypes", "BucketName", "${aws_s3_bucket.aggregate-bucket.id}" ]
                ],
                "region": "${var.aws-region}",
                "title": "${aws_s3_bucket.aggregate-bucket.id}-Object-Count",
                "period": 86400
            }
        }
    ]
   } 
EOF
}
