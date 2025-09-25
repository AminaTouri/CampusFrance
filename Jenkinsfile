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
                catchError(buildResult: 'SUCCESS', stageResult: 'FAILURE') {
                    bat """
                        dotnet test CampusFrance/CampusFrance.Test.csproj --no-build ^
                            --logger "trx;LogFileName=TestResults.trx" ^
                            /p:CollectCoverage=true ^
                            /p:CoverletOutputFormat=cobertura ^
                            /p:CoverletOutput=CampusFrance\\TestResults\\ ^
                            /p:Include="[*]*"
                    """
                }
            }
        }

        stage('Generate HTML Report') {
            steps {
                bat 'dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools'
                bat '''
                    tools\\reportgenerator ^
                        -reports:CampusFrance\\TestResults\\coverage.cobertura.xml ^
                        -targetdir:TestReport ^
                        -reporttypes:Html
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
