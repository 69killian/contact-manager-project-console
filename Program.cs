using System;
using System.Collections.Generic;
using ContactNamespace;
using GestionContact;
using ContactManager.Logging;

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
            // Initialisation du système de logging
            Logger.Initialize();
            Logger.LogInfo("Application démarrée", "ContactManager v2.0");
            
            try
            {
                // Chargement des contacts existants
                List<Contact> ContactList = GestionContact.GestionContact.ChargerContacts();
                Logger.LogInfo("Contacts chargés au démarrage", $"Nombre: {ContactList.Count}");

                int choix;
                do
                {
                    // Affichage du menu principal
                    Console.WriteLine("\n" + "=".PadRight(60, '='));
                    Console.WriteLine("📱 GESTIONNAIRE DE CONTACTS");
                    Console.WriteLine("=".PadRight(60, '='));
                    Console.WriteLine("1. ➕ Ajouter un contact");
                    Console.WriteLine("2. 🗑️  Supprimer un contact");
                    Console.WriteLine("3. ✏️  Modifier un contact");
                    Console.WriteLine("4. 🔍 Rechercher un contact par nom");
                    Console.WriteLine("5. 🆔 Rechercher un contact par ID");
                    Console.WriteLine("6. 📋 Lister tous les contacts");
                    Console.WriteLine("7. 💾 Sauvegarder les contacts");
                    Console.WriteLine("8. 🔄 Trier les contacts");
                    Console.WriteLine("9. 📝 Afficher les logs récents");
                    Console.WriteLine("10. 📊 Statistiques des logs");
                    Console.WriteLine("11. 🧹 Nettoyer les anciens logs");
                    Console.WriteLine("0. 🚪 Quitter");
                    Console.WriteLine("=".PadRight(60, '='));
                    Console.Write("Votre choix : ");

                    if (int.TryParse(Console.ReadLine(), out choix))
                    {
                        Logger.LogInfo($"Option sélectionnée: {choix}", GetMenuOptionName(choix));
                        
                        switch (choix)
                        {
                            case 1:
                                GestionContact.GestionContact.AjouterContactAvecVerification(ContactList);
                                break;
                            case 2:
                                GestionContact.GestionContact.SupprimerContact(ContactList);
                                break;
                            case 3:
                                GestionContact.GestionContact.ModifierContact(ContactList);
                                break;
                            case 4:
                                GestionContact.GestionContact.RechercherContact(ContactList);
                                break;
                            case 5:
                                GestionContact.GestionContact.RechercherContactParId(ContactList);
                                break;
                            case 6:
                                GestionContact.GestionContact.ListerContacts(ContactList);
                                break;
                            case 7:
                                GestionContact.GestionContact.SauvegarderContacts(ContactList);
                                break;
                            case 8:
                                GestionContact.GestionContact.TrierContacts(ContactList);
                                break;
                            case 9:
                                Console.Write("Nombre de lignes à afficher (défaut 10) : ");
                                if (int.TryParse(Console.ReadLine(), out int nbLignes) && nbLignes > 0)
                                {
                                    Logger.ShowRecentLogs(nbLignes);
                                }
                                else
                                {
                                    Logger.ShowRecentLogs();
                                }
                                break;
                            case 10:
                                Logger.ShowLogStats();
                                break;
                            case 11:
                                Console.Write("Nombre de jours à conserver (défaut 30) : ");
                                if (int.TryParse(Console.ReadLine(), out int jours) && jours > 0)
                                {
                                    Logger.CleanupLogs(jours);
                                    Console.WriteLine($"✅ Logs antérieurs à {jours} jours supprimés.");
                                }
                                else
                                {
                                    Logger.CleanupLogs();
                                    Console.WriteLine("✅ Logs antérieurs à 30 jours supprimés.");
                                }
                                break;
                            case 0:
                                Logger.LogInfo("Utilisateur a choisi de quitter l'application");
                                Console.WriteLine("👋 Au revoir !");
                                break;
                            default:
                                Console.WriteLine("❌ Choix invalide ! Veuillez réessayer.");
                                Logger.LogWarning("Choix de menu invalide", $"Valeur: {choix}");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ Veuillez entrer un nombre valide !");
                        Logger.LogWarning("Saisie invalide dans le menu", "Non numérique");
                        choix = -1; // Pour continuer la boucle
                    }

                    if (choix != 0)
                    {
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }

                } while (choix != 0);

                // Sauvegarde automatique avant fermeture
                Logger.LogInfo("Sauvegarde automatique avant fermeture");
                GestionContact.GestionContact.SauvegarderContacts(ContactList);
            }
            catch (Exception ex)
            {
                Logger.LogException("Main", ex);
                Console.WriteLine($"❌ Erreur critique : {ex.Message}");
                Console.WriteLine("Appuyez sur une touche pour fermer...");
                Console.ReadKey();
            }
            finally
            {
                // Finalisation du système de logging
                Logger.Shutdown();
            }
        }

        /// <summary>
        /// Retourne le nom de l'option du menu pour le logging
        /// </summary>
        private static string GetMenuOptionName(int choix)
        {
            return choix switch
            {
                1 => "Ajouter un contact",
                2 => "Supprimer un contact",
                3 => "Modifier un contact",
                4 => "Rechercher par nom",
                5 => "Rechercher par ID",
                6 => "Lister les contacts",
                7 => "Sauvegarder",
                8 => "Trier les contacts",
                9 => "Afficher les logs",
                10 => "Statistiques des logs",
                11 => "Nettoyer les logs",
                0 => "Quitter",
                _ => "Option inconnue"
            };
        }
    }
}
