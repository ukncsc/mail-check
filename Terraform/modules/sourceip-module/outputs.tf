output "internal-sources-acl" {
  value = "${data.template_file.internal-sources.rendered}"
}

output "internal-sources-map" {
  value = "${var.internal-sources}"
}

output "government-sources-acl" {
  value = "${data.template_file.government-sources.rendered}"
}

output "government-sources-map" {
  value = "${var.government-sources}"
}

output "police-sources-acl" {
  value = "${data.template_file.police-sources.rendered}"
}

output "police-sources-map" {
  value = "${var.police-sources}"
}

output "localgov-sources-acl" {
  value = "${data.template_file.localgov-sources.rendered}"
}

output "localgov-sources-map" {
  value = "${var.localgov-sources}"
}

output "nhs-sources-acl" {
  value = "${data.template_file.nhs-sources.rendered}"
}

output "nhs-sources-map" {
  value = "${var.nhs-sources}"
}

output "all-external-sources-acl" {
  value = "${data.template_file.all-external-sources.rendered}"
}
