# Define the policy to access database credentials in parameter store
data "aws_iam_policy_document" "parameter-store-policy" {
  statement {
    actions   = ["ssm:DescribeParameters"]
    resources = ["*"]
  }

  statement {
    actions = ["ssm:GetParameter", "ssm:PutParameter"]

    resources = ["arn:aws:ssm:${var.aws-region}:${var.aws-account-id}:parameter/${var.env-name}*"]
  }
}

resource "aws_iam_policy" "parameter-store-policy" {
  count       = "${var.parameter-store-access == "true" ? 1 : 0}"
  name        = "TF-${var.env-name}-${var.cluster-name}-parameterstore"
  path        = "/"
  description = "Instance to SSM parameter store"
  policy      = "${data.aws_iam_policy_document.parameter-store-policy.json}"
}

# attach parameter store policy if permitted
resource "aws_iam_role_policy_attachment" "attach-parameter-store" {
  count      = "${var.parameter-store-access == "true" ? 1 : 0}"
  role       = "${aws_iam_role.ecs-instance-role.name}"
  policy_arn = "${aws_iam_policy.parameter-store-policy.arn}"
}

# attach a custom policy if needed
resource "aws_iam_role_policy_attachment" "attach-custom-policy" {
  count      = "${var.custom-policy-arn != "" ? 1 : 0}"
  role       = "${aws_iam_role.ecs-instance-role.name}"
  policy_arn = "${var.custom-policy-arn}"
}

# Define the policy to access sns topics
data "aws_iam_policy_document" "sns-topic-policy" {
  statement {
    actions   = ["sns:Publish", "sns:ListTopics"]
    resources = ["*"]
  }
}

resource "aws_iam_policy" "sns-topic-policy" {
  count       = "${var.sns-access == "true" ? 1 : 0}"
  name        = "TF-${var.env-name}-${var.cluster-name}-sns-topic"
  path        = "/"
  description = "Instance to SNS Topics"
  policy      = "${data.aws_iam_policy_document.sns-topic-policy.json}"
}

# attach sns topic if permitted
resource "aws_iam_role_policy_attachment" "sns-topic-policy" {
  count      = "${var.sns-access == "true" ? 1 : 0}"
  role       = "${aws_iam_role.ecs-instance-role.name}"
  policy_arn = "${aws_iam_policy.sns-topic-policy.arn}"
}

# Define the policy to access sqs queues
data "aws_iam_policy_document" "sqs-queue-policy" {
  statement {
    actions   = ["sqs:*"]
    resources = ["*"]
  }
}

resource "aws_iam_policy" "sqs-queue-policy" {
  count       = "${var.sqs-access == "true" ? 1 : 0}"
  name        = "TF-${var.env-name}-${var.cluster-name}-sqs-queues"
  path        = "/"
  description = "Instance to SQS queues"
  policy      = "${data.aws_iam_policy_document.sqs-queue-policy.json}"
}

# attach sqs queue if permitted
resource "aws_iam_role_policy_attachment" "attach-sqs-policy" {
  count      = "${var.sqs-access == "true" ? 1 : 0}"
  role       = "${aws_iam_role.ecs-instance-role.name}"
  policy_arn = "${aws_iam_policy.sqs-queue-policy.arn}"
}

# Define the policy to access IAM public keys for SSH
data "aws_iam_policy_document" "ssh-pubkey-policy" {
  statement {
    actions = ["iam:ListUsers",
      "iam:GetGroup",
      "iam:GetSSHPublicKey",
      "iam:ListSSHPublicKeys",
    ]

    resources = ["*"]
  }
}

resource "aws_iam_policy" "ssh-pubkey-policy" {
  name        = "TF-${var.env-name}-${var.cluster-name}-ssh-pubkey"
  path        = "/"
  description = "Instance to IAM SSH Public Keys"
  policy      = "${data.aws_iam_policy_document.ssh-pubkey-policy.json}"
}

# attach ssh policy to iam role
resource "aws_iam_role_policy_attachment" "attach-ssh-pubkey-policy" {
  role       = "${aws_iam_role.ecs-instance-role.name}"
  policy_arn = "${aws_iam_policy.ssh-pubkey-policy.arn}"
}

# Define the policy to allocate and associate EIPs
data "aws_iam_policy_document" "eip-allocation-policy" {
  statement {
    actions   = ["ec2:AllocateAddress", "ec2:AssociateAddress", "ec2:DescribeAddresses", "ec2:DisassociateAddress"]
    resources = ["*"]
  }
}

resource "aws_iam_policy" "eip-allocation-policy" {
  count       = "${var.eip-allocation == "true" ? 1 : 0}"
  name        = "TF-${var.env-name}-${var.cluster-name}-eip-allocation"
  path        = "/"
  description = "Instance to allocate IPs"
  policy      = "${data.aws_iam_policy_document.eip-allocation-policy.json}"
}

# attach eip allocation policy if permitted
resource "aws_iam_role_policy_attachment" "attach-eip-allocation" {
  count      = "${var.eip-allocation == "true" ? 1 : 0}"
  role       = "${aws_iam_role.ecs-instance-role.name}"
  policy_arn = "${aws_iam_policy.eip-allocation-policy.arn}"
}

resource "aws_iam_role_policy" "ecs-instance-policy" {
  name = "TF-${var.env-name}-${var.cluster-name}-ecs-instance-policy"
  role = "${aws_iam_role.ecs-instance-role.id}"

  policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ecs:CreateCluster",
        "ecs:DeregisterContainerInstance",
        "ecs:DiscoverPollEndpoint",
        "ecs:Poll",
        "ecs:RegisterContainerInstance",
        "ecs:StartTelemetrySession",
        "ecs:Submit*",
        "ecr:GetAuthorizationToken",
        "ecr:BatchCheckLayerAvailability",
        "ecr:GetDownloadUrlForLayer",
        "ecr:BatchGetImage"
      ],
      "Resource": [ "*" ]
    },
    {
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:PutLogEvents",
        "logs:DescribeLogStreams"
      ],
      "Resource": [
        "arn:aws:logs:*:*:*"
      ]
    }
  ]
}
EOF
}

resource "aws_iam_role" "ecs-instance-role" {
  name = "TF-${var.env-name}-${var.cluster-name}-ecs-instance-role"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "ec2.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}

resource "aws_iam_instance_profile" "ecs-instance-profile" {
  name = "TF-${var.env-name}-${var.cluster-name}-ecs-instance-profile"
  role = "${aws_iam_role.ecs-instance-role.name}"
}

resource "aws_iam_role_policy" "ecs-service-policy" {
  count = "${var.lb-arn-count}"
  name  = "TF-${var.env-name}-${var.cluster-name}-ecs-service-policy"
  role  = "${aws_iam_role.ecs-service-role.id}"

  policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "ec2:AuthorizeSecurityGroupIngress",
        "ec2:Describe*"
      ],
      "Resource": "*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "elasticloadbalancing:DeregisterInstancesFromLoadBalancer",
        "elasticloadbalancing:Describe*",
        "elasticloadbalancing:RegisterInstancesWithLoadBalancer",
        "elasticloadbalancing:RegisterTargets",
        "elasticloadbalancing:DeregisterTargets"
      ],
      "Resource": [
        "*"
      ]
    }
  ]
}
EOF
}

resource "aws_iam_role" "ecs-service-role" {
  name = "TF-${var.env-name}-${var.cluster-name}-ecs-service-role"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": "sts:AssumeRole",
      "Principal": {
        "Service": "ecs.amazonaws.com"
      },
      "Effect": "Allow",
      "Sid": ""
    }
  ]
}
EOF
}
