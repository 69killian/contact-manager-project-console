using System;

namespace ContactNamespace
{
    /// <summary>
    /// Classe représentant un contact avec ses informations personnelles
    /// </summary>
    public class Contact
    {
        // Identifiant unique du contact
        public Guid Id { get; set; }
        
        // Propriétés du contact
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        /// <summary>
        /// Constructeur par défaut - génère automatiquement un ID unique
        /// </summary>
        public Contact()
        {
            Id = Guid.NewGuid();
            Nom = string.Empty;
            Prenom = string.Empty;
            Email = string.Empty;
            Telephone = string.Empty;
        }

        /// <summary>
        /// Constructeur avec paramètres - génère automatiquement un ID unique
        /// </summary>
        public Contact(string nom, string prenom, string email, string telephone)
        {
            Id = Guid.NewGuid();
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Telephone = telephone;
        }

        /// <summary>
        /// Constructeur avec ID spécifique (utile pour le chargement depuis fichier)
        /// </summary>
        public Contact(Guid id, string nom, string prenom, string email, string telephone)
        {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Telephone = telephone;
        }

        /// <summary>
        /// Retourne une représentation textuelle du contact
        /// </summary>
        public override string ToString()
        {
            return $"{Nom} {Prenom} ({Email}) - {Telephone}";
        }

        /// <summary>
        /// Compare deux contacts par leur ID
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Contact other)
            {
                return Id.Equals(other.Id);
            }
            return false;
        }

        /// <summary>
        /// Retourne le hash code basé sur l'ID
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
