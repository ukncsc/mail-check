data "template_file" "internal-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(values(var.internal-sources)))}"
  }
}
data "template_file" "external-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(values(var.external-sources)))}"
  }
}


data "template_file" "all-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(compact(concat(values(var.internal-sources), values(var.external-sources)))))}"
  }
}
