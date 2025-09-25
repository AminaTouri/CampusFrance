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

        stage('Run Tests with Coverage') {
            steps {
                catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
                    bat '''
                        dotnet test CampusFrance/CampusFrance.Test.csproj --no-build ^
                        --logger "trx;LogFileName=TestResults.trx" ^
                        /p:CollectCoverage=true ^
                        /p:CoverletOutputFormat=cobertura ^
                        /p:CoverletOutput=../TestResults/coverage.xml
                    '''
                }
            }
        }

        stage('Generate HTML Report') {
            steps {
                bat '''
                    dotnet tool install --global dotnet-reportgenerator-globaltool
                    set PATH=%PATH%;%USERPROFILE%\\.dotnet\\tools
                    reportgenerator ^
                        -reports:TestResults\\coverage.xml ^
                        -targetdir:TestReport ^
                        -reporttypes:Html
                    dir TestReport
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
