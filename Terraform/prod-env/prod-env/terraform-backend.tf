terraform {
    backend "s3" {
        bucket = "ncscdmarc-terraform-state"
        key = ""
        region = "eu-west-2"
//	shared_credentials_file = "/mnt/jenkins-home/aws-shared-credentials"
    }
}
