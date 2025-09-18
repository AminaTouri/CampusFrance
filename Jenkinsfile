pipeline {
    agent any

    tools {
        dotnetsdk 'dotnet8'  // Nom que tu as donné dans Jenkins → Géré dans "Global Tool Configuration"
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
                bat 'dotnet test --no-build --logger "trx;LogFileName=TestResults.trx"'
            }
        }

        stage('Generate HTML Report') {
            steps {
                // Génère le rapport HTML à partir du .trx avec ReportGenerator
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
            // Publier le rapport HTML dans Jenkins
            publishHTML(target: [
                reportName: 'Rapport des tests automatisés',
                reportDir: 'TestReport',          // Dossier généré par ReportGenerator
                reportFiles: 'index.html',        // Fichier principal du rapport
                alwaysLinkToLastBuild: true,
                keepAll: true,
                allowMissing: false
            ])
        }
    }
}
