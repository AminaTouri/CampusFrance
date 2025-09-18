pipeline {
    agent any

    tools {
        dotnetsdk 'dotnet8'
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/AminaTouri/CampusFrance.git'
            }
        }

        stage('Restore & Build') {
            steps {
                bat 'dotnet restore'
                bat 'dotnet build --no-restore'
            }
        }

        stage('Run Tests') {
            steps {
                catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
                    bat 'dotnet test --no-build --logger "trx;LogFileName=TestResults.trx"'
                }
            }
        }

        stage('Generate HTML Report') {
            steps {
                bat '''
                    dotnet tool install --global dotnet-reportgenerator-globaltool
                    set PATH=%PATH%;%USERPROFILE%\\.dotnet\\tools
                    reportgenerator -reports:**/TestResults.trx -targetdir:TestReport -reporttypes:Html
                '''
            }
        }
    }

    post {
        always {
            publishHTML(target: [
                reportName: 'Rapport des tests automatisés',
                reportDir: 'TestReport',
                reportFiles: 'index.html',
                alwaysLinkToLastBuild: true,
                keepAll: true,
                allowMissing: true
            ])
        }
    }
}
