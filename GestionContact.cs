using System;
using System.Collections.Generic;
using System.IO;
using ContactNamespace;

namespace GestionContact
{
    /// <summary>
    /// Classe héritant de Contact et contenant toutes les méthodes de gestion
    /// </summary>
    public class GestionContact : Contact
    {
        // Chemin du fichier de sauvegarde des contacts
        private static string fichierContacts = "contacts.txt";

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
                Console.WriteLine("\nListe des contacts :");
                Console.WriteLine("--------------------");
                for (int i = 0; i < ContactList.Count; i++)
                {
                    Contact c = ContactList[i];
                    Console.WriteLine($"[{i + 1}] Nom : {c.Nom}, Prénom : {c.Prenom}, Email : {c.Email}, Téléphone : {c.Telephone}");
                }
                Console.WriteLine("Nombre de contacts : " + ContactList.Count);

                Console.WriteLine("\nEntrez le numéro du contact à supprimer : ");
                if (int.TryParse(Console.ReadLine(), out int choix) && choix >= 1 && choix <= ContactList.Count)
                {
                    Contact contactASupprimer = ContactList[choix - 1];
                    Console.WriteLine($"\nVoulez-vous vraiment supprimer le contact : {contactASupprimer.Nom} {contactASupprimer.Prenom} ? (O/N)");
                    string? reponse = Console.ReadLine()?.ToLower() ?? "n";

                    if (reponse == "o" || reponse == "oui")
                    {
                        ContactList.RemoveAt(choix - 1);
                        Console.WriteLine("Contact supprimé avec succès !");
                    }
                    else
                    {
                        Console.WriteLine("Suppression annulée !");
                    }
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

                Console.WriteLine("\nListe des contacts :");
                Console.WriteLine("--------------------");
                for (int i = 0; i < ContactList.Count; i++)
                {
                    Contact c = ContactList[i];
                    Console.WriteLine($"[{i + 1}] Nom : {c.Nom}, Prénom : {c.Prenom}, Email : {c.Email}, Téléphone : {c.Telephone}");
                }

                Console.WriteLine("\nEntrez le numéro du contact à modifier : ");
                if (int.TryParse(Console.ReadLine(), out int choix) && choix >= 1 && choix <= ContactList.Count)
                {
                    int index = choix - 1;
                    Contact c = ContactList[index];
                    
                    Console.WriteLine($"\nModification du contact : {c.Nom} {c.Prenom}");
                    Console.WriteLine("(Appuyez sur Entrée sans rien écrire pour garder la valeur actuelle)");

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
                Console.WriteLine("Nombre de contacts : " + ContactList.Count);
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

        /// <summary>
        /// Trie la liste des contacts par nom ou prénom, dans l'ordre ascendant ou descendant
        /// </summary>
        public static void TrierContacts(List<Contact> ContactList)
        {
            try
            {
                if (ContactList.Count == 0)
                {
                    Console.WriteLine("Aucun contact à trier !");
                    return;
                }

                Console.WriteLine("\nTrier les contacts par :");
                Console.WriteLine("1. Nom");
                Console.WriteLine("2. Prénom");
                Console.Write("Votre choix : ");

                if (!int.TryParse(Console.ReadLine(), out int choix) || (choix != 1 && choix != 2))
                {
                    Console.WriteLine("Choix invalide !");
                    return;
                }

                Console.Write("Ordre ascendant (A) ou descendant (D) ? ");
                string? ordre = Console.ReadLine()?.ToUpper();
                
                if (ordre != "A" && ordre != "D")
                {
                    Console.WriteLine("Choix invalide !");
                    return;
                }

                switch (choix)
                {
                    case 1: // Tri par nom
                        ContactList.Sort((c1, c2) => c1.Nom.CompareTo(c2.Nom));
                        break;
                    case 2: // Tri par prénom
                        ContactList.Sort((c1, c2) => c1.Prenom.CompareTo(c2.Prenom));
                        break;
                }

                if (ordre == "D")
                {
                    ContactList.Reverse();
                }

                Console.WriteLine("\nListe des contacts triés :");
                Console.WriteLine("------------------------");
                foreach (Contact c in ContactList)
                {
                    Console.WriteLine($"Nom : {c.Nom}, Prénom : {c.Prenom}, Email : {c.Email}, Téléphone : {c.Telephone}");
                }
                Console.WriteLine("------------------------");
                Console.WriteLine("Contacts triés avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du tri : {ex.Message}");
            }
        }
    }
} 