using System;
using System.Collections.Generic;
using System.IO;

namespace ContactNamespace
{
    /// <summary>
    /// Classe représentant un contact avec ses informations personnelles
    /// </summary>
    class Contact
    {
        // Propriétés du contact
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        // Chemin du fichier de sauvegarde des contacts
        private static string fichierContacts = "contacts.txt";

        /// <summary>
        /// Constructeur par défaut initialisant un contact vide
        /// </summary>
        public Contact()
        {
            Nom = string.Empty;
            Prenom = string.Empty;
            Email = string.Empty;
            Telephone = string.Empty;
        }

        /// <summary>
        /// Constructeur avec paramètres pour créer un contact avec toutes ses informations
        /// </summary>
        public Contact(string nom, string prenom, string email, string telephone)
        {
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Telephone = telephone;
        }

        /// <summary>
        /// Permet à l'utilisateur d'ajouter un nouveau contact en saisissant ses informations
        /// </summary>
        public void AjouterContact()
        {
            try
            {
                Console.WriteLine("Entrez le nom du contact : ");
                Nom = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Entrez le prenom du contact : ");
                Prenom = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Entrez l'email du contact : ");
                Email = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Entrez le numero de telephone du contact : ");
                Telephone = Console.ReadLine() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout du contact : {ex.Message}");
            }
        }

        /// <summary>
        /// Affiche la liste des contacts et permet d'en supprimer un en sélectionnant son numéro
        /// </summary>
        public static void SupprimerContact(List<Contact> ContactList)
        {
            try
            {
                // Affichage de la liste numérotée des contacts
                Console.WriteLine("\nListe des contacts :");
                Console.WriteLine("--------------------");
                for (int i = 0; i < ContactList.Count; i++)
                {
                    Contact c = ContactList[i];
                    Console.WriteLine($"[{i + 1}] Nom : {c.Nom}, Prénom : {c.Prenom}, Email : {c.Email}, Téléphone : {c.Telephone}");
                }

                // Suppression du contact sélectionné
                Console.WriteLine("\nEntrez le numéro du contact à supprimer : ");
                if (int.TryParse(Console.ReadLine(), out int choix) && choix >= 1 && choix <= ContactList.Count)
                {
                    ContactList.RemoveAt(choix - 1);
                    Console.WriteLine("Contact supprimé avec succès !");
                }
                else
                {
                    Console.WriteLine("Numéro de contact invalide !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
            }
        }

        /// <summary>
        /// Affiche la liste des contacts et permet d'en modifier un en sélectionnant son numéro
        /// Les champs vides conservent leur valeur précédente
        /// </summary>
        public static void ModifierContact(List<Contact> ContactList)
        {
            try
            {
                if (ContactList.Count == 0)
                {
                    Console.WriteLine("Aucun contact à modifier !");
                    return;
                }

                // Affichage de la liste numérotée des contacts
                Console.WriteLine("\nListe des contacts :");
                Console.WriteLine("--------------------");
                for (int i = 0; i < ContactList.Count; i++)
                {
                    Contact c = ContactList[i];
                    Console.WriteLine($"[{i + 1}] Nom : {c.Nom}, Prénom : {c.Prenom}, Email : {c.Email}, Téléphone : {c.Telephone}");
                }

                // Sélection et modification du contact
                Console.WriteLine("\nEntrez le numéro du contact à modifier : ");
                if (int.TryParse(Console.ReadLine(), out int choix) && choix >= 1 && choix <= ContactList.Count)
                {
                    int index = choix - 1;
                    Contact c = ContactList[index];
                    
                    Console.WriteLine($"\nModification du contact : {c.Nom} {c.Prenom}");
                    Console.WriteLine("(Appuyez sur Entrée sans rien écrire pour garder la valeur actuelle)");

                    // Modification des champs avec conservation des anciennes valeurs si vide
                    Console.Write($"Nom actuel : {c.Nom}\nNouveau nom : ");
                    string nouveauNom = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(nouveauNom)) c.Nom = nouveauNom;

                    Console.Write($"Prénom actuel : {c.Prenom}\nNouveau prénom : ");
                    string nouveauPrenom = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(nouveauPrenom)) c.Prenom = nouveauPrenom;

                    Console.Write($"Email actuel : {c.Email}\nNouvel email : ");
                    string nouvelEmail = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(nouvelEmail)) c.Email = nouvelEmail;

                    Console.Write($"Téléphone actuel : {c.Telephone}\nNouveau téléphone : ");
                    string nouveauTelephone = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(nouveauTelephone)) c.Telephone = nouveauTelephone;

                    Console.WriteLine("\nContact modifié avec succès !");
                }
                else
                {
                    Console.WriteLine("Numéro de contact invalide !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
            }
        }

        /// <summary>
        /// Recherche un contact par son nom exact
        /// </summary>
        public static void RechercherContact(List<Contact> ContactList)
        {
            try
            {
                Console.WriteLine("Entrez le nom du contact à rechercher : ");
                string nom = Console.ReadLine() ?? string.Empty;
                Contact contact = ContactList.Find(c => c.Nom == nom);
                if (contact != null)
                {
                    Console.WriteLine($"Contact trouvé : Nom : {contact.Nom}, Prénom : {contact.Prenom}, Email : {contact.Email}, Téléphone : {contact.Telephone}");
                }
                else
                {
                    Console.WriteLine("Contact non trouvé !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la recherche : {ex.Message}");
            }
        }

        /// <summary>
        /// Affiche la liste complète des contacts
        /// </summary>
        public static void ListerContacts(List<Contact> ContactList)
        {
            try
            {
                Console.WriteLine("\nListe des contacts :");
                Console.WriteLine("--------------------");
                foreach (Contact c in ContactList)
                {
                    Console.WriteLine($"Nom : {c.Nom}, Prénom : {c.Prenom}, Email : {c.Email}, Téléphone : {c.Telephone}");
                }
                Console.WriteLine("--------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'affichage de la liste : {ex.Message}");
            }
        }

        /// <summary>
        /// Sauvegarde la liste des contacts dans un fichier texte
        /// Format : Nom;Prénom;Email;Téléphone (un contact par ligne)
        /// </summary>
        public static void SauvegarderContacts(List<Contact> ContactList)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fichierContacts))
                {
                    foreach (Contact c in ContactList)
                    {
                        writer.WriteLine($"{c.Nom};{c.Prenom};{c.Email};{c.Telephone}");
                    }
                }
                Console.WriteLine("Contacts sauvegardés avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde : {ex.Message}");
            }
        }

        /// <summary>
        /// Charge la liste des contacts depuis le fichier texte
        /// Crée une liste vide si le fichier n'existe pas
        /// </summary>
        public static List<Contact> ChargerContacts()
        {
            List<Contact> contacts = new List<Contact>();
            try
            {
                if (File.Exists(fichierContacts))
                {
                    using (StreamReader reader = new StreamReader(fichierContacts))
                    {
                        string? ligne;
                        while ((ligne = reader.ReadLine()) != null)
                        {
                            string[] donnees = ligne.Split(';');
                            if (donnees.Length == 4)
                            {
                                contacts.Add(new Contact(donnees[0], donnees[1], donnees[2], donnees[3]));
                            }
                        }
                    }
                    Console.WriteLine("Contacts chargés avec succès !");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement : {ex.Message}");
            }
            return contacts;
        }
    }
}
