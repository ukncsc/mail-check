data "terraform_remote_state" "tfstate" {
    backend = "s3"
	config {
        bucket = "ncscdmarc-terraform-state"
        key = "${var.env-name}/terraform.tfstate"
        region = "eu-west-2"
//	shared_credentials_file = "/mnt/jenkins-home/aws-shared-credentials"
    }
}
