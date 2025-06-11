using System;

namespace ContactNamespace
{
    /// <summary>
    /// Classe représentant un contact avec ses informations personnelles
    /// </summary>
    public class Contact
    {
        // Propriétés du contact
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public Contact()
        {
            Nom = string.Empty;
            Prenom = string.Empty;
            Email = string.Empty;
            Telephone = string.Empty;
        }

        /// <summary>
        /// Constructeur avec paramètres
        /// </summary>
        public Contact(string nom, string prenom, string email, string telephone)
        {
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Telephone = telephone;
        }
    }
}
