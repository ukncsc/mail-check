provider "aws" {
  region = "${var.aws-region}"
}

provider "aws" {
  alias  = "secondary"
  region = "${var.aws-secondary-region}"
}
