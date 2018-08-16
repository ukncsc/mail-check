provider "aws" {
  region              = "${var.aws-region}"
  allowed_account_ids = "${split(",",var.allowed-account-ids)}"

  assume_role {
    role_arn = "${var.role-to-assume}"
  }
}

provider "aws" {
  alias  = "secondary"
  region = "${var.aws-secondary-region}"
}
