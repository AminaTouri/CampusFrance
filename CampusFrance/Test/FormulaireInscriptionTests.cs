using CampusFrance.Test;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CampusFrance.Tests
{
    public class TestsInscriptionCampusFrance
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private static List<UserRegistrationData> utilisateurs;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver(); // Démarre Chrome
            driver.Manage().Window.Maximize(); // Maximiser la fenêtre
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10)); // Timeout 10s

            // Charger les données depuis fichier JSON
            string chemin = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Data.json");
            utilisateurs = UserDataLoader.LoadFromJson(chemin);
        }

        [Test]
        public void TesterTousLesUtilisateurs()
        {
            // Ouvrir le site une première fois
            driver.Navigate().GoToUrl("https://www.campusfrance.org/fr/user/register");
            FermerBanniereCookies();

            foreach (var user in utilisateurs)
            {
                RemplirFormulaire(user);

                // Recharger le site pour utilisateur suivant
                driver.Navigate().GoToUrl("https://www.campusfrance.org/fr/user/register");
                FermerBanniereCookies();
            }
        }

        [TearDown]
        public void TearDown()
        {
            driver.Dispose(); // Fermer navigateur à la fin
        }
        private void FermerBanniereCookies()
        {
            try
            {
                var boutonAccepter = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#tarteaucitronPersonalize2")));
                boutonAccepter.Click();
                Thread.Sleep(500); // attendre que la popup disparaisse
                Console.WriteLine("✅ Bannière de cookies fermée.");
            }
            catch (WebDriverTimeoutException) 
            {
                Console.WriteLine("⚠️ Bannière cookies non trouvée à temps.");
            }
            catch (NoSuchElementException) 
            {
                Console.WriteLine("⚠️ Bannière cookies absente.");
            }
        }


    

        // MÉTHODE PRINCIPALE
        private void RemplirFormulaire(UserRegistrationData user)
        {
            RemplirIdentifiants(user);               // Email + mot de passe
            RemplirInformationsPersonnelles(user);   // Nom, prénom, pays, etc.
            RemplirStatut(user);                     // Radio bouton statut

            // Conditions selon le statut
            if (user.VousEtes == "Étudiants" || user.VousEtes == "Chercheurs")
                RemplirEtudiantChercheur(user);
            else if (user.VousEtes == "Institutionnel")
                RemplirInstitutionnel(user);
        }

        // Email + mot de passe
        private void RemplirIdentifiants(UserRegistrationData user)
        {
            // Adresse email
            driver.FindElement(By.XPath("//input[@placeholder='monadresse@domaine.com']")).SendKeys(user.AdresseEmail);

            // Mot de passe + confirmation
            driver.FindElement(By.Id("edit-pass-pass1")).SendKeys(user.MotDePasse);
            driver.FindElement(By.Id("edit-pass-pass2")).SendKeys(user.ConfirmerMotDePasse);

            // Civilité 
            driver.FindElement(By.CssSelector("label[for='edit-field-civilite-mr']")).Click();
        }

        // Informations personnelles
        private void RemplirInformationsPersonnelles(UserRegistrationData user)
        {
            // Nom et prénom
            driver.FindElement(By.Id("edit-field-nom-0-value")).SendKeys(user.Nom);
            driver.FindElement(By.Id("edit-field-prenom-0-value")).SendKeys(user.Prenom);

            // Pays de résidence 
            var paysInput = driver.FindElement(By.Id("edit-field-pays-concernes-selectized"));
            paysInput.Clear(); // Nettoyer le champ
            paysInput.SendKeys(user.PaysDeResidence); // Saisir pays
            Thread.Sleep(300); // Laisser la liste apparaître
            paysInput.SendKeys(Keys.Enter); // Sélectionner

            // Nationalité
            driver.FindElement(By.Id("edit-field-nationalite-0-target-id")).SendKeys(user.PaysDeNationalite);

            // Adresse
            driver.FindElement(By.Id("edit-field-code-postal-0-value")).SendKeys(user.CodePostal);
            driver.FindElement(By.Id("edit-field-ville-0-value")).SendKeys(user.Ville);
            driver.FindElement(By.Id("edit-field-telephone-0-value")).SendKeys(user.Telephone);
        }

        // Sélectionner statut (radio button)
        private void RemplirStatut(UserRegistrationData user)
        {
            if (user.VousEtes == "Étudiants")
            {
                var radio = driver.FindElement(By.Id("edit-field-publics-cibles-2"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", radio);
                Assert.IsTrue(radio.Selected, " Étudiant non sélectionné !");
                TestContext.WriteLine(" Étudiant sélectionné.");
            }
            else if (user.VousEtes == "Chercheurs")
            {
                var radio = driver.FindElement(By.Id("edit-field-publics-cibles-3"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", radio);
                Assert.IsTrue(radio.Selected, " Chercheur non sélectionné !");
                TestContext.WriteLine("Chercheur sélectionné.");
            }
            else if (user.VousEtes == "Institutionnel")
            {
                var radio = driver.FindElement(By.Id("edit-field-publics-cibles-4"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", radio);
                Assert.IsTrue(radio.Selected, " Institutionnel non sélectionné !");
                TestContext.WriteLine("Institutionnel sélectionné.");
            }
        }

        // Pour les étudiants ou chercheurs
        private void RemplirEtudiantChercheur(UserRegistrationData user)
        {
            // Domaine d'études 
            var domaineInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.Id("edit-field-domaine-etudes-selectized")));
            domaineInput.Click();
            domaineInput.SendKeys(Keys.Control + "a");
            domaineInput.SendKeys(Keys.Backspace);
            domaineInput.SendKeys(user.DomaineEtudes);
            domaineInput.SendKeys(Keys.Enter);
            TestContext.WriteLine(" Domaine d'études sélectionné.");

            // Niveau d'études 
            var niveauInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.Id("edit-field-niveaux-etude-selectized")));
            niveauInput.Click();
            Thread.Sleep(200);
            niveauInput.SendKeys(Keys.Control + "a");
            niveauInput.SendKeys(Keys.Backspace);
            niveauInput.SendKeys(user.NiveauEtude);
            niveauInput.SendKeys(Keys.Enter);
            TestContext.WriteLine(" Niveau d'études sélectionné.");
        }

        //  Pour les institutionnels
        private void RemplirInstitutionnel(UserRegistrationData user)
        {
            // Fonction
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("edit-field-fonction-0-value")));
            driver.FindElement(By.Id("edit-field-fonction-0-value")).SendKeys(user.Fonction);

            // Type d’organisme 
            var typeInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.Id("edit-field-type-d-organisme-selectized")));
            typeInput.Click();
            typeInput.SendKeys(Keys.Control + "a");
            typeInput.SendKeys(Keys.Backspace);
            typeInput.SendKeys(user.TypeOrganisme);
            typeInput.SendKeys(Keys.Enter);

            // Nom de l’organisme
            driver.FindElement(By.Id("edit-field-nom-de-l-organisme-0-value")).SendKeys(user.NomOrganisme);
            TestContext.WriteLine("Informations institutionnelles saisies.");
        }
    }
}
