properties([disableConcurrentBuilds(), pipelineTriggers([githubPush()])])
node {
    stage('Checkout') {

//---------------- Variables/Definitions
    env.WORKSPACE = pwd()
    env.JENKINSHOME = "/mnt/jenkins-home"
	env.TERRAFORMPATH = "/mnt/jenkins-home/terraform-${BRANCH_NAME}"
	env.TERRAFORM ="${env.TERRAFORMPATH}/terraform"
	env.TERRAFORMVERSION = "0.11.7"
	env.TERRAFORMURL = "https://releases.hashicorp.com/terraform/${env.TERRAFORMVERSION}/terraform_${env.TERRAFORMVERSION}_linux_amd64.zip"
	env.YARNPATH = "/mnt/jenkins-home/yarn-${BRANCH_NAME}"
	env.YARN ="${env.YARNPATH}/yarn-v1.9.4/bin/yarn"
	env.YARNVERSION = "1.9.4"
	env.YARNURL = "https://yarnpkg.com/latest.tar.gz"
	env.NODEPATH ="/mnt/jenkins-home/node-${BRANCH_NAME}"
	env.TF_PLAN_FILE = "TF-${BRANCH_NAME}-plan.out"
	env.TF_COMMON_PLAN_FILE = "TF-common-plan.out"
	env.NODEVERSION = "v8.11.2"
    env.NODE = "${env.NODEPATH}/node-${env.NODEVERSION}-linux-x64/bin/node"
	env.NPM  = "${env.NODEPATH}/node-${env.NODEVERSION}-linux-x64/bin/npm"
    env.NGPATH = "/mnt/jenkins-home/ng-${BRANCH_NAME}"
	env.NG = "${env.NGPATH}/lib/node_modules/@angular/cli/bin/ng"
	env.NGVERSION = "@angular/cli: 1.0.0"
	env.AWSPATH = "/mnt/jenkins-home/aws-${BRANCH_NAME}"
	env.AWS = "${env.AWSPATH}/bin/aws"
	env.AWSREGION = "eu-west-2"
	env.AWSVERSION = "11529"
	env.AWSURL = "https://s3.amazonaws.com/aws-cli/awscli-bundle.zip"
	env.MySQLPATH = "/mnt/jenkins-home/mysql-${BRANCH_NAME}"
	env.MySQL = "${env.MySQLPATH}/mysql"
	env.MySQLVERSION = "5.6.35"
	env.PATH = "${env.NODEPATH}/node-${env.NODEVERSION}-linux-x64/bin/:${env.DOTNETPATH}:${env.PATH}" 
	env.FRONTENDHASHFILE = "/mnt/jenkins-home/${BRANCH_NAME}-frontend-hash"
	env.DOTNETHASHFILE = "/mnt/jenkins-home/${BRANCH_NAME}-dotnet-code-hash"
    env.DOTNET_CONTAINER_GITHASH_FILE = "/mnt/jenkins-home/${BRANCH_NAME}-dotnet-container-githash"
	env.FRONTEND_CONTAINER_GITHASH_FILE = "/mnt/jenkins-home/${BRANCH_NAME}-frontend-container-githash"
    env.DOTNETBINARYSTASH = "/mnt/jenkins-home/${BRANCH_NAME}-dotnet-binary-stash/"
    env.DOTNETPUBLISHSTASH = "/mnt/jenkins-home/${BRANCH_NAME}-dotnet-publish-stash/"
	env.ANGULARAPPSTASH = "/mnt/jenkins-home/${BRANCH_NAME}-angular-app-stash/"
    env.ANGULARAPPHASHFILE = "/mnt/jenkins-home/${BRANCH_NAME}-angular-app-code-hash"
	env.ANGULARPUBLICHASHFILE = "/mnt/jenkins-home/${BRANCH_NAME}-angular-public-code-hash"
	env.REACTAPPSTASH = "/mnt/jenkins-home/${BRANCH_NAME}-react-app-stash/"
    env.REACTAPPHASHFILE = "/mnt/jenkins-home/${BRANCH_NAME}-react-app-code-hash"
	env.REACTPUBLICHASHFILE = "/mnt/jenkins-home/${BRANCH_NAME}-react-public-code-hash"
    env.PUBLIC_SSH_DEPLOY_KEY_ID = readFile('/mnt/jenkins-home/public-repo-ssh-deploy-key-id').trim()
	env.PRIVATE_SSH_DEPLOY_KEY_ID = readFile('/mnt/jenkins-home/private-repo-ssh-deploy-key-id').trim()
    env.STATE_S3_BUCKET_FILE = "/mnt/jenkins-home/state-s3-bucket"
	env.STATE_S3_BUCKET = readFile(env.STATE_S3_BUCKET_FILE).trim()


//	env.APP_S3_BUCKET_FILE = "/tmp/${BRANCH_NAME}-app-s3-bucket"
//    env.PUBLIC_S3_BUCKET_FILE = "/tmp/${BRANCH_NAME}-public-s3-bucket"


//-----------------Checkout
//        gitClean()
    slackSend "Build Started - ${env.JOB_NAME} ${env.BUILD_NUMBER} (<${env.BUILD_URL}|Open>)"
    sh "rm -rf *"
    sh "rm -rf .git"
    checkout scm
	sh "git rev-parse --short HEAD | tee shorthash"
	env.GITSHORTHASH = readFile('shorthash').trim()
	echo "Checkout complete... extracting environment name from tfvars"

   }
   stage('Tools') {
   // Output build container environment
   sh "cat /etc/*-release"
   // Check docker works
   sh "docker -v"

   install_aws()
   install_mysql()
   install_node()
   install_angular()
   install_yarn()
   install_terraform()
   env.DOTNET = install_dotnet("dotnet","https://download.microsoft.com/download/E/8/A/E8AF2EE0-5DDA-4420-A395-D1A50EEFD83E/dotnet-sdk-2.1.401-linux-x64.tar.gz")
   }
   
   stage('Environment') {
	 

	 if ("${BRANCH_NAME}" == "master") {  
	    env.TFVARS_FILE = "prod.tfvars"
		env.STATE_KEY = "prod"
		} else {
        env.TFVARS_FILE = "${BRANCH_NAME}.tfvars"
		env.STATE_KEY = "${BRANCH_NAME}"
		}

		sh "cat Terraform/prod-env/${env.TFVARS_FILE} | grep -E '^env-name' | tr -d ' ' | awk '{print \$2 }' FS='=' | awk '{print \$2 }' FS='\"' |tr -dc ' \$0-9a-z-'   > env-name"
		env.ENV_NAME = readFile("env-name").trim()
		echo "Environment name: ${env.ENV_NAME}"
	
 		sh "cat Terraform/prod-env/${env.TFVARS_FILE} | grep -E '^role-to-assume' | tr -d ' ' | awk '{print \$2 }' FS='=' | awk '{print \$2 }' FS='\"' >role-to-assume"
		env.ROLE_TO_ASSUME = readFile("role-to-assume").trim()
		echo "Role to assume: ${env.ROLE_TO_ASSUME}"
// Get AWS account numbers from parameter store

		env.AWSACCOUNT = sh(returnStdout : true, script: "${env.AWS} ssm get-parameters --names ${env.ENV_NAME}-account --region=${env.AWSREGION} | jq -r '.Parameters[0].Value'").trim()
		env.buildAwsAccount = sh(returnStdout : true, script: "${env.AWS} ssm get-parameters --names build-account --region=${env.AWSREGION} | jq -r '.Parameters[0].Value'").trim()
		env.ecrAwsAccount = sh(returnStdout : true, script: "${env.AWS} ssm get-parameters --names ecr-account --region=${env.AWSREGION} | jq -r '.Parameters[0].Value'").trim()
		if (env.ROLE_TO_ASSUME != "") {
			write_aws_config("/tmp/${env.BUILD_NUMBER}-aws.conf")
		}

   }


//  Terraform the common components which can't exist per environment (SES etc)

    stage('TF Plan (Common)') {
                    
        //Remove the terraform state file so we always start from a clean state
        if (fileExists(".terraform/terraform.tfstate")) {
            sh "rm -rf .terraform/terraform.tfstate"
        }
        if (fileExists("status")) {
            sh "rm status"
        }
		
        sh "cd Terraform/common/common; ${env.TERRAFORM} init -backend=true -force-copy -input=false -backend-config=\"key=common/terraform.tfstate\""
//        sh "${env.TERRAFORM} get Terraform/common/common"
         sh "set +e; cd Terraform/common/common; ${env.TERRAFORM} plan -detailed-exitcode -refresh=true -out=${env.TF_COMMON_PLAN_FILE} -var-file ../common.tfvars .; echo \$? > /tmp/status"
		
		def exitCode = readFile('/tmp/status').trim()
        env.APPLY = "false"
        echo "Terraform Plan Exit Code: ${exitCode}"
        if (exitCode == "0") {
            currentBuild.result = 'SUCCESS'
        }
        if (exitCode == "1") {
            error "Plan Failed: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
            currentBuild.result = 'FAILURE'
        }
		
        if (exitCode == "2") {
		    if ("${BRANCH_NAME}" == "master") { 
                echo "Plan Awaiting Approval: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
                try {
                    input message: 'Apply Plan?', ok: 'Apply'
                    env.APPLY = "true"
                } catch (err) {
                    error "Plan Discarded: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
                    env.APPLY = "false"
                    currentBuild.result = 'UNSTABLE'
                }
			}
        }
    }


//---------------------Compare dotnet code hash to avoid recompiling unnecessarily
    stage('Compare Dotnet') {
	    if (fileExists("${env.DOTNETHASHFILE}") && fileExists("${env.DOTNET_CONTAINER_GITHASH_FILE}")) { 
		    env.PREVDOTNETHASH =  readFile("${env.DOTNETHASHFILE}").trim()
			sh "rm -f ${env.DOTNETHASHFILE}"
		}

	    sh "find src/dotnet -type f -print0 | sort -z | xargs -0 sha1sum | sha1sum |  cut -d \" \" -f1 | tee dotnethash"
	    env.DOTNETHASH = readFile("dotnethash").trim()
	    if (env.PREVDOTNETHASH == env.DOTNETHASH) {
		    env.DOTNETCOMPILE = "false"
			env.DOTNET_CONTAINER_GITHASH = readFile("${env.DOTNET_CONTAINER_GITHASH_FILE}").trim()
			echo "Previous dotnet code hash ${env.PREVDOTNETHASH} matches current commit. Skipping."   
	    } else {
		    env.DOTNETCOMPILE = "true"
			env.DOTNET_CONTAINER_GITHASH = env.GITSHORTHASH
			echo "Previous dotnet code hash ${env.PREVDOTNETHASH} does not match ${env.DOTNETHASH}. Recompiling....."
			sh "rm -rf ${env.DOTNETPUBLISHSTASH}"
			sh "mkdir -p ${env.DOTNETPUBLISHSTASH}"
             }
	}

// ---------------------Build dotnet code

	stage('Dotnet Build') {
	    if (env.DOTNETCOMPILE == "true") {
	        env.PROJECT = "src/dotnet/Dmarc/src/"
			
			sh "rm -rf ${env.DOTNETBINARYSTASH}"
            sh "mkdir -p ${env.DOTNETBINARYSTASH}"	
			sh "cp -r ${env.PROJECT}* ${env.DOTNETBINARYSTASH}"
			
			env.PROJECT = "${env.DOTNETBINARYSTASH}"
			sh "rm -rf /mnt/jenkins-home/.nuget"			
           
			sh "#!/bin/bash \n" +
			"for f in ${env.PROJECT}*/; do \n" +
			"echo Building in \n" +
			"cd \$f \n" +
			"pwd \n" +
			"if ${env.DOTNET} build --version-suffix build-${env.GITSHORTHASH} \n" +
			"then \n" +
			"echo Successfully built \$f \n" +
			"else \n" +
			"exit 1\n" +
			"fi \n" +
			"done" 
			
        } 
    }


    stage('Unit Tests') {
        if (env.DOTNETCOMPILE == "true") {
		    sh "/bin/df -kh"
            sh "#!/bin/bash \n" +
			   "for f in ${env.PROJECT}/*.Test; do \n" +
			   "echo Running unit tests on \$f \n" +
			   "cd \$f \n" +
			   "if ${env.DOTNET} test --no-build --filter TestCategory!=Integration \n" +
			   "then \n" +
			   "echo Tests ok \n" +
			   "else \n" +
			   "exit 1\n" +
			   "fi \n" +
			   "done" 
        }
		
    }


// ---------------------AggregateReportApi

    stage('Package AggregateReportApi') {
	    if (env.DOTNETCOMPILE == "true") {
		    sh "cd ${env.PROJECT}Dmarc.AggregateReport.Api; ${env.DOTNET} publish -c release -o ${env.DOTNETPUBLISHSTASH}AggregateReportApi"
		}
    }

	stage('Docker Build AggregateReportApi') {
        if (env.DOTNETCOMPILE == "true") {
		    sh "cp src/docker/microservice/Dockerfile ${env.DOTNETPUBLISHSTASH}AggregateReportApi/Dockerfile"
		    sh "cd ${env.DOTNETPUBLISHSTASH}AggregateReportApi;ls"
            sh "cd ${env.DOTNETPUBLISHSTASH}AggregateReportApi;docker build -t ${ENV_NAME}-aggregatereportapi ."
		}
	}

// ---------------------SecurityTester

    stage('Package SecurityTester') {
	    if (env.DOTNETCOMPILE == "true") {
		    sh "cd ${env.PROJECT}Dmarc.MxSecurityTester; ${env.DOTNET} publish -c release -o ${env.DOTNETPUBLISHSTASH}MxSecurityTester"
		}
    }

	stage('Docker Build SecurityTester') {
        if (env.DOTNETCOMPILE == "true") {
		    sh "cp src/docker/microservice/Dockerfile ${env.DOTNETPUBLISHSTASH}MxSecurityTester/Dockerfile"
		    sh "cd ${env.DOTNETPUBLISHSTASH}MxSecurityTester;ls"
            sh "cd ${env.DOTNETPUBLISHSTASH}MxSecurityTester;docker build -t ${ENV_NAME}-securitytester ."
		}
	}

// ---------------------SecurityEvaluator

    stage('Package SecurityEvaluator') {
	    if (env.DOTNETCOMPILE == "true") {
		    sh "cd ${env.PROJECT}Dmarc.MxSecurityEvaluator; ${env.DOTNET} publish -c release -o ${env.DOTNETPUBLISHSTASH}MxSecurityEvaluator"
		}
    }

	stage('Docker Build SecurityEvaluator') {
        if (env.DOTNETCOMPILE == "true") {
		    sh "cp src/docker/microservice/Dockerfile ${env.DOTNETPUBLISHSTASH}MxSecurityEvaluator/Dockerfile"
		    sh "cd ${env.DOTNETPUBLISHSTASH}MxSecurityEvaluator;ls"
            sh "cd ${env.DOTNETPUBLISHSTASH}MxSecurityEvaluator;docker build -t ${ENV_NAME}-securityevaluator ."
		}
	}



// ---------------------AggregateReportParser

  
    stage('Package AggregateReportParser') {
	    if (env.DOTNETCOMPILE == "true") {
            sh "cd ${env.PROJECT}Dmarc.AggregateReport.Parser.Lambda; ${env.DOTNET} lambda package --framework netcoreapp2.1 -c Release -o ${env.DOTNETPUBLISHSTASH}AggregateReportParser.zip"
        }
	}

// ---------------------DNS Importer

    stage('Package DNS Importer') {
	    if (env.DOTNETCOMPILE == "true") {
            sh "cd ${env.PROJECT}Dmarc.DnsRecord.Importer.Lambda; ${env.DOTNET} lambda package --framework netcoreapp2.1 -c Release -o ${env.DOTNETPUBLISHSTASH}DnsRecordImporter.zip" 
        }
	}

// ---------------------Domain Status

    stage('Package Domain Status') {
	    if (env.DOTNETCOMPILE == "true") {
		    sh "cd ${env.PROJECT}Dmarc.DomainStatus.Api; ${env.DOTNET} publish -c Release -o ${env.DOTNETPUBLISHSTASH}DomainStatusApi" 
        }
	}

    stage('Docker Build Domain Status') {
        if (env.DOTNETCOMPILE == "true") {
		    sh "cp src/docker/microservice/Dockerfile ${env.DOTNETPUBLISHSTASH}DomainStatusApi/Dockerfile"
		    sh "cd ${env.DOTNETPUBLISHSTASH}DomainStatusApi;ls"
            sh "cd ${env.DOTNETPUBLISHSTASH}DomainStatusApi;docker build -t ${ENV_NAME}-domainstatusapi ."
		}
	}

// ---------------------record evaluator

    stage('Package Record Evaluator') {
	    if (env.DOTNETCOMPILE == "true") {
		    sh "cd ${env.PROJECT}Dmarc.DnsRecord.Evaluator; ${env.DOTNET} publish -c Release -o ${env.DOTNETPUBLISHSTASH}DmarcDnsRecordEvaluator" 
        }
	}

    stage('Docker Build Record Evaluator') {
        if (env.DOTNETCOMPILE == "true") {
		    sh "cp src/docker/microservice/Dockerfile ${env.DOTNETPUBLISHSTASH}DmarcDnsRecordEvaluator/Dockerfile"
		    sh "cd ${env.DOTNETPUBLISHSTASH}DmarcDnsRecordEvaluator;ls"
            sh "cd ${env.DOTNETPUBLISHSTASH}DmarcDnsRecordEvaluator;docker build -t ${ENV_NAME}-recordevaluator ."
		}
	}

// ---------------------admin api

    stage('Package admin API') {
	    if (env.DOTNETCOMPILE == "true") {
		    sh "cd ${env.PROJECT}Dmarc.Admin.Api; ${env.DOTNET} publish -c Release -o ${env.DOTNETPUBLISHSTASH}DmarcAdminApi" 
        }
	}

    stage('Docker Build Admin API') {
        if (env.DOTNETCOMPILE == "true") {
		    sh "cp src/docker/microservice/Dockerfile ${env.DOTNETPUBLISHSTASH}DmarcAdminApi/Dockerfile"
		    sh "cd ${env.DOTNETPUBLISHSTASH}DmarcAdminApi;ls"
            sh "cd ${env.DOTNETPUBLISHSTASH}DmarcAdminApi;docker build -t ${ENV_NAME}-adminapi ."
		}
	}

// ---------------------metrics api

    stage('Package metrics API') {
	    if (env.DOTNETCOMPILE == "true") {
		    sh "cd ${env.PROJECT}Dmarc.Metrics.Api; ${env.DOTNET} publish -c Release -o ${env.DOTNETPUBLISHSTASH}DmarcMetricsApi" 
        }
	}

    stage('Docker Build Metrics') {
        if (env.DOTNETCOMPILE == "true") {
		    sh "cp src/docker/microservice/Dockerfile ${env.DOTNETPUBLISHSTASH}DmarcMetricsApi/Dockerfile"
		    sh "cd ${env.DOTNETPUBLISHSTASH}DmarcMetricsApi;ls"
            sh "cd ${env.DOTNETPUBLISHSTASH}DmarcMetricsApi;docker build -t ${ENV_NAME}-metricsapi ."
		}
	}



//---------------------Compare angular app code hash to avoid recompiling unnecessarily
    stage('Compare Angular App') {
	    if (fileExists("${env.ANGULARAPPHASHFILE}") && fileExists("${env.ANGULARAPPSTASH}")) { 
		    env.PREVANGULARAPPHASH =  readFile("${env.ANGULARAPPHASHFILE}").trim()
		}
	    sh "find src/angular/dmarc-service -type f -print0 | sort -z | xargs -0 sha1sum | sha1sum |  cut -d \" \" -f1 | tee angularapphash"
	    env.ANGULARAPPHASH = readFile("angularapphash").trim()
	    if (env.PREVANGULARAPPHASH == env.ANGULARAPPHASH) {
		    env.ANGULARAPPCOMPILE = "false"
			echo "Previous Angular app code hash ${env.ANGULARAPPHASH} matches current commit. Skipping."   
	    } else {
		    env.ANGULARAPPCOMPILE = "true"
			echo "Previous Angular app code hash ${env.ANGULARAPPHASH} does not match ${env.PREVANGULARAPPHASH}. Recompiling....."
        }
	}

    stage('Angular App Build') {
	    if (env.ANGULARAPPCOMPILE == "true") {
			sh "cd src/angular/dmarc-service;export YARN_CACHE_FOLDER=/mnt/jenkins-home/yarn/${BRANCH_NAME}; ${env.YARN} --frozen-lockfile;${env.YARN} build"
//            sh "cd src/angular/dmarc-service; ${env.NPM} install"
//		    sh "${env.NPM} --version"
//		    sh "cd src/angular/dmarc-service; ${env.NPM} run build"
        }
	}
	
	stage('Angular App Test') {
        if (env.ANGULARAPPCOMPILE == "true") {
	        // sh "cd src/angular/dmarc-service; ${env.NPM} test"
			if (fileExists("${env.ANGULARAPPSTASH}")) {
				sh "rm -r ${env.ANGULARAPPSTASH};"
			}
			sh "mkdir -p ${env.ANGULARAPPSTASH}"
			sh "cp -r src/angular/dmarc-service/dist/* ${env.ANGULARAPPSTASH}"
		}
    }

//---------------------Compare angular app code hash to avoid recompiling unnecessarily
    stage('Compare React App') {
	    if (fileExists("${env.REACTAPPHASHFILE}") && fileExists("${env.REACTAPPSTASH}")) { 
		    env.PREVREACTAPPHASH =  readFile("${env.REACTAPPHASHFILE}").trim()
		}
	    sh "find src/react -type f -print0 | sort -z | xargs -0 sha1sum | sha1sum |  cut -d \" \" -f1 | tee reactapphash"
	    env.REACTAPPHASH = readFile("reactapphash").trim()
	    if (env.PREVREACTAPPHASH == env.REACTAPPHASH) {
		    env.REACTAPPCOMPILE = "false"
			echo "Previous React app code hash ${env.REACTAPPHASH} matches current commit. Skipping."   
	    } else {
		    env.REACTAPPCOMPILE = "true"
			echo "Previous React app code hash ${env.REACTAPPHASH} does not match ${env.PREVREACTAPPHASH}. Recompiling....."
			
			env.NODE_PATH= "src/"
			sh "cd src/react/ukncsc-mail-check-app; echo NODE_PATH=${env.NODE_PATH} > .env;echo REACT_APP_URL_ROUTE=/app >> .env;echo PUBLIC_URL=/app >> .env"
        }
	}
    stage('React App Test') {
		if (env.REACTAPPCOMPILE == "true") {

			env.NODE_PATH= "src/"
		    sh "cd src/react/ukncsc-mail-check-app;export CI=true;${env.YARN} --frozen-lockfile;${env.YARN} test"
		}
	}


    stage('React App Build') {
	    if (env.REACTAPPCOMPILE == "true") {
			if ("${BRANCH_NAME}" != "master") {   
				env.NODE_ENV="development"
			}
			sh "${env.AWS} s3 cp s3://ncsc-mailcheck-static-assets/HelveticaNeue.ttf src/react/ukncsc-semantic-ui-theme/src/themes/default/assets/fonts/"
			sh "cd src/react/ukncsc-semantic-ui-theme;${env.YARN} unlink || exit 0"
			sh "cd src/react/ukncsc-semantic-ui-theme;${env.YARN};${env.YARN} build;${env.YARN} link" 
			env.NODE_PATH= "src/"
			sh "cd src/react/ukncsc-mail-check-app;${env.YARN} unlink \"ukncsc-semantic-ui-theme\" || exit 0"
		    sh "cd src/react/ukncsc-mail-check-app;${env.YARN} link \"ukncsc-semantic-ui-theme\";${env.YARN} --frozen-lockfile;${env.YARN} build"
			if (fileExists("${env.REACTAPPSTASH}")) {
				sh "rm -r ${env.REACTAPPSTASH};"
			}
			sh "mkdir -p ${env.REACTAPPSTASH}"
			sh "cp -r src/react/ukncsc-mail-check-app/build/* ${env.REACTAPPSTASH}"
        }
	}
	

//------------------------------------DEPLOY SECTION - DO NOT MODIFY ENVIRONMENT BEFORE THIS POINT

 // ------------------ Frontend
 stage('Docker Build Frontend') {
         sh "mkdir -p frontend/public_html;cp -r src/docker/frontend/* frontend/"
	     sh "mkdir -p frontend/public_html/a;cp -r ${env.ANGULARAPPSTASH}* frontend/public_html/a/"
		 sh "mkdir -p frontend/public_html/app;cp -r ${env.REACTAPPSTASH}* frontend/public_html/app/"

 if (fileExists("${env.FRONTENDHASHFILE}") && fileExists("${env.FRONTEND_CONTAINER_GITHASH_FILE}")) { 
	 		    env.PREVFRONTENDHASH =  readFile("${env.FRONTENDHASHFILE}").trim()
		}
	    sh "find frontend -type f -print0 | sort -z | xargs -0 sha1sum | sha1sum |  cut -d \" \" -f1 | tee frontendhash"
	    env.FRONTENDHASH = readFile("frontendhash").trim()
	    if (env.PREVFRONTENDHASH == env.FRONTENDHASH) {
		    env.FRONTENDBUILD = "false"
			echo "Previous frontend hash ${env.FRONTENDHASH} matches current commit. Skipping."   
			env.FRONTEND_CONTAINER_GITHASH = readFile("${env.FRONTEND_CONTAINER_GITHASH_FILE}").trim()
	    } else {
		    env.FRONTENDBUILD = "true"
			echo "Previous frontend hash ${env.FRONTENDHASH} does not match ${env.PREVFRONTENDHASH}. Rebuilding....."
			env.FRONTEND_CONTAINER_GITHASH = env.GITSHORTHASH
			sh "cd frontend;find ."
            sh "cd frontend;docker build -t ${env.ENV_NAME}-frontend ."
        }

 }




// ------------------------Terraform Planning

    stage('TF Plan') {
                    
        //Remove the terraform state file so we always start from a clean state
        if (fileExists(".terraform")) {
            sh "rm -rf .terraform"
        }
        if (fileExists("status")) {
            sh "rm status"
        }
   
	env.APPLY = "false"
	if (fileExists("Terraform/prod-env/${env.TFVARS_FILE}")) {
		    sshagent(["${env.PRIVATE_SSH_DEPLOY_KEY_ID}"]) {
				// add some logic here for TOFU
				sh "mkdir -p ~/.ssh;ssh-keyscan github.com | tee -a ~/.ssh/known_hosts"
                sh "cd Terraform/prod-env/prod-env; ${env.TERRAFORM} init -backend=true -force-copy -input=false -backend-config=\"key=${env.STATE_KEY}/terraform.tfstate\""
			}
            sh "cd Terraform/prod-env/prod-env; cp ${env.DOTNETPUBLISHSTASH}*.zip . "
//          sh "${env.TERRAFORM} get Terraform/prod-env/prod-env"
            if (fileExists("${env.TF_PLAN_FILE}")) {
			  sh "rm ${env.TF_PLAN_FILE}"
			}
		    write_dynamic_tfvars("/tmp/${env.BUILD_NUMBER}.tfvars")
            sh "cd Terraform/prod-env/prod-env; set +e; ${env.TERRAFORM} plan -detailed-exitcode -refresh=true -out=${env.TF_PLAN_FILE} -var-file ../${env.TFVARS_FILE} -var-file /tmp/${env.BUILD_NUMBER}.tfvars   .; echo \$? > /tmp/status" 
            sh "rm /tmp/${env.BUILD_NUMBER}.tfvars"
		    def exitCode = readFile('/tmp/status').trim()
            echo "Terraform Plan Exit Code: ${exitCode}"
        if (exitCode == "0") {
            currentBuild.result = 'SUCCESS'
        }
        if (exitCode == "1") {     
            currentBuild.result = 'FAILURE'
			sh "rm -rf ${env.DOTNETBINARYSTASH}"
			sh "echo FORCE REBUILD > ${env.DOTNETHASHFILE}"
			error "Plan Failed: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
			
        }
        if (exitCode == "2") {
            echo "Plan Awaiting Approval: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
            try {
                if ("${BRANCH_NAME}" == "master") {    
                    input message: 'Apply Plan?', ok: 'Apply'
				}
                env.APPLY = "true"
                } catch (err) {
                    error "Plan Discarded: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
                    env.APPLY = "false"
                    currentBuild.result = 'UNSTABLE'
                }
            }
        }
	}
// Upload docker images to ECR
    stage('Docker to ECR') {

	    sh "eval `${env.AWS} ecr get-login --no-include-email --region ${env.AWSREGION}`"
		if (env.DOTNETCOMPILE == "true") {
			slackSend "Pushing kestrel containers with tag ${env.DOTNET_CONTAINER_GITHASH} - ${env.JOB_NAME} ${env.BUILD_NUMBER} (<${env.BUILD_URL}|Open>)"
			docker_push("aggregatereportapi",env.DOTNET_CONTAINER_GITHASH)
			docker_push("domainstatusapi",env.DOTNET_CONTAINER_GITHASH)
			docker_push("securitytester",env.DOTNET_CONTAINER_GITHASH)
			docker_push("securityevaluator",env.DOTNET_CONTAINER_GITHASH)
			docker_push("recordevaluator",env.DOTNET_CONTAINER_GITHASH)
			docker_push("adminapi",env.DOTNET_CONTAINER_GITHASH)
			docker_push("metricsapi",env.DOTNET_CONTAINER_GITHASH)

		}
		if (env.FRONTENDBUILD == "true") {
			slackSend "Pushing frontend containers with tag ${env.FRONTEND_CONTAINER_GITHASH} - ${env.JOB_NAME} ${env.BUILD_NUMBER} (<${env.BUILD_URL}|Open>)"

			docker_push("frontend",env.FRONTEND_CONTAINER_GITHASH)
		}

    }



    stage('TF Apply') {
	        
	        echo "Checking to see if plan was approved, or there are no changes to make..."
            if (env.APPLY == "true") {
	            echo "Applying the plan..."
                if (fileExists("status.apply")) {
                    sh "rm status.apply"
                }
                sh "cd Terraform/prod-env/prod-env; set +e; ${env.TERRAFORM} apply ${env.TF_PLAN_FILE}; echo \$? > /tmp/status.apply"
                def applyExitCode = readFile('/tmp/status.apply').trim()
                if (applyExitCode == "0") {
                    echo "Changes Applied ${env.JOB_NAME} - ${env.BUILD_NUMBER}"    
                } else {
                    error "Apply Failed: ${env.JOB_NAME} - ${env.BUILD_NUMBER} "
                    currentBuild.result = 'FAILURE'	
                } 
            }
    }

	stage('Database Schema') {
		def awsprofile = ""
		if (env.ROLE_TO_ASSUME != "") {
			awsprofile = "--profile assumerole"	
		}
        sh "/bin/bash ./apply_schema_changes  Terraform/prod-env/${env.TFVARS_FILE} src/sql ${env.MySQL} ${env.AWS} ${env.AWSREGION} ${awsprofile}"
	}
	
    stage('TF Apply (Common)') {
		def stagingBucketName = sh(returnStdout : true, script: 'cat Terraform/common/common.tfvars | grep staging-report-bucket | awk \'{print $3}\' FS=\'[=\"]\'').trim()
		def devBucketName = sh(returnStdout : true, script: 'cat Terraform/common/common.tfvars | grep dev-report-bucket | awk \'{print $3}\' FS=\'[=\"]\'').trim()
		echo "Report buckets Staging: ${stagingBucketName} Dev: ${devBucketName}"
	    if ("${BRANCH_NAME}" == "master") {
	        echo "Checking to see if plan was approved, or there are no changes to make...${env.apply}"
		if (env.APPLY == "true") {
	        echo "Applying the plan..."
                if (fileExists("status.apply")) {
                    sh "rm status.apply"
                }
                sh "set +e; cd Terraform/common/common; ${env.TERRAFORM} apply ${env.TF_COMMON_PLAN_FILE}; echo \$? > /tmp/status.apply"
                def applyExitCode = readFile('/tmp/status.apply').trim()
				
                if (applyExitCode == "0") {
					sh "${env.AWS}  s3api put-bucket-replication --bucket ${stagingBucketName} --replication-configuration  file://Terraform/common/staging-bucket-replication-policy.json"
					sh "${env.AWS}  s3api put-bucket-replication --bucket ${devBucketName} --replication-configuration  file://Terraform/common/dev-bucket-replication-policy.json"
                    echo "Changes Applied ${env.JOB_NAME} - ${env.BUILD_NUMBER}"    
                } else {
                    error "Apply Failed: ${env.JOB_NAME} - ${env.BUILD_NUMBER} "
                    currentBuild.result = 'FAILURE'
                }
			}
        } else {
		    echo "Not applying terraform outside the master branch"
		}
    }   
	

	stage('Code Release') {
        if ("${BRANCH_NAME}" == "master") { 
		    sh "mkdir -p -p ~/.ssh/"
	        sshagent(["${env.PRIVATE_SSH_DEPLOY_KEY_ID}"]) {
				try {
						input message: 'Get host key from github.com?', ok: 'Fetch'
						sh "ssh-keyscan github.com | tee -a ~/.ssh/known_hosts"
						} catch (err) {
						echo "skipping SSH host key fetch - will fail if this has not been done previously"
					}
				sh "git clone git@github.com:ukncsc/dmarc-processing.git private-repo"
				sshagent(["${env.PUBLIC_SSH_DEPLOY_KEY_ID}"]) {	
					sh "git clone git@github.com:ukncsc/mail-check.git public-repo"
					env.RELEASE = readFile('RELEASE').trim()
					if (fileExists("public-repo/RELEASE")) {
						env.CURRENTRELEASE = readFile('public-repo/RELEASE').trim()
					} else {
						env.CURRENTRELEASE = "NONE"
					}
					echo "Private release version: ${env.RELEASE}"
					echo "Public release version: ${env.CURRENTRELEASE}"

					if ("${env.RELEASE}" != "${env.CURRENTRELEASE}") {
				
						echo "Commiting files in the manifest to the public repo"
						sh "git config --global user.name \'NCSC Pipeline\'"
						sh "git config --global user.email \'ncsc@git\'"
						sh "rsync -av --delete --recursive --exclude=\'.*\' --exclude=\'*.ps1\' --exclude=\'.*/\' --files-from=./MANIFEST ./private-repo/ ./public-repo/"
						try {
							input message: 'Publish release to public repository?', ok: 'Apply'
							sh "cd public-repo; git add .; git commit -m \'Release ${RELEASE}\'; git push"
						} catch (err) {
							error "Release to public repository cancelled: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
							currentBuild.result = 'UNSTABLE'
						}
							
					}
				
				}
			}
		}
	}
    stage('Store code hashes') {
	    // Storing code hashes in jenkins home after a successful build
	    sh "echo ${env.DOTNETHASH} > ${env.DOTNETHASHFILE}"
        sh "echo ${env.ANGULARAPPHASH} > ${env.ANGULARAPPHASHFILE}"
        sh "echo ${env.ANGULARPUBLICHASH} > ${env.ANGULARPUBLICHASHFILE}"
		sh "echo ${env.FRONTENDHASH} > ${env.FRONTENDHASHFILE}"
		sh "echo ${env.REACTAPPHASH} > ${env.REACTAPPHASHFILE}"
		sh "echo ${env.DOTNET_CONTAINER_GITHASH} > ${env.DOTNET_CONTAINER_GITHASH_FILE}"  
		sh "echo ${env.FRONTEND_CONTAINER_GITHASH} > ${env.FRONTEND_CONTAINER_GITHASH_FILE}"  
		slackSend "Build Completed - ${env.JOB_NAME} ${env.BUILD_NUMBER} (<${env.BUILD_URL}|Open>)"
	}
}

void gitClean() {
    timeout(time: 300, unit: 'SECONDS') {
        if (fileExists('.git')) {
            echo 'Found Git repository: using Git to clean the tree.'
            // The sequence of reset --hard and clean -fdx first
            // in the root and then using submodule foreach
            // is based on how the Jenkins Git SCM clean before checkout
            // feature works.
            sh 'git reset --hard'
            // Note: -e is necessary to exclude the temp directory
            // .jenkins-XXXXX in the workspace where Pipeline puts the
            // batch file for the 'bat' command.
            // TODO: don't delete .venv
            sh 'git clean -ffdx -e ".venv/ .jenkins-*/"'
            sh 'git submodule foreach --recursive git reset --hard'
            sh 'git submodule foreach --recursive git clean -ffdx'
        }
        else
        {
            echo 'No Git repository found: using deleteDir() to wipe clean'
            deleteDir()
        }
    }
}

String commit_sha() {
    sh 'git rev-parse --short HEAD | tee .out'
    def commit_sha = readFile('.out').trim()
    commit_sha ? commit_sha : "unknown"
}

String branch_name_short() {
    sh 'echo $BRANCH_NAME | head -c3 | tee .out'
    def branch_name_short = readFile('.out').trim()
    branch_name_short ? branch_name_short : "unknown"
}

void install_aws() {
	
   //-----------------Check AWS version and install
   if (fileExists("${env.AWS}")) {
	    sh "${env.AWS} --version  2>&1 > /dev/null | sed \'s/ .*//\' | tr -d \'/.A-Za-z\\-\' | tee version"
	    env.AWSCURRENTVERSION = readFile('version').trim()
		} else {
        env.AWSCURRENTVERSION = "not installed"
		}
	if ("${env.AWSCURRENTVERSION}" < "${env.AWSVERSION}") {
        echo "Installing AWS CLI..."
		sh "rm -rf ${env.AWSPATH}; mkdir -p ${env.AWSPATH}"
		sh "wget -O awscli-bundle.zip ${env.AWSURL}"
        sh "unzip awscli-bundle.zip"
        sh "./awscli-bundle/install -i ${env.AWSPATH}"
		}
	if (fileExists("${env.AWS}")) {
	    sh "${env.AWS} --version  2>&1 > /dev/null | sed \'s/ .*//\' | tr -d \'/.A-Za-z\\-\' | tee version"
	    env.AWSCURRENTVERSION = readFile('version').trim()
		echo "AWS CLI version: ${env.AWSCURRENTVERSION}"
		} else {
        currentBuild.result = 'FAILURE'
		echo "AWS CLI installation has failed!"
		}
	if ("${env.AWSCURRENTVERSION}" < "${env.AWSVERSION}") {
	    currentBuild.result = 'FAILURE'
	    error "AWS CLI installation version ${env.AWSCURRENTVERSION} does not match required version of ${env.AWSVERSION}!"
	    
	}
}
void install_mysql() {
	//-----------------Check MySQL version and install

  if (fileExists("${env.MySQL}")) {
	    sh "${env.MySQL} --version | grep -E -o '[0-9]+\\.[0-9]+\\.[0-9]+' | tee version"
	    env.MySQLCURRENTVERSION = readFile('version').trim()
		} else {
        env.MySQLCURRENTVERSION = "not installed"
		}
	if ("${env.MySQLCURRENTVERSION}" != "${env.MySQLVERSION}") {
        echo "Installing MySQL Client..."
		sh "rm -rf ${env.MySQLPATH}; mkdir -p ${env.MySQLPATH}"
		sh "wget -O mysql-installer.tar.gz https://dev.mysql.com/get/Downloads/MySQL-5.6/mysql-5.6.35-linux-glibc2.5-x86_64.tar.gz"
        sh "tar -xvzf mysql-installer.tar.gz --no-anchored --strip-components 2 -C ${env.MySQLPATH} bin/mysql "
		}
	if (fileExists("${env.MySQL}")) {
	    sh "${env.MySQL} --version | grep -E -o '[0-9]+\\.[0-9]+\\.[0-9]+' | tee version"
	    env.MySQLCURRENTVERSION = readFile('version').trim()
		echo "MySQL version: ${env.MySQLCURRENTVERSION}"
		} else {
        currentBuild.result = 'FAILURE'
		echo "MySQL installation has failed!"
		}
	if ("${env.MySQLCURRENTVERSION}" != "${env.MySQLVERSION}") {
	    currentBuild.result = 'FAILURE'
	    error "MySQL installation version ${env.MySQLCURRENTVERSION} does not match required version of ${env.MySQLVERSION}!"
	    
	}
}

void install_node() {
	
//-----------------Check Node version and install
    if (fileExists("${env.NODE}")) {
	    sh "${env.NODE} --version >version"
	    env.NODECURRENTVERSION = readFile('version').trim()
		} else {
        env.NODECURRENTVERSION = "not installed"
		}
	if ("${env.NODECURRENTVERSION}" != "${env.NODEVERSION}") {
        echo "Installing Node and NPM..."
        sh "rm -rf ${env.NODEPATH}; mkdir -p ${env.NODEPATH}"
	    sh "wget -O node.tar.xz https://nodejs.org/dist/v8.11.2/node-v8.11.2-linux-x64.tar.xz"
	    sh "tar -xvf node.tar.xz -C ${env.NODEPATH}"
        }
	if (fileExists("${env.NODE}")) {
	    sh "${env.NODE} --version >version"
	    env.NODECURRENTVERSION = readFile('version').trim()
		echo "Node version: ${env.NODECURRENTVERSION}"
		} else {
        currentBuild.result = 'FAILURE'
		echo "Node installation has failed!"
		}
	if ("${env.NODECURRENTVERSION}" != "${env.NODEVERSION}") {
	    currentBuild.result = 'FAILURE'
	    error "Node installation version ${env.NODECURRENTVERSION} does not match required version of ${env.NODEVERSION}!"
	   
	}
}

void install_angular() {
	
//----------- Check version of angular cli and install
     if (fileExists("${env.NG}")) { 
	    sh "${env.NG} --version | grep @angular/cli | tee version"
	    env.NGCURRENTVERSION = readFile('version').trim()
		} else {
        env.NGCURRENTVERSION = "not installed"
		}
	if ("${env.NGCURRENTVERSION}" != "${env.NGVERSION}") {
        echo "Installing Angular CLI..."
        sh "rm -rf ${env.NGPATH}; mkdir -p ${env.NGPATH}"
	    sh "${env.NPM} install --prefix ${env.NGPATH} @angular/cli@1.0.0 -g"
        }
	if (fileExists("${env.NG}")) {
	    sh "${env.NG} --version | grep @angular/cli >version"
	    env.NGCURRENTVERSION = readFile('version').trim()
		echo "NG version: ${env.NGCURRENTVERSION}"
		} else {
        currentBuild.result = 'FAILURE'
		echo "NG installation has failed!"
		}
	if ("${env.NGCURRENTVERSION}" != "${env.NGVERSION}") {
	    currentBuild.result = 'FAILURE'
	    error "NG installation version ${env.NGCURRENTVERSION} does not match required version of ${env.NGVERSION}!"
	    
        }
}

void install_yarn() {
	
		//----------- Check version of yarn and install
     if (fileExists("${env.YARN}")) { 
	    sh "${env.YARN} --version | tee version"
	    env.YARNCURRENTVERSION = readFile('version').trim()
		} else {
        env.YARNCURRENTVERSION = "not installed"
		}
	if ("${env.YARNCURRENTVERSION}" != "${env.YARNVERSION}") {
        echo "Installing Yarn..."
        sh "rm -rf ${env.YARNPATH}; mkdir -p ${env.YARNPATH}"
	    sh "wget -O yarn.tar.gz ${env.YARNURL}"
	    sh "tar -xvzf yarn.tar.gz -C ${env.YARNPATH}"
        }
	if (fileExists("${env.YARN}")) {
	    sh "${env.YARN} --version >version"
	    env.YARNCURRENTVERSION = readFile('version').trim()
		echo "Yarn version: ${env.YARNCURRENTVERSION}"
		} else {
        currentBuild.result = 'FAILURE'
		echo "Yarn installation has failed!"
		}
	if ("${env.YARNCURRENTVERSION}" != "${env.YARNVERSION}") {
	    currentBuild.result = 'FAILURE'
	    error "Yarn installation version ${env.YARNCURRENTVERSION} does not match required version of ${env.YARNVERSION}!"
        }
}


void install_terraform() {
//----------- Check version of terraform and install
     if (fileExists("${env.TERRAFORM}")) {
	    sh "${env.TERRAFORM} --version | grep -E -o 'v[0-9\\.]+' | tee version"
	    env.TERRAFORMCURRENTVERSION = readFile('version').trim()
		} else {
        env.TERRAFORMCURRENTVERSION = "not installed"
		}
	if ("${env.TERRAFORMCURRENTVERSION}" != "v${env.TERRAFORMVERSION}") {
        echo "Installing Terraform because current version is ${env.TERRAFORMCURRENTVERSION}...."
        sh "rm -rf ${env.TERRAFORMPATH}; mkdir -p ${env.TERRAFORMPATH}"
	    sh "wget -O terraform.zip ${env.TERRAFORMURL}"
	    sh "unzip -d ${env.TERRAFORMPATH} terraform.zip"
        }
	if (fileExists("${env.TERRAFORM}")) {
	    sh "${env.TERRAFORM} --version | grep -E -o 'v[0-9\\.]+' | tee version"
 		env.TERRAFORMCURRENTVERSION = readFile('version').trim()
		} else {
        currentBuild.result = 'FAILURE'
		error "Terraform installation has failed!"
		}
	if ("${env.TERRAFORMCURRENTVERSION}" != "v${env.TERRAFORMVERSION}") {
	    currentBuild.result = 'FAILURE'
	    error "Terraform installation version ${env.TERRAFORMCURRENTVERSION} does not match required version of v${env.TERRAFORMVERSION}!"
	    
        }
}

def install_dotnet(dotnetPathBase, dotnetSdkUrl){
	def dotnetSdkHash = sh(returnStdout : true, script: "echo ${dotnetSdkUrl} | md5sum | cut -d ' ' -f 1").trim()

    def dotnetDir = env.JENKINSHOME + "/" +dotnetPathBase + "/" + dotnetSdkHash

	def dotnet = "${dotnetDir}" + "/" + "dotnet"
	
	if(fileExists("${dotnet}"))
	{
		def dotnetVersion = sh(returnStdout : true, script: "${dotnet} --version").trim()

		echo "Current version ${dotnetVersion} of dotnet core up to date."
	}else{
		echo "Didnt find dotnet core installed. Installing from ${dotnetSdkUrl}"

		sh "mkdir -p ${dotnetDir}"

		def dotnetTar = "${dotnetDir}" + "/" + "dotnet-core-sdk.tar.gz"

		sh "wget -O ${dotnetTar} ${dotnetSdkUrl}"

		sh "tar -xvf ${dotnetTar} -C ${dotnetDir}"

		sh "rm -rf ${dotnetTar}"

		def dotnetVersion = sh(returnStdout : true, script: "${dotnet} --version").trim()

		echo "Installed version ${dotnetVersion} of dotnet core."
	}
	return dotnet
}


  void docker_push(String serviceName,String tag) {
	  sh "docker tag  ${env.ENV_NAME}-${serviceName} ${env.ecrAwsAccount}.dkr.ecr.${env.AWSREGION}.amazonaws.com/${env.ENV_NAME}/${serviceName}:${tag}" 	
	  sh "docker push ${env.ecrAwsAccount}.dkr.ecr.${env.AWSREGION}.amazonaws.com/${env.ENV_NAME}/${serviceName}:${tag}" 
  }

void write_dynamic_tfvars(String tfvarsFile) {
	sh "echo dotnet-container-githash=\\\"${env.DOTNET_CONTAINER_GITHASH}\\\" | tee ${tfvarsFile}"
	sh "echo frontend-container-githash=\\\"${env.FRONTEND_CONTAINER_GITHASH}\\\" | tee -a ${tfvarsFile}"
	sh "echo allowed-account-ids=\\\"${env.AWSACCOUNT}\\\" | tee -a ${tfvarsFile}"
	sh "echo build-account-id=\\\"${env.buildAwsAccount}\\\" | tee -a ${tfvarsFile}"
	sh "echo ecr-aws-account-id=\\\"${env.ecrAwsAccount}\\\" | tee -a ${tfvarsFile}"
	sh "echo aws-account-id=\\\"${env.AWSACCOUNT}\\\" | tee -a ${tfvarsFile}"
}

void write_aws_config(String awsConfigFile) {
	env.AWS_CONFIG_FILE=awsConfigFile
//	sh "echo [default] | tee ${awsConfigFile}"
	sh "echo [profile assumerole] | tee -a ${awsConfigFile}"
	sh "echo role_arn=${env.ROLE_TO_ASSUME} | tee -a ${awsConfigFile}"
	sh "echo credential_source = Ec2InstanceMetadata | tee -a ${awsConfigFile}"
}