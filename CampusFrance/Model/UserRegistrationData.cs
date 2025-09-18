using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusFrance.Test
{
    public class UserRegistrationData
    {
        // Informations de connexion
        public string AdresseEmail { get; set; }
        public string MotDePasse { get; set; }
        public string ConfirmerMotDePasse { get; set; }

        // Informations personnelles
        public string Civilite { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string PaysDeResidence { get; set; }
        public string PaysDeNationalite { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string Telephone { get; set; }

        // Informations complémentaires
        public string VousEtes { get; set; }

        // Étudiants / Chercheurs
        public string DomaineEtudes { get; set; }
        public string NiveauEtude { get; set; }

        // Institutionnel 
        public string Fonction { get; set; }
        public string TypeOrganisme { get; set; }
        public string NomOrganisme { get; set; }
    }
}

