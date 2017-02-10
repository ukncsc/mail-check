![N|Solid](https://www.ncsc.gov.uk/global/logos/logo.png)
 
# MailCheck
MailCheck is the [National Cyber Security Centre's](http://www.ncsc.gov.uk/) tooling to test and report on email security for a large number of UK public sector domains.
 
Initially MailCheck development has focussed on processing DMARC reports for UK public sector organisations. Future additions will include testing the configuration of anti-spoofing controls and support for TLS to encrypt email over SMTP. These tests will be used to help track adoption of the UK government's [email security standard](https://www.gov.uk/guidance/securing-government-email) by the public sector. We'll open source new features like these as we add them.
 
For an introduction to DMARC and other anti-spoofing controls there is a [good introduction on GOV.UK](https://www.gov.uk/government/publications/email-security-standards/domain-based-message-authentication-reporting-and-conformance-dmarc).
 
Currently MailCheck provides the following:

1. A command line tool to process DMARC aggregate reports and output the data they contain in a variety of formats.
2. An AWS Lambda function for processing DMARC aggregate reports and persisting them to a database.
3. An AWS API Gateway for returning various statistics and summarised reports from the database of aggregate reports.
4. Terraform scripts for creating an AWS-based infrastructure to host MailCheck.
 
## Architecture

### DMARC report parsing back-end
DMARC reports are received via email using AWS SES, which writes the incoming reports to an S3 bucket and inserts an entry onto a processing queue.
An AWS Lambda function is initiated every minute by an AWS CloudWatch event, which processes the queue of reports. 
The vast majority of DMARC reports are very small, and can be processed by a Lambda function with little memory. However some DMARC reports can be very large. These are put onto a second queue to be processed by a Lambda function with enough memory to process them. 
The Lambda functions are developed in C# dotnet core 1.0 and essentially parse DMARC reports and insert the data they contain into a relational database.

### Application back-end
An application back-end is provided via various AWS Lambda functions served by CloudFront and an API Gateway. These essentially pull summary reports and data from the database. The Lambda functions are written in C# .NET Core 1.0. 

### Application front-end
No front-end is currently included, but it's something we're working on. 

### Re-creating the environment
The Terraform variables will need to be defined to suit your specific environment, please create a .tfvars file entry for each of the variables defined in variables.tf
Terraform scripts in prod-env can be created for each of your environments. 
Scripts in common are to create objects that do not exist within an AWS VPC or are otherwise global.

Use the Jenkins pipeline to compile the code and build the environment in AWS. You can manually deploy following the steps in the Jenkinsfile.
 
## Contributing
If you'd like to contribute please follow these steps:

1. Sign the [GCHQ Contributor Licence Agreement](https://github.com/gchq/Gaffer/wiki/GCHQ-OSS-Contributor-License-Agreement-V1.0).
2. Push your changes to your fork.
3. Submit a pull request.


