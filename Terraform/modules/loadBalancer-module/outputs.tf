output "lb-dns-name" {
  value = "${aws_lb.lb.dns_name}"
}

output "lb-sg1-id" {
  value = "${aws_security_group.lb-http-sg1.id}"
}

output "lb-sg2-id" {
  value = "${aws_security_group.lb-http-sg2.id}"
}

output "lb-arn" {
  value = "${aws_lb.lb.arn}"
}

output "target-group-arns" {
  value = ["${aws_lb_target_group.lb-target.*.arn}"]
}

output "target-group-paths" {
  value = ["${split(",",var.lb-target-paths)}"]
}
