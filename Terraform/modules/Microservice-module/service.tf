resource "aws_cloudwatch_log_group" "microservice-cluster-log-group" {
  name = "TF-${var.env-name}-service-${var.service-name}"
}

data "template_file" "docker-mountpoints-container" {
  count    = "${var.volumelist-count}"
  template = "$${jsonout}"

  vars {
    jsonout = "${jsonencode(map("sourceVolume","${lookup(var.volumelist[count.index],"name")}", "containerPath","${lookup(var.volumelist[count.index],"host_path")}"))}"
  }
}

data "template_file" "docker-environment" {
  count = "${var.docker-environment-count}"

  template = "$${jsonout}"

  vars {
    jsonout = "${jsonencode(map("Name","${element(split("=",element(var.docker-environment,count.index)),0)}", "Value","${element(split("=",element(var.docker-environment,count.index)),1)}"))}"
  }
}

data "template_file" "docker-port-mapping" {
  count = "${var.docker-port-mapping-count}"

  template = "{\n    \"hostPort\": $${hostPort},\n    \"containerPort\": $${containerPort},\n    \"protocol\": \"$${protocol}\"\n}"

  vars {
    hostPort      = "${element(split(":",element(var.docker-port-mapping,count.index)),0)}"
    containerPort = "${element(split(":",element(var.docker-port-mapping,count.index)),1)}"
    protocol      = "tcp"
  }
}

resource "aws_ecs_task_definition" "microservice-task" {
  family = "TF-${var.env-name}-${var.service-name}-Task"

  container_definitions = <<EOF
[ 
    {
      "memoryReservation": ${var.container-memory},
      "volumesFrom": [],
      "extraHosts": null,
      "dnsServers": null,
      "disableNetworking": null,
      "dnsSearchDomains": null,
      "portMappings": [
       {
          "hostPort": ${var.dynamic-port == "true" ? 0 : var.port}, 
          "containerPort": ${var.port},
          "protocol": "tcp"
        }
        ${var.docker-port-mapping-count == 0 ? "" : ",${join(",",data.template_file.docker-port-mapping.*.rendered)}"}
      ],
      "hostname": null,
      "essential": true,
      "command": ${var.command == "" ? "null" : "[\"${var.command}\", \"${var.parameter}\"]"},
      "name": "${var.env-name}-${var.service-name}",
      "ulimits": null,
      "dockerSecurityOptions": null,
      "environment": [
          {
            "name": "EnvironmentName",
	          "value": "${var.env-name}"
	        },        
          {
            "name": "ServerName",
            "value": "${var.server-name == "" ? "${var.service-name}" : "${var.server-name}"}"
          },
          {
            "name": "CacheHostName",
            "value": "${element(concat(aws_elasticache_replication_group.cache-rg.*.primary_endpoint_address,list("")),0)}"
          },
          {
            "name": "ConnectionString",
            "value": "${var.connection-string}"
          }
          ${var.docker-environment-count == 0 ? "" : ",${join(",",data.template_file.docker-environment.*.rendered)}"  },
          {
            "name": "SqsQueueUrl",
            "value": "${element(concat(aws_sqs_queue.microservice-input-queue.*.id,list("")),0)}"
          },
          {
            "name": "SnsTopicArn",
            "value": "${element(concat(aws_sns_topic.microservice-output.*.arn,list("")),0)}"
          }

      ],
          "mountPoints": [
        ${join(",",data.template_file.docker-mountpoints-container.*.rendered)}
          ],
      "links": null,
      "workingDirectory": null,
      "readonlyRootFilesystem": null,
      "image": "${var.registry-url}",
      "user": null,
      "dockerLabels": null,
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "${aws_cloudwatch_log_group.microservice-cluster-log-group.name}",
          "awslogs-region": "${var.aws-region}",
          "awslogs-stream-prefix": "${var.env-name}-${var.service-name}"
        }
      },
      "cpu": 0,
      "privileged": null,
      "expanded": true
    }
]
EOF

  volume = "${var.volumelist}"

  //  volume = ["${data.template_file.docker-mountpoints-host.*.rendered}"]
}

resource "aws_ecs_service" "loadbalanced-service" {
  count           = "${var.processor-only == "true" ? 0 : 1}"
  depends_on      = ["aws_ecs_task_definition.microservice-task"]
  name            = "TF-${var.env-name}-${var.service-name}"
  cluster         = "${var.cluster-id}"
  task_definition = "${aws_ecs_task_definition.microservice-task.arn}"
  desired_count   = "${var.env-name == "prod" || var.env-name == "stage" || var.env-name == "staging"? var.prod-stage-task-count : var.default-task-count}"
  iam_role        = "${var.ecs-service-role-arn}"

  load_balancer {
    target_group_arn = "${var.target-group-arn}"
    container_name   = "${var.env-name}-${var.service-name}"
    container_port   = "${var.port}"
  }
}

resource "aws_ecs_service" "processor-service" {
  count           = "${var.processor-only == "true" ? 1 : 0}"
  depends_on      = ["aws_ecs_task_definition.microservice-task"]
  name            = "TF-${var.env-name}-${var.service-name}"
  cluster         = "${var.cluster-id}"
  task_definition = "${aws_ecs_task_definition.microservice-task.arn}"
  desired_count   = "${var.env-name == "prod" || var.env-name == "stage" || var.env-name == "staging"? var.prod-stage-task-count : var.default-task-count}"
}
