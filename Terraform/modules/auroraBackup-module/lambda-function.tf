resource "aws_lambda_function" "lambdaTakeSnapshotsAurora" {
  count            = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  filename         = "${var.lambda-filename-lambdaTakeSnapshotsAurora}"
  function_name    = "${var.env-name}-lambdaTakeSnapshotsAurora"
  role             = "${aws_iam_role.lambda-role.arn}"
  runtime          = "python3.6"
  handler          = "lambda_function.lambda_handler"
  source_code_hash = "${base64sha256(file(var.lambda-filename-lambdaTakeSnapshotsAurora))}"
  memory_size      = "${var.lambda-memory}"
  timeout          = "${var.lambda-timeout}"

  environment {
    variables = {
      INTERVAL        = "${var.BackupInterval}"
      PATTERN         = "${var.ClusterNamePattern}"
      LOG_LEVEL       = "${var.LogLevel}"
      REGION_OVERRIDE = "${var.SourceRegionOverride}"
    }
  }
}

resource "aws_lambda_function" "lambdaShareSnapshotsAurora" {
  count            = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  filename         = "${var.lambda-filename-lambdaShareSnapshotsAurora}"
  function_name    = "${var.env-name}-lambdaShareSnapshotsAurora"
  role             = "${aws_iam_role.lambda-role.arn}"
  runtime          = "python3.6"
  handler          = "lambda_function.lambda_handler"
  source_code_hash = "${base64sha256(file(var.lambda-filename-lambdaShareSnapshotsAurora))}"
  memory_size      = "${var.lambda-memory}"
  timeout          = "${var.lambda-timeout}"

  environment {
    variables = {
      DEST_ACCOUNT    = "${var.DestinationAccount}"
      PATTERN         = "${var.ClusterNamePattern}"
      LOG_LEVEL       = "${var.LogLevel}"
      REGION_OVERRIDE = "${var.SourceRegionOverride}"
    }
  }
}

resource "aws_lambda_function" "lambdaCopySnapshotsAurora" {
  count            = "${var.source-or-dest == "DEST" ? 1 :0}"
  filename         = "${var.lambda-filename-lambdaCopySnapshotsAurora}"
  function_name    = "${var.env-name}-lambdaCopySnapshotsAurora"
  role             = "${aws_iam_role.lambda-role.arn}"
  runtime          = "python3.6"
  handler          = "lambda_function.lambda_handler"
  source_code_hash = "${base64sha256(file(var.lambda-filename-lambdaCopySnapshotsAurora))}"
  memory_size      = "${var.lambda-memory}"
  timeout          = "${var.lambda-timeout}"

  environment {
    variables = {
      SNAPSHOT_PATTERN    = "${var.SnapshotPattern}"
      DEST_REGION         = "${var.DestinationRegion}"
      KMS_KEY_SOURCE_REGION = "${var.KmsKeySource}"
      KMS_KEY_DEST_REGION = "${var.KmsKeyDestination}"
      RETENTION_DAYS      = "${var.RetentionDays}"
      PATTERN             = "${var.ClusterNamePattern}"
      LOG_LEVEL           = "${var.LogLevel}"
      REGION_OVERRIDE     = "${var.SourceRegionOverride}"
    }
  }
}

resource "aws_lambda_function" "lambdaDeleteOldSnapshotsAurora" {
  count            = "${var.source-or-dest == "SOURCE" ? 1 :0}"
  filename         = "${var.lambda-filename-lambdaDeleteOldSnapshotsAurora}"
  function_name    = "${var.env-name}-lambdaDeleteOldSnapshotsAurora"
  role             = "${aws_iam_role.lambda-role.arn}"
  runtime          = "python3.6"
  handler          = "lambda_function.lambda_handler"
  source_code_hash = "${base64sha256(file(var.lambda-filename-lambdaDeleteOldSnapshotsAurora))}"
  memory_size      = "${var.lambda-memory}"
  timeout          = "${var.lambda-timeout}"

  environment {
    variables = {
      RETENTION_DAYS  = "${var.RetentionDays}"
      PATTERN         = "${var.ClusterNamePattern}"
      LOG_LEVEL       = "${var.LogLevel}"
      REGION_OVERRIDE = "${var.SourceRegionOverride}"
    }
  }
}

resource "aws_lambda_function" "lambdaDeleteOldSnapshotsDestAurora" {
  count            = "${var.source-or-dest == "DEST" ? 1 :0}"
  filename         = "${var.lambda-filename-lambdaDeleteOldSnapshotsDestAurora}"
  function_name    = "${var.env-name}-lambdaDeleteOldSnapshotsDestAurora"
  role             = "${aws_iam_role.lambda-role.arn}"
  runtime          = "python3.6"
  handler          = "lambda_function.lambda_handler"
  source_code_hash = "${base64sha256(file(var.lambda-filename-lambdaDeleteOldSnapshotsDestAurora))}"
  memory_size      = "${var.lambda-memory}"
  timeout          = "${var.lambda-timeout}"

  environment {
    variables = {
      RETENTION_DAYS  = "${var.RetentionDays}"
      PATTERN         = "${var.ClusterNamePattern}"
      LOG_LEVEL       = "${var.LogLevel}"
      REGION_OVERRIDE = "${var.SourceRegionOverride}"
    }
  }
}
