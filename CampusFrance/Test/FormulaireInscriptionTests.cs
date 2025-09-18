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
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            string chemin = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Data.json");
            utilisateurs = UserDataLoader.LoadFromJson(chemin);
        }

        [Test]
        public void TesterTousLesUtilisateurs()
        {
            driver.Navigate().GoToUrl("https://www.campusfrance.org/fr/user/register");
            FermerBanniereCookies();

            foreach (var user in utilisateurs)
            {
                RemplirFormulaire(user);
                driver.Navigate().GoToUrl("https://www.campusfrance.org/fr/user/register");
                FermerBanniereCookies();
            }
        }

        [TearDown]
        public void TearDown()
        {
            driver.Dispose();
        }
        private void FermerBanniereCookies()
{
    try
    {
        var boutonAccepter = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("tarteaucitronPersonalize2")));
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

    // 🔽 Ajouter cette partie pour s'assurer que le X (cross) a disparu
    try
    {
        wait.Until(driver =>
        {
            var cross = driver.FindElement(By.CssSelector(".tarteaucitronCross"));
            return !cross.Displayed || !cross.Enabled;
        });
        Console.WriteLine("✅ Bouton X de la bannière cookies disparu.");
    }
    catch (WebDriverTimeoutException)
    {
        Console.WriteLine("⚠️ Le bouton X est toujours visible (possible blocage).");
    }

    // 🔽 Masquer le bouton "tarteaucitronManager" s’il bloque les clics
    try
    {
        var managerButton = driver.FindElement(By.Id("tarteaucitronManager"));
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.display='none';", managerButton);
        Console.WriteLine("✅ Bouton tarteaucitronManager masqué.");
    }
    catch (NoSuchElementException)
    {
        Console.WriteLine("⚠️ Bouton tarteaucitronManager absent.");
    }
}


     
}


        private void RemplirFormulaire(UserRegistrationData user)
        {
            RemplirIdentifiants(user);
            RemplirInformationsPersonnelles(user);
            RemplirStatut(user);

            if (user.VousEtes == "Étudiants" || user.VousEtes == "Chercheurs")
                RemplirEtudiantChercheur(user);
            else if (user.VousEtes == "Institutionnel")
                RemplirInstitutionnel(user);
        }

        private void RemplirIdentifiants(UserRegistrationData user)
        {
            driver.FindElement(By.XPath("//input[@placeholder='monadresse@domaine.com']")).SendKeys(user.AdresseEmail);
            driver.FindElement(By.Id("edit-pass-pass1")).SendKeys(user.MotDePasse);
            driver.FindElement(By.Id("edit-pass-pass2")).SendKeys(user.ConfirmerMotDePasse);
            driver.FindElement(By.CssSelector("label[for='edit-field-civilite-mr']")).Click();
        }

        private void RemplirInformationsPersonnelles(UserRegistrationData user)
        {
            driver.FindElement(By.Id("edit-field-nom-0-value")).SendKeys(user.Nom);
            driver.FindElement(By.Id("edit-field-prenom-0-value")).SendKeys(user.Prenom);

            var paysInput = driver.FindElement(By.Id("edit-field-pays-concernes-selectized"));
            paysInput.Clear();
            paysInput.SendKeys(user.PaysDeResidence);
            Thread.Sleep(300);
            paysInput.SendKeys(Keys.Enter);

            driver.FindElement(By.Id("edit-field-nationalite-0-target-id")).SendKeys(user.PaysDeNationalite);
            driver.FindElement(By.Id("edit-field-code-postal-0-value")).SendKeys(user.CodePostal);
            driver.FindElement(By.Id("edit-field-ville-0-value")).SendKeys(user.Ville);
            driver.FindElement(By.Id("edit-field-telephone-0-value")).SendKeys(user.Telephone);
        }

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

                // ✅ Attendre l’apparition des champs dynamiques
                wait.Until(driver =>
                {
                    try
                    {
                        var elem = driver.FindElement(By.Id("edit-field-type-d-organisme-selectized"));
                        return elem.Displayed && elem.Enabled;
                    }
                    catch (NoSuchElementException)
                    {
                        return false;
                    }
                });
            }
        }

        private void RemplirEtudiantChercheur(UserRegistrationData user)
        {
            var domaineInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.Id("edit-field-domaine-etudes-selectized")));
            domaineInput.Click();
            domaineInput.SendKeys(Keys.Control + "a");
            domaineInput.SendKeys(Keys.Backspace);
            domaineInput.SendKeys(user.DomaineEtudes);
            domaineInput.SendKeys(Keys.Enter);
            TestContext.WriteLine(" Domaine d'études sélectionné.");

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

        private void RemplirInstitutionnel(UserRegistrationData user)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("edit-field-fonction-0-value")));
            driver.FindElement(By.Id("edit-field-fonction-0-value")).SendKeys(user.Fonction);

            var typeInput = wait.Until(ExpectedConditions.ElementIsVisible(
                By.Id("edit-field-type-d-organisme-selectized")));
            typeInput.Click();
            typeInput.SendKeys(Keys.Control + "a");
            typeInput.SendKeys(Keys.Backspace);
            typeInput.SendKeys(user.TypeOrganisme);
            typeInput.SendKeys(Keys.Enter);

            driver.FindElement(By.Id("edit-field-nom-organisme-0-value")).SendKeys(user.NomOrganisme);
            TestContext.WriteLine("Informations institutionnelles saisies.");
        }
    }
}

