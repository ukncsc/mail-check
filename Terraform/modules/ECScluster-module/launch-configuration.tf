data "aws_ami" "amazon-ecs" {
  most_recent = true

  filter {
    name   = "name"
    values = ["amzn-ami-*-amazon-ecs-optimized"]
  }

  filter {
    name   = "virtualization-type"
    values = ["hvm"]
  }

  owners = ["amazon"]
}

resource "aws_launch_configuration" "ecs" {
  name_prefix   = "TF-${var.env-name}-${var.cluster-name}"
  image_id      = "${var.ami == "" ? data.aws_ami.amazon-ecs.id : var.ami}"
  instance_type = "${var.env-name == "prod" || var.env-name == "stage" || var.env-name == "staging"? var.prod-stage-instance-type : var.default-instance-type}"

  iam_instance_profile = "${aws_iam_instance_profile.ecs-instance-profile.id}"
  key_name             = "${var.ssh-key-name}"
  security_groups      = ["${aws_security_group.service-instance.id}"]

  user_data = <<DATA
Content-Type: multipart/mixed; boundary="==BOUNDARY=="
MIME-Version: 1.0

--==BOUNDARY==
MIME-Version: 1.0
Content-Type: text/cloud-config; charset="us-ascii"
#cloud-config
repo_update: true
repo_upgrade: all
packages:
${var.launch-packages}
runcmd: 
${var.launch-runcmd}

--==BOUNDARY==
MIME-Version: 1.0
Content-Type: text/x-shellscript; charset="us-ascii"
#!/bin/bash
# Install package to take ssh public keys from IAM
yum install -y https://s3-eu-west-1.amazonaws.com/widdix-aws-ec2-ssh-releases-eu-west-1/aws-ec2-ssh-1.7.0-1.el7.centos.noarch.rpm

cat <<'EOF' > /etc/aws-ec2-ssh.conf
IAM_AUTHORIZED_GROUPS="iam-ssh-users"
LOCAL_MARKER_GROUP="iam-synced-users"
LOCAL_GROUPS=""
SUDOERS_GROUPS="iam-ssh-sudoers"
ASSUMEROLE=""

# Remove or set to 0 if you are done with configuration
# To change the interval of the sync change the file
# /etc/cron.d/aws-ec2-ssh
DONOTSYNC=0


EOF


--==BOUNDARY==
MIME-Version: 1.0
Content-Type: text/x-shellscript; charset="us-ascii"
#!/bin/bash
# remove ntp and install chrony instead (preconfigured for 169.254 ntp server for ec2)
yum -y erase ntp*
yum -y install chrony
service chronyd start
chkconfig --levels 3 chronyd on


--==BOUNDARY==
MIME-Version: 1.0
Content-Type: text/x-shellscript; charset="us-ascii"
#!/bin/bash
# Set up daily security updates
# Stagger restart time based on local IP (assigned randomly)

IP=`hostname | sed -r 's/.*-([0-9]+)$/\1/'`
MINS=$(( $IP % 59 ))

cat <<EOF > ./crontab
SHELL=/bin/bash
PATH=/sbin:/bin:/usr/sbin:/usr/bin
MAILTO=root
HOME=/

# For details see man 4 crontabs

# Example of job definition:
# .---------------- minute (0 - 59)
# |  .------------- hour (0 - 23)
# |  |  .---------- day of month (1 - 31)
# |  |  |  .------- month (1 - 12) OR jan,feb,mar,apr ...
# |  |  |  |  .---- day of week (0 - 6) (Sunday=0 or 7) OR sun,mon,tue,wed,thu,fri,sat
# |  |  |  |  |
# *  *  *  *  * user-name command to be executed

42 * * * * root   run-parts /etc/cron.hourly
$MINS 0 * * * root   run-parts /etc/cron.daily

EOF

sudo cp ./crontab /etc/crontab

cat <<'EOF' > ./security-updates
#!/bin/bash
sudo yum update -y --security
sudo yum update -y ecs-init
sleep 10
sudo reboot

EOF

chmod +x ./security-updates
sudo cp ./security-updates /etc/cron.daily

--==BOUNDARY==
MIME-Version: 1.0
Content-Type: text/x-shellscript; charset="us-ascii"
#!/bin/bash
# Set cluster name
echo ECS_CLUSTER=${aws_ecs_cluster.microservice-cluster.name} >> /etc/ecs/ecs.config

--==BOUNDARY==
MIME-Version: 1.0
Content-Type: text/x-shellscript; charset="us-ascii"
#!/bin/bash
# Install awslogs and the jq JSON parser
yum install -y awslogs jq

# Inject the CloudWatch Logs configuration file contents
cat > /etc/awslogs/awslogs.conf <<- EOF
[general]
state_file = /var/lib/awslogs/agent-state

[/var/log/dmesg]
file = /var/log/dmesg
log_group_name = ${var.env-name}-${var.cluster-name}/var/log/dmesg
log_stream_name = {cluster}/{container_instance_id}

[/var/log/messages]
file = /var/log/messages
log_group_name = ${var.env-name}-${var.cluster-name}/var/log/messages
log_stream_name = {cluster}/{container_instance_id}
datetime_format = %b %d %H:%M:%S

[/var/log/docker]
file = /var/log/docker
log_group_name = ${var.env-name}-${var.cluster-name}/var/log/docker
log_stream_name = {cluster}/{container_instance_id}
datetime_format = %Y-%m-%dT%H:%M:%S.%f

[/var/log/ecs/ecs-init.log]
file = /var/log/ecs/ecs-init.log.*
log_group_name = ${var.env-name}-${var.cluster-name}/var/log/ecs/ecs-init.log
log_stream_name = {cluster}/{container_instance_id}
datetime_format = %Y-%m-%dT%H:%M:%SZ

[/var/log/ecs/ecs-agent.log]
file = /var/log/ecs/ecs-agent.log.*
log_group_name = ${var.env-name}-${var.cluster-name}/var/log/ecs/ecs-agent.log
log_stream_name = {cluster}/{container_instance_id}
datetime_format = %Y-%m-%dT%H:%M:%SZ

[/var/log/ecs/audit.log]
file = /var/log/ecs/audit.log.*
log_group_name = ${var.env-name}-${var.cluster-name}/var/log/ecs/audit.log
log_stream_name = {cluster}/{container_instance_id}
datetime_format = %Y-%m-%dT%H:%M:%SZ

EOF

--==BOUNDARY==
MIME-Version: 1.0
Content-Type: text/x-shellscript; charset="us-ascii"
#!/bin/bash
# Set the region to send CloudWatch Logs data to (the region where the container instance is located)
# region=$(curl 169.254.169.254/latest/meta-data/placement/availability-zone | sed s'/.$//')
region=${var.aws-region}
sed -i -e "s/region = us-east-1/region = $region/g" /etc/awslogs/awscli.conf

--==BOUNDARY==
MIME-Version: 1.0
Content-Type: text/upstart-job; charset="us-ascii"

#upstart-job
description "Configure and start CloudWatch Logs agent on Amazon ECS container instance"
author "Amazon Web Services"
start on started ecs

script
	exec 2>>/var/log/ecs/cloudwatch-logs-start.log
	set -x

	until curl -s http://localhost:51678/v1/metadata
	do
		sleep 1
	done

	# Grab the cluster and container instance ARN from instance metadata
	cluster=$(curl -s http://localhost:51678/v1/metadata | jq -r '. | .Cluster')
	container_instance_id=$(curl -s http://localhost:51678/v1/metadata | jq -r '. | .ContainerInstanceArn' | awk -F/ '{print $2}' )

	# Replace the cluster name and container instance ID placeholders with the actual values
	sed -i -e "s/{cluster}/$cluster/g" /etc/awslogs/awslogs.conf
	sed -i -e "s/{container_instance_id}/$container_instance_id/g" /etc/awslogs/awslogs.conf

	service awslogs start
	chkconfig awslogs on
end script
--==BOUNDARY==--
DATA

  lifecycle {
    create_before_destroy = true
  }
}
