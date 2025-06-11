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
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Chargement initial des contacts depuis le fichier
            List<Contact> ContactList = GestionContact.GestionContact.ChargerContacts();
            bool continuer = true;

            // Boucle principale du programme
            while (continuer)
            {
                // Affichage du menu principal
                Console.WriteLine("\n=== Gestionnaire de Contacts ===");
                Console.WriteLine("1. Ajouter un contact");
                Console.WriteLine("2. Supprimer un contact");
                Console.WriteLine("3. Lister les contacts");
                Console.WriteLine("4. Rechercher un contact");
                Console.WriteLine("5. Modifier un contact");
                Console.WriteLine("6. Trier les contacts");
                Console.WriteLine("7. Sauvegarder et quitter");
                Console.Write("Votre choix : ");

                // Traitement du choix de l'utilisateur
                string? choix = Console.ReadLine();
                switch (choix)
                {
                    case "1": // Ajout d'un contact
                        GestionContact.GestionContact contact = new GestionContact.GestionContact();
                        contact.AjouterContact();
                        ContactList.Add(contact);
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

                    case "4": // Recherche d'un contact
                        GestionContact.GestionContact.RechercherContact(ContactList);
                        break;

                    case "5": // Modification d'un contact
                        GestionContact.GestionContact.ModifierContact(ContactList);
                        GestionContact.GestionContact.SauvegarderContacts(ContactList);
                        break;

                    case "6": // Tri des contacts
                        GestionContact.GestionContact.TrierContacts(ContactList);
                        break;

                    case "7": // Sauvegarde et sortie
                        GestionContact.GestionContact.SauvegarderContacts(ContactList);
                        continuer = false;
                        break;

                    default: // Choix invalide
                        Console.WriteLine("Choix invalide !");
                        break;
                }
            }
        }
    }
}
