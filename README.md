![N|Solid](https://www.ncsc.gov.uk/global/logos/logo.png)
 
# MailCheck
MailCheck is the [National Cyber Security Centre's](http://www.ncsc.gov.uk/) tooling to test and report on email security for a large number of UK public sector domains.
 
MailCheck processes aggregate DMARC reports for UK public sector organisations and tests the configuration of anti-spoofing controls and support for TLS to encrypt email over SMTP. These tests will be used to help track adoption of the UK government's [email security standard](https://www.gov.uk/guidance/securing-government-email) by the public sector.
 
For an introduction to DMARC and other anti-spoofing controls there is a [good introduction on GOV.UK](https://www.gov.uk/government/publications/email-security-standards/domain-based-message-authentication-reporting-and-conformance-dmarc).
 
Currently MailCheck provides the following:

1. A command line tool to process DMARC aggregate reports and output the data they contain in a variety of formats.
1. An AWS Lambda function for processing DMARC aggregate reports and persisting them to a database.
1. An ECS container for probing mail servers for supported TLS configurations
1. A single page application written in React and Angular2
1. An API returning various statistics and summarised reports from the database of aggregate reports.
1. An API returning the status of a domain's DMARC and SPF records, and TLS configuration of the mail servers
1. Terraform scripts for creating an AWS-based infrastructure to host MailCheck.
1. Terraform scripts for creating a build environment
 
## Architecture

### DMARC report parsing
DMARC reports are received via email using AWS SES, which writes the incoming reports to an S3 bucket and inserts an entry onto a processing queue.
An AWS Lambda function is initiated every minute by an AWS CloudWatch event, which processes the queue of reports. 
The vast majority of DMARC reports are very small, and can be processed by a Lambda function with little memory. However some DMARC reports can be very large. These are put onto a second queue to be processed by a Lambda function with enough memory to process them. 
The Lambda functions are developed in C# dotnet core and essentially parse DMARC reports and insert the data they contain into a relational database.
 

## Repositories

The main repository is mail-check, you will need to clone this into your own location to add configuration files.

A number of microservices have their own repositories, build pipelines and store artefacts in ECR repositories (docker) and S3 buckets (lambda)





## Infrastructure

### Common infrastructure

Found in /Terraform/common
This must run in an AWS region that supports Simple Email Service.

A set of SES rules to process aggregate reports and write them to an S3 bucket, drop forensic reports (we have removed this functionality because of GDPR and lack of support by the large email providers).

This also contains code to replicate the S3 bucket into per environment buckets.



### Per environment infrastructure

Found in /Terraform/prod-env/prod-env

This can be instantiated as many times as necessary, creating production and non production environments as required. All infrastructure here is independent and it its own VPC.



## Data storage

A MySQL Aurora instance provides most of the data storage, however there is a move to microservices with their own storage so this will eventually be deprecated.

Each new style microservice (ReverseDns & CertificateEvaluator) have their own Aurora MySQL database to persist data.



## Front end - containers

An internet facing Application Load Balancer (ALB) is created with one default target which is a container running apache.
The container runs on a separate ECS cluster for security reasons.

The Apache instance serves static content, sets headers, and performs OIDC authentication with the help of the Apache module mod_auth_oidc

Any calls to APIs (paths beginning with /api) are proxied to the backend loadbalancer



## Backend (API) - Containers

An ECS cluster is dedicated for backend API services, this runs a number of microservices which are routed by the ALB using a different target group and path for each service.

/api/aggregatereport/ - Provides statistics from aggregate DMARC reports

/api/domainstatus/ - Provides the status of a domain based on DNS and SMTP probing

/api/admin/ - Allows user management by administrators

/api/metrics/ - Provides high level statistics for reporting

/api/reverse-dns/ - Returns the mapping of an IP address to a hostname

/api/certificates - Provides details on TLS certificates presented by remote mail servers.



## Backend (Processing) - Containers

An ECS cluster is dedicated for backend processing services, this can have a higher load as none of these processes are time critical.

dmarcrecordevaluator - Applies a set of rules against the DMARC records retrieved by the lambda.

spfrecordevaluator - Applies a set of rules against the SPF records retrieved by the lambda.

mxsecuritytester - Connects to each mail server and performs a number of TLS tests

securityevaluator - Applies a set of rules against the results of the TLS tests

reverse-dns-schema-migrations - Manages the database schema for the reverse-dns microservice

certificate-evaluator-schema-migrations - Manages the database schema for the certificate-evaluator microservice




## Build Environment

The build pipeline is defined as code in a file called Jenkinsfile at the root of the repository. Although we use Jenkins it should be fairly easy to adapt to other build tools or run manually.

The tools required are downloaded from within the build script, these are:

Microsoft dotnet - for compiling the C# code 

AWS CLI - to get login for docker repository (ECR), S3 Uploads

Yarn - for the react/angular front end js code

Terraform - for deploying the infrastructure as code

MySql Client - to deploy the schema changes to the database

These are downloaded and installed once per repository branch, this allows multiple environments using different versions of the tool on the same Jenkins box.

You may need to customise paths, or just create the directory /mnt/jenkins-home as this is used for all persistence of files between builds (for tools and artefacts)



The jenkins environment needs sufficient permissions to create and destroy all of the objects needed to deploy the solution. If jenkins is running within AWS this is best achieved via an IAM instance role

Terraform code to create a suitable Jenkins build environment exists in /Terraform/build-env



### Pipeline configuration

The AWS account number is read from a file, either create or modify this line e.g. ```env.AWSACCOUNT = "12345678"```:

 ```env.AWSACCOUNT = readFile("/mnt/jenkins-home/aws-account-number").trim()```



S3 bucket in which to store the terraform state files, either create the file or modify this line:

```env.STATE_S3_BUCKET = readFile(env.STATE_S3_BUCKET_FILE).trim()```

SSH key id, as defined in the jenkins interface, this is for code release so you can replace these lines with = ""
```env.PUBLIC_SSH_DEPLOY_KEY_ID = readFile('/mnt/jenkins-home/public-repo-ssh-deploy-key-id').trim()```
```env.PRIVATE_SSH_DEPLOY_KEY_ID = readFile('/mnt/jenkins-home/private-repo-ssh-deploy-key-id').trim()```



### Build Pipeline Steps


1. Checkout  
   Prepare the build environment by checking out the latest commit from github

1. Tools  
   Install any tools that are needed in the pipeline. One instance per pipeline/branch (moving towards one per version)

1. TF Plan (Common)  
   Generate a plan to make the currently deployed infrastructure match the infrastructure defined in code

1. Compare Dotnet  
   Compare a hash of all of the dotnet code with the previously built code to see if the built and stored artefacts can be reused

1. Dotnet Build  
   Build all code in /src/dotnet

1. Unit Tests  
   Run any unit tests in /src/dotnet/*.Test

1. Package AggregateReportApiDocker  
   Generate the compiled package.

1. Docker Build AggregateReportApi  
   Create a docker container containing everything required to run the compiled package.
   The image in ECR is tagged with the short git commit hash to keep the infrastructure and containers synchronised.

1. Package SecurityTesterDocker  
   Generate the compiled package.

1. Docker Build SecurityTester  
   Create a docker container containing everything required to run the compiled package.
   The image in ECR is tagged with the short git commit hash to keep the infrastructure and containers synchronised.

1. Package SecurityEvaluator  
   Generate the compiled package.

1. Docker Build  SecurityEvaluator  
   Create a docker container containing everything required to run the compiled package.
   The image in ECR is tagged with the short git commit hash to keep the infrastructure and containers synchronised.

1. Package AggregateReportParser  
   Generate the compiled package.

1. Package DNS Importer  
   Generate the compiled package.

1. Package Domain Status  
   Generate the compiled package.

1. Docker Build Domain Status  
   Create a docker container containing everything required to run the compiled package.
   The image in ECR is tagged with the short git commit hash to keep the infrastructure and containers synchronised.

1. Package Record Evaluator  
   Generate the compiled package.

1. Docker Build Record Evaluator  
   Create a docker container containing everything required to run the compiled package.
   The image in ECR is tagged with the short git commit hash to keep the infrastructure and containers synchronised.

1. Package admin API  
   Generate the compiled package.

1. Docker Build Admin API  
   Create a docker container containing everything required to run the compiled package.
   The image in ECR is tagged with the short git commit hash to keep the infrastructure and containers synchronised.

1. Package metrics API  
   Generate the compiled package.

1. Docker Build Metrics  
   Create a docker container containing everything required to run the compiled package.
   The image in ECR is tagged with the short git commit hash to keep the infrastructure and containers synchronised.

1. Compare Angular App  
   Create a docker container containing everything required to run the compiled package.

1. Angular App Build  
   Create a docker container containing everything required to run the compiled package.

1. Angular App Test  
   Create a docker container containing everything required to run the compiled package.

1. Compare React App  
   Compare a hash of all of the dotnet code with the previously built code to see if the built and stored artefacts can be reused

1. React App Test  
   Run the unit tests for React - which are mostly checking against a previous stored state.

1. React App Build  
   Generate production code for frontend.

1. Docker Build Frontend  
   Create a docker container containing the frontend web server and all static content.
   The image in ECR is tagged with the short git commit hash to keep the infrastructure and containers synchronised.

1. TF Plan  
   Build a plan to make the infrastructure specific for this environment match that defined in the terraform code.
   The git shorthash is passed into the plan to migrate the containers to the latest version, this is managed by AWS and the changeover should be seamless.

1. Docker to ECR  
   Upload the docker containers to ECR

1. TF Apply  
   Apply the plan to make the infrastructure specific for this environment match that defined in the terraform code.

1. TF Apply (Common)  
   Apply the plan to make the infrastructure that applies across environments match that defined in the terraform code.
   This includes the simple email service (SES) rules, related S3 buckets, and DNS entries. This is because the reports can only be ingested at one point - although they are later replicated to the different environments.

1. Database Schema  
   Apply any SQL files from /src/sql/schema that have not been previously applied.
   Apply all grants from /src/sql/grants

1. Code Release  
   This manages the release of code from the private repository containing configuration information, to the public repository only copying the generic code.

### Store code hashes

Store hashes of the code related to built (and stored) artefacts, this only happens on a successful build so that a failed build can't get stuck in a loop. 

When a build fails, everything is regenerated on the next build.





## Infrastructure configuration file

The pipeline looks for a file in /Terraform/prod-env/ that matches the name of the branch with the file extension .tf

For master the file is /Terraform/prod-env/prod.tf

For all other branches it is /Terraform/prod-env/branchname.tf

```aws-account-id = "Your AWS Account Number"

backup-account = "AWS account number for snapshot backups (optional)"

KmsKeySource = "AWS KMS key ARN used to encrypt databases for snapshot backups (optional)"

aws-region = "Your AWS Region"

aws-region-name = "Name of region for object names and tags"

aws-secondary-region = "Your AWS region for common infrastructure, we use eu-west-1 as it supports SES"

aws-secondary-region-name = "AWS region name"

ssh-key-name = "A default SSH key for boxes - upload to IAM"

env-name = "example" // must be lower case for s3 bucket

vpc-cidr-block = "x.x.x.x/x"

public-zone-count = "Normally 3 but can reduce to save money"

natgw-count = "Normally 3 but can reduce to save money"
zone-count = "Normally 3 but can reduce to save money"

zone-names = {
zone0 = "eu-west-2a"
zone1 = "eu-west-2b"
zone2 = "eu-west-2c"
}

zone-subnets = {
zone0 = "x.x.x.x/x"
zone1 = "x.x.x.x/x"
zone2 = "x.x.x.x/x"
}

public-zone-names = {
zone0 = "eu-west-2a"
zone1 = "eu-west-2b"
zone2 = "eu-west-2c"
}

public-zone-subnets = {
zone0 = "x.x.x.x/x"
zone1 =  "x.x.x.x/x"
zone2 =  "x.x.x.x/x"
}

frontend-zone-names = {
zone0 = "eu-west-2a"
zone1 = "eu-west-2b"
zone2 = "eu-west-2c"
}

frontend-zone-subnets = {
zone0 = "x.x.x.x/x"
zone1 =  "x.x.x.x/x"
zone2 =  "x.x.x.x/x"
}

db-master-size = "db.t2.medium"

db-replica-size = "db.t2.small"

db-replica-count = "1"

db-name = "mailcheck"

db-snapshot-to-restore = "Put snapshot name in here to restore, or blank for clean DB"

db-kms-key-id = "KMS key ARN for database encryption"

db-username = "Database master username"

db-password = "Database master password"

build-vpc = "VPC ID of the build environment to create a peering for SQL connectivity"

build-route-table = "Routing table ID of the build environment to create a peering for SQL connectivity"


parent-zone = "Route 53 zone for this environment"

web-url = "Web URL for the solution (should be inside the parent-zone above)"

email-domain = "domain for SES"

aggregate-report-bucket = "<Bucket in which to store aggregate reports>"

create-buckets = "1"

cors-origin = "*"

auth-OIDC-provider-metadata = "URL for OIDC metadata - you can use google as a provider - read docs here: https://developers.google.com/identity/protocols/OpenIDConnect"

auth-OIDC-client-id = "client id"

auth-OIDC-client-secret = "client secret"

frontend-OIDCCryptoPassphrase = "Randomly generated string to encrypt session cache"
disable-firewall = "set to true if you want the solution to be publicly accessible"

allow-external = "set to true if you want the solution to be accessible to external users as defined in the source IP module"
```

## Source IP configuration

a module in /Terraform/modules/sourceip-module will need to be configured for your environment then copied into /Terraform/private-moduiles/sourceip-module.

This is a simple module that just returns a list of IP addresses that are used within the security groups when configured without the disable-firewall option, the supplied module is a template for you to add your own IP addresses.

## Contributing
If you'd like to contribute please follow these steps:

1. Sign the [GCHQ Contributor Licence Agreement](https://github.com/gchq/Gaffer/wiki/GCHQ-OSS-Contributor-License-Agreement-V1.0).
2. Push your changes to your fork.
3. Submit a pull request.


