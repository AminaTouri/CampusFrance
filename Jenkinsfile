pipeline {
    agent any // Utilise n’importe quel agent Jenkins disponible

    tools {
        dotnetsdk 'dotnet8' // Déclare l’utilisation du SDK .NET 8 installé dans Jenkins
    }

    stages {
        stage('Checkout') { // Étape 1 : Récupérer le code source depuis GitHub
            steps {
                git branch: 'main', url: 'https://github.com/AminaTouri/CampusFrance.git'
            }
        }

        stage('Restore & Build') { // Étape 2 : Restaurer les packages et compiler le projet
            steps {
                bat 'dotnet restore' // Restaure les dépendances NuGet
                bat 'dotnet build --no-restore' // Compile les projets .NET (sans refaire restore)
            }
        }

        stage('Run Tests') { // Étape 3 : Exécuter les tests automatisés (Selenium/NUnit)
            steps {
                // En cas d’erreur dans les tests, on continue le pipeline (build UNSTABLE)
                catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
                    bat 'dotnet test --no-build --logger "trx;LogFileName=TestResults.trx"' 
                    // Génère un fichier .trx avec les résultats des tests
                }
            }
        }

        stage('Generate HTML Report') { // Étape 4 : Générer un rapport de test lisible en HTML
            steps {
                bat '''
                    dotnet tool install --global dotnet-reportgenerator-globaltool
                    set PATH=%PATH%;%USERPROFILE%\\.dotnet\\tools
                    reportgenerator -reports:**/TestResults.trx -targetdir:TestReport -reporttypes:Html
                    '''
                // Utilise "reportgenerator" pour transformer .trx en rapport HTML dans "TestReport"
            }
        }
    }

    post {
        always {
            // Étape 5 : Publier le rapport HTML dans Jenkins à la fin, même si les tests échouent
            publishHTML(target: [
                reportName: 'Rapport des tests automatisés', // Nom affiché dans Jenkins
                reportDir: 'TestReport', // Dossier contenant le fichier HTML
                reportFiles: 'index.html', // Fichier HTML principal du rapport
                alwaysLinkToLastBuild: true,
                keepAll: true,
                allowMissing: true
            ])
        }
    }
}
