data "template_file" "internal-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(values(var.internal-sources)))}"
  }
}

data "template_file" "government-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(values(var.government-sources)))}"
  }
}

data "template_file" "police-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(values(var.police-sources)))}"
  }
}

data "template_file" "localgov-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(values(var.localgov-sources)))}"
  }
}

data "template_file" "nhs-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(values(var.nhs-sources)))}"
  }
}

data "template_file" "all-external-sources" {
  template = "$${sourceips}"

  vars {
    sourceips = "${join(",",distinct(compact(concat(values(var.nhs-sources), values(var.localgov-sources), values(var.police-sources), values(var.government-sources)))))}"
  }
}
