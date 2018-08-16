output "internal-sources-acl" {
  value = "${data.template_file.internal-sources.rendered}"
}

output "internal-sources-map" {
  value = "${var.internal-sources}"
}
output "external-sources-acl" {
  value = "${data.template_file.external-sources.rendered}"
}
output "all-sources-map" {
  value = "${merge(var.internal-sources, var.external-sources)}"
}

output "all-sources-acl" {
  value = "${data.template_file.all-sources.rendered}"
}
