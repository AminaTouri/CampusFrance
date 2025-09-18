pipeline {
    agent any

    tools {
    dotnetsdk 'dotnet8'  // ✅ Correct : correspond au type reconnu par Jenkins
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/AminaTouri/CampusFrance.git'
            }
        }

        stage('Restore') {
            steps {
                dir('CampusFrance') {
                    bat 'dotnet restore'
                }
            }
        }

        stage('Build') {
            steps {
                dir('CampusFrance') {
                    bat 'dotnet build --configuration Release'
                }
            }
        }

        stage('Run Tests') {
            steps {
                dir('CampusFrance') {
                    bat 'dotnet test --no-build --logger "trx;LogFileName=TestResults.trx"'
                }
            }
        }
    }

    post {
        always {
            echo '📦 Pipeline terminé.'
        }

        success {
            echo '✅ Tests réussis.'
        }

        failure {
            echo '❌ Tests échoués.'
        }
    }
}
