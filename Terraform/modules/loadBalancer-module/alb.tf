# Create a new load balancer
resource "aws_lb" "lb" {
  name            = "TF-${var.env-name}-${var.balancer-name}"
  subnets         = ["${split(",", var.lb-subnet-ids)}"]
  security_groups = ["${aws_security_group.lb-http-sg1.id}", "${aws_security_group.lb-http-sg2.id}", "${aws_security_group.lb-https-sg1.id}", "${aws_security_group.lb-https-sg2.id}"]
  internal        = "${var.lb-internal}"
  idle_timeout    = 120

  access_logs {
    bucket = "${aws_s3_bucket.lblogs.bucket}"
    prefix = "TF-${var.env-name}-${var.balancer-name}"
  }
}
