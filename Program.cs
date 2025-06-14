using System;
using System.Collections.Generic;
using ContactNamespace;
using GestionContact;

namespace ContactManager
{
    /// <summary>
    /// Programme principal du gestionnaire de contacts
    /// Permet d'ajouter, modifier, supprimer, rechercher et lister des contacts
    /// Les contacts sont sauvegardés automatiquement après chaque modification
    /// Chaque contact possède un identifiant unique (Guid) pour éviter les doublons
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Chargement initial des contacts depuis le fichier
            List<Contact> ContactList = GestionContact.GestionContact.ChargerContacts();
            bool continuer = true;

            Console.WriteLine("🎯 Gestionnaire de Contacts Avancé v2.0");
            Console.WriteLine("Avec identifiants uniques et détection de doublons");

            // Boucle principale du programme
            while (continuer)
            {
                // Affichage du menu principal
                Console.WriteLine("\n=== 📋 Gestionnaire de Contacts ===");
                Console.WriteLine("1. ➕ Ajouter un contact (avec vérification doublons)");
                Console.WriteLine("2. ❌ Supprimer un contact");
                Console.WriteLine("3. 📄 Lister les contacts");
                Console.WriteLine("4. 🔍 Rechercher un contact par nom");
                Console.WriteLine("5. 🆔 Rechercher un contact par ID");
                Console.WriteLine("6. ✏️  Modifier un contact");
                Console.WriteLine("7. 🔄 Trier les contacts");
                Console.WriteLine("8. 💾 Sauvegarder et quitter");
                Console.Write("Votre choix : ");

                // Traitement du choix de l'utilisateur
                string? choix = Console.ReadLine();
                switch (choix)
                {
                    case "1": // Ajout d'un contact avec vérification
                        GestionContact.GestionContact.AjouterContactAvecVerification(ContactList);
                        GestionContact.GestionContact.SauvegarderContacts(ContactList);
                        break;

                    case "2": // Suppression d'un contact
                        if (ContactList.Count > 0)
                        {
                            GestionContact.GestionContact.SupprimerContact(ContactList);
                            GestionContact.GestionContact.SauvegarderContacts(ContactList);
                        }
                        else
                        {
                            Console.WriteLine("Aucun contact à supprimer !");
                        }
                        break;

                    case "3": // Affichage de la liste des contacts
                        GestionContact.GestionContact.ListerContacts(ContactList);
                        break;

                    case "4": // Recherche d'un contact par nom
                        GestionContact.GestionContact.RechercherContact(ContactList);
                        break;

                    case "5": // Recherche d'un contact par ID
                        GestionContact.GestionContact.RechercherContactParId(ContactList);
                        break;

                    case "6": // Modification d'un contact
                        GestionContact.GestionContact.ModifierContact(ContactList);
                        GestionContact.GestionContact.SauvegarderContacts(ContactList);
                        break;

                    case "7": // Tri des contacts
                        GestionContact.GestionContact.TrierContacts(ContactList);
                        break;

                    case "8": // Sauvegarde et sortie
                        GestionContact.GestionContact.SauvegarderContacts(ContactList);
                        Console.WriteLine("👋 Au revoir ! Vos contacts ont été sauvegardés.");
                        continuer = false;
                        break;

                    default: // Choix invalide
                        Console.WriteLine("❌ Choix invalide ! Veuillez choisir entre 1 et 8.");
                        break;
                }
            }
        }
    }
}
