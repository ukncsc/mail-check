node {
    stage('Checkout') {

//---------------- Variables/Definitions
        env.WORKSPACE = pwd()
	env.DOTNETPATH = "/mnt/jenkins-home/dotnet"
	env.DOTNET = "${env.DOTNETPATH}/dotnet"
	env.TERRAFORMPATH = "/mnt/jenkins-home/terraform"
	env.TERRAFORM ="${env.TERRAFORMPATH}/terraform"
	env.TERRAFORMVERSION = "Terraform v0.8.6"
	env.NODEPATH ="/mnt/jenkins-home/node"
	
	env.NODEVERSION = "v6.9.4"
    env.NODE = "${env.NODEPATH}/node-${env.NODEVERSION}-linux-x64/bin/node"
	env.NPM  = "${env.NODEPATH}/node-${env.NODEVERSION}-linux-x64/bin/npm"
    env.NGPATH = "/mnt/jenkins-home/ng"
	env.NG = "${env.NGPATH}/lib/node_modules/angular-cli/bin/ng"
	env.NGVERSION = "angular-cli: 1.0.0-beta.26"
	env.AWSPATH = "/mnt/jenkins-home/aws"
	env.AWS = "${env.AWSPATH}/bin/aws"
	env.AWSVERSION = "aws-cli/1.11.44 Python/2.7.9 Linux/4.4.35-33.55.amzn1.x86_64 botocore/1.5.7"
	env.PATH = "${env.NODEPATH}/node-${env.NODEVERSION}-linux-x64/bin/:${env.PATH}" 
	env.DOTNETHASHFILE = "/mnt/jenkins_home/${env.JOB_NAME}-dotnet-code-hash"
    env.SSH_DEPLOY_KEY_ID = readFile('/mnt/jenkins-home/ssh-deploy-key-id').trim()
	env.APP_S3_BUCKET = readFile('/mnt/jenkins-home/app-s3-bucket').trim()

//-----------------Checkout
        gitClean()
        checkout scm
	echo "Checkout complete..."
//-----------------Check AWS version and install
   }
   stage('Tools') {
   if (fileExists("${env.AWS}")) {
	    sh "${env.AWS} --version  2>&1 > /dev/null | tee version"
	    env.AWSCURRENTVERSION = readFile('version').trim()
		} else {
        env.AWSCURRENTVERSION = "not installed"
		}
	if ("${env.AWSCURRENTVERSION}" != "${env.AWSVERSION}") {
        echo "Installing AWS CKI..."
		sh "rm -rf ${env.AWSPATH}; mkdir ${env.AWSPATH}"
		sh "wget -O awscli-bundle.zip https://s3.amazonaws.com/aws-cli/awscli-bundle.zip"
        sh "unzip awscli-bundle.zip"
        sh "./awscli-bundle/install -i ${env.AWSPATH}"
		}
	if (fileExists("${env.AWS}")) {
	    sh "${env.AWS} --version  2>&1 > /dev/null | tee version"
	    env.AWSCURRENTVERSION = readFile('version').trim()
		echo "AWS CLI version: ${env.AWSCURRENTVERSION}"
		} else {
        currentBuild.result = 'FAILURE'
		echo "AWS CLI installation has failed!"
		}
	if ("${env.AWSCURRENTVERSION}" != "${env.AWSVERSION}") {
	    currentBuild.result = 'FAILURE'
	    error "AWS CLI installation version ${env.AWSCURRENTVERSION} does not match required version of ${env.AWSVERSION}!"
	    
	}



//-----------------Check Node version and install
    if (fileExists("${env.NODE}")) {
	    sh "${env.NODE} --version >version"
	    env.NODECURRENTVERSION = readFile('version').trim()
		} else {
        env.NODECURRENTVERSION = "not installed"
		}
	if ("${env.NODECURRENTVERSION}" != "${env.NODEVERSION}") {
        echo "Installing Node and NPM..."
        sh "rm -rf ${env.NODEPATH}; mkdir ${env.NODEPATH}"
	    sh "wget -O node.tar.xz https://nodejs.org/dist/v6.9.4/node-v6.9.4-linux-x64.tar.xz"
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

//----------- Check version of angular cli and install
     if (fileExists("${env.NG}")) { 
	    sh "${env.NG} --version | grep angular-cli | tee version"
	    env.NGCURRENTVERSION = readFile('version').trim()
		} else {
        env.NGCURRENTVERSION = "not installed"
		}
	if ("${env.NGCURRENTVERSION}" != "${env.NGVERSION}") {
        echo "Installing Angular CLI..."
        sh "rm -rf ${env.NGPATH}; mkdir ${env.NGPATH}"
	    sh "${env.NPM} install --prefix ${env.NGPATH} angular-cli -g"
        }
	if (fileExists("${env.NG}")) {
	    sh "${env.NG} --version | grep angular-cli >version"
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

//----------- Check version of terraform and install
     if (fileExists("${env.TERRAFORM}")) {
	    sh "${env.TERRAFORM} --version >version"
	    env.TERRAFORMCURRENTVERSION = readFile('version').trim()
		} else {
        env.TERRAFORMCURRENTVERSION = "not installed"
		}
	if ("${env.TERRAFORMCURRENTVERSION}" != "${env.TERRAFORMVERSION}") {
        echo "Installing Terraform because current version is ${env.TERRAFORMCURRENTVERSION}...."
        sh "rm -rf ${env.TERRAFORMPATH}; mkdir ${env.TERRAFORMPATH}"
	    sh "wget -O terraform.zip https://releases.hashicorp.com/terraform/0.8.6/terraform_0.8.6_linux_amd64.zip"
	    sh "unzip -d ${env.TERRAFORMPATH} terraform.zip"
        }
	if (fileExists("${env.TERRAFORM}")) {
	    sh "${env.TERRAFORM} --version >version" 
		env.TERRAFORMCURRENTVERSION = readFile('version').trim()
		} else {
        currentBuild.result = 'FAILURE'
		error "Terraform installation has failed!"
		}
	if ("${env.TERRAFORMCURRENTVERSION}" != "${env.TERRAFORMVERSION}") {
	    currentBuild.result = 'FAILURE'
	    error "Terraform installation version ${env.NGCURRENTVERSION} does not match required version of ${env.NGVERSION}!"
	    
        }


//----------- Dotnet install



	// If dotnet isn't installed do this now
	if (!fileExists("${env.DOTNET}")) {
	    echo "Installing .net CLI"
	    sh "rm -rf ${env.DOTNETPATH}; mkdir ${env.DOTNETPATH}"
	    sh "wget -O dotnet.tar.gz https://go.microsoft.com/fwlink/?LinkID=836302";
	    sh "tar -xvf dotnet.tar.gz -C ${env.DOTNETPATH}"
	    }
	//output dotnet version
	sh "${env.DOTNET} --version"

	
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
	sh "${env.TERRAFORM} remote config \
        -backend=s3 \
       	-backend-config='bucket=ncscdmarc-terraform-state' \
    	-backend-config='key=common/terraform.tfstate' \
    	-backend-config='region=eu-west-2'"
    sh "${env.TERRAFORM} get Terraform/common/common"
        sh "set +e; ${env.TERRAFORM} plan -detailed-exitcode -out=plan.out -var-file Terraform/common/common.tfvars Terraform/common/common; echo \$? > status"
        def exitCode = readFile('status').trim()
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
            stash name: "plan", includes: "plan.out"
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
    stage('TF Apply (Common)') {
	echo "Checking to see if plan was approved, or there are no changes to make...${env.apply}"
    
		if (env.APPLY == "true") {
	    echo "Applying the plan..."
            unstash 'plan'
            if (fileExists("status.apply")) {
                sh "rm status.apply"
            }
            sh "set +e; ${env.TERRAFORM} apply plan.out; echo \$? > status.apply"
            def applyExitCode = readFile('status.apply').trim()
            if (applyExitCode == "0") {
                echo "Changes Applied ${env.JOB_NAME} - ${env.BUILD_NUMBER}"    
            } else {
                error "Apply Failed: ${env.JOB_NAME} - ${env.BUILD_NUMBER} "
                currentBuild.result = 'FAILURE'

            }
        }
    }    
    stage('Compare Dotnet') {
	    if (fileExists("${env.DOTNETHASHFILE}")) { 
		    env.PREVDOTNETHASH =  readFile("${env.DOTNETHASHFILE}").trim()
		}
	    sh "find src/dotnet -type f -print0 | sort -z | xargs -0 sha1sum | sha1sum | tee dotnethash"
	    env.DOTNETHASH = readFile("dotnethash").trim()
	    if ("${env.PREVDOTNETHASH}" != "${env.DOTNETHASH}") {
		    env.DOTNETCOMPILE = "true"
			echo "Previous dotnet code hash ${env.PREVDOTNETHASH} does not match ${env.DOTNETHASH}. Recompiling....." 
	    } else {
            env.DOTNETCOMPILE = "false"
			echo "Previous dotnet code hash ${env.PREVDOTNETHASH} matches current commit. Skipping." 
	    }
	}

// ---------------------AggregateReportApi

	stage('Build AggregateReportApi') {
	    if (env.DOTNETCOMPILE == "true") {
	        env.PROJECT = "src/dotnet/Dmarc/src/Dmarc.AggregateReport.Api"
            sh "${env.DOTNET} restore"
            sh "rm -rf /tmp/dotnet"
            sh "mkdir /tmp/dotnet"	
            sh "${env.DOTNET} build -b /tmp/dotnet ${env.PROJECT}"
        }
    }

    stage('Unit Tests AggregateReportApi') {
        if (env.DOTNETCOMPILE == "true") {
            sh "${env.DOTNET} test -b /tmp/dotnet ${env.PROJECT}.Test"
        }
    }

    stage('Publish AggregateReportApi') {
	    if (env.DOTNETCOMPILE == "true") {
            sh "rm -rf publish"
            sh "mkdir -p publish"
            sh "${env.DOTNET} publish -b /tmp/dotnet -o publish ${env.PROJECT}"
            sh "cd publish; zip -r ../AggregateReportApi.zip ."
		}
    }

// ---------------------AggregateReportParser

    stage('Build AggregateReportParser') {
	    if (env.DOTNETCOMPILE == "true") {
            env.PROJECT = "src/dotnet/Dmarc/src/Dmarc.AggregateReport.Parser.Lambda"
            sh "${env.DOTNET} restore"
            sh "rm -rf /tmp/dotnet"
            sh "mkdir /tmp/dotnet"	
            sh "${env.DOTNET} build -b /tmp/dotnet ${env.PROJECT}"
        }
    }

    stage('Unit Tests AggregateReportParser') {
	    if (env.DOTNETCOMPILE == "true") {
            sh "${env.DOTNET} test -b /tmp/dotnet ${env.PROJECT}.Test"
		}
    }

    stage('Publish AggregateReportParser') {
	    if (env.DOTNETCOMPILE == "true") {
            sh "rm -rf publish"
            sh "mkdir -p publish"
            sh "${env.DOTNET} publish -b /tmp/dotnet -o publish ${env.PROJECT}"
            sh "cd publish; zip -r ../AggregateReportParser.zip ."
        }
	}

// ------------------------Terraform

    stage('TF Plan (Staging)') {
                    
        //Remove the terraform state file so we always start from a clean state
        if (fileExists(".terraform/terraform.tfstate")) {
            sh "rm -rf .terraform/terraform.tfstate"
        }
        if (fileExists("status")) {
            sh "rm status"
        }
	sh "${env.TERRAFORM} remote config \
        -backend=s3 \
       	-backend-config='bucket=ncscdmarc-terraform-state' \
    	-backend-config='key=DublinTest/terraform.tfstate' \
    	-backend-config='region=eu-west-2'"
//	sh "${env.TERRAFORM} taint -allow-missing aws_api_gateway_resource.aggregate-api-resource"

    sh "${env.TERRAFORM} get Terraform/prod-env/prod-env"
        sh "set +e; ${env.TERRAFORM} plan -detailed-exitcode -refresh=true -out=plan.out -var-file Terraform/prod-env/staging.tfvars Terraform/prod-env/prod-env; echo \$? > status"
        def exitCode = readFile('status').trim()
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
            stash name: "plan", includes: "plan.out"
            echo "Plan Awaiting Approval: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
            try {
//                input message: 'Apply Plan?', ok: 'Apply'
                env.APPLY = "true"
            } catch (err) {
                error "Plan Discarded: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
                env.APPLY = "false"
                currentBuild.result = 'UNSTABLE'
			
            }
        }
    }
    stage('TF Apply (Staging)') {
	echo "Checking to see if plan was approved, or there are no changes to make..."
        if (env.APPLY == "true") {
	    echo "Applying the plan..."
            unstash 'plan'
            if (fileExists("status.apply")) {
                sh "rm status.apply"
            }
            sh "set +e; ${env.TERRAFORM} apply plan.out; echo \$? > status.apply"
            def applyExitCode = readFile('status.apply').trim()
            if (applyExitCode == "0") {
                echo "Changes Applied ${env.JOB_NAME} - ${env.BUILD_NUMBER}"    
            } else {
                error "Apply Failed: ${env.JOB_NAME} - ${env.BUILD_NUMBER} "
                currentBuild.result = 'FAILURE'
			
            }
        }
    }
    stage('Angular Build') {
	
          sh "cd src/angular/dmarc-service; ${env.NPM} install"
		  sh "${env.NG} --version"
		  sh "cd src/angular/dmarc-service; ${env.NG} build --prod --aot"
    }

    stage('Angular Test') {
 //     sh "cd src/angular/dmarc-service; ${env.NPM} run test"
 //     junit 'target/test-reports/TEST-*.xml'
    }

    stage('Angular Publish') {
        sh "${env.AWS} s3 cp src/angular/dmarc-service/dist s3://${env.APP_S3_BUCKET}/ --recursive --exclude \".git/*\""
    }

    stage('System Tests') {
	echo "Performing system tests on test environment..."
    }
    stage('Security Tests') {
	echo "Checking security parameters..."
    }
    stage('TF Plan (Prod)') {
	echo "Planning terraform changes to production environment..."
    }
    stage('TF Apply (Prod)') {
	echo "Applying changes to production environment..."
    }
	stage('Code Release') {
		sh "mkdir -p ~/.ssh/"
	   
	    sshagent(["${env.SSH_DEPLOY_KEY_ID}"]) {
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
				try {
					input message: 'Get host key from github.com?', ok: 'Fetch'
					sh "ssh-keyscan github.com | tee -a ~/.ssh/known_hosts"
					} catch (err) {
					echo "skipping SSH host key fetch - will fail if this has not been done previously"
					}
				try {
					input message: 'Publish release to public repository?', ok: 'Apply'
					env.RELEASEAPPROVED = "true"
					} catch (err) {
					error "Release to public repository cancelled: ${env.JOB_NAME} - ${env.BUILD_NUMBER}"
					env.RELEASEAPPROVED = "false"
					currentBuild.result = 'UNSTABLE'
				}
				if ("${env.RELEASEAPPROVED}" == "true") {
					echo "Commiting files in the manifest to the public repo"
					sh "git config --global user.name \'NCSC Pipeline\'"
					sh "git config --global user.email \'ncsc@git\'"

					sh "rsync -av --delete --recursive --files-from=./MANIFEST ./ ./public-repo/"
					sh "cd public-repo; git add .; git commit -m \'Release ${RELEASE}\'; git push"
				}
			}
		}
	}
    stage('Store code hashes') {
	    // Storing code hashes in jenkins home after a successful build
	    sh "echo ${env.DOTNETHASH} > ${env.DOTNETHASHFILE}"
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
