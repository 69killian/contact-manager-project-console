using System;
using System.Collections.Generic;
using ContactNamespace;
using GestionContact;
using ContactManager.Logging;
using ContactManager.Storage;

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
            Logger.LogInfo("Application démarrée", "ContactManager v2.0 Json");
            
            try
            {
                // Chargement des contacts existants (priorité au JSON)
                List<Contact> ContactList = ChargerContactsAvecPriorite();
                Logger.LogInfo("Contacts chargés au démarrage", $"Nombre: {ContactList.Count}");

                int choix;
                do
                {
                    // Affichage du menu principal étendu
                    Console.WriteLine("\n" + "=".PadRight(70, '='));
                    Console.WriteLine("📱 GESTIONNAIRE DE CONTACTS v2.0 - Json");
                    Console.WriteLine("=".PadRight(70, '='));
                    Console.WriteLine("1. ➕ Ajouter un contact");
                    Console.WriteLine("2. 🗑️  Supprimer un contact");
                    Console.WriteLine("3. ✏️  Modifier un contact");
                    Console.WriteLine("4. 🔍 Rechercher un contact par nom");
                    Console.WriteLine("5. 🆔 Rechercher un contact par ID");
                    Console.WriteLine("6. 📋 Lister tous les contacts");
                    Console.WriteLine("7. 💾 Sauvegarder (TXT)");
                    Console.WriteLine("8. 🔄 Trier les contacts");
                    Console.WriteLine("9. 📝 Afficher les logs récents");
                    Console.WriteLine("10. 📊 Statistiques des logs");
                    Console.WriteLine("11. 🧹 Nettoyer les anciens logs");
                    Console.WriteLine("=".PadRight(70, '='));
                    Console.WriteLine("📄 SAUVEGARDE JSON MODERNE");
                    Console.WriteLine("=".PadRight(70, '='));
                    Console.WriteLine("12. 💾 Sauvegarder en JSON");
                    Console.WriteLine("13. 📂 Charger depuis JSON");
                    Console.WriteLine("14. 📤 Exporter JSON personnalisé");
                    Console.WriteLine("15. 📥 Importer JSON personnalisé");
                    Console.WriteLine("16. ℹ️  Informations fichier JSON");
                    Console.WriteLine("=".PadRight(70, '='));
                    Console.WriteLine("0. 🚪 Quitter");
                    Console.WriteLine("=".PadRight(70, '='));
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
                            case 12: // Sauvegarder en JSON
                                JsonContactManager.SauvegarderContactsJson(ContactList);
                                break;
                            case 13: // Charger depuis JSON
                                ContactList = JsonContactManager.ChargerContactsJson();
                                break;
                            case 14: // Exporter JSON personnalisé
                                Console.Write("Nom du fichier d'export (ex: export_contacts.json) : ");
                                string? exportFile = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(exportFile))
                                {
                                    if (!exportFile.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                                    {
                                        exportFile += ".json";
                                    }
                                    JsonContactManager.ExporterContacts(ContactList, exportFile);
                                }
                                else
                                {
                                    Console.WriteLine("❌ Nom de fichier invalide.");
                                }
                                break;
                            case 15: // Importer JSON personnalisé
                                Console.Write("Nom du fichier à importer : ");
                                string? importFile = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(importFile))
                                {
                                    var contactsImportes = JsonContactManager.ImporterContacts(importFile);
                                    if (contactsImportes.Count > 0)
                                    {
                                        Console.Write($"Voulez-vous remplacer ({ContactList.Count}) ou fusionner ({contactsImportes.Count}) ? (R/F) : ");
                                        string? choixImport = Console.ReadLine()?.ToUpper();
                                        if (choixImport == "R")
                                        {
                                            ContactList = contactsImportes;
                                            Console.WriteLine("✅ Contacts remplacés.");
                                        }
                                        else if (choixImport == "F")
                                        {
                                            ContactList.AddRange(contactsImportes);
                                            Console.WriteLine($"✅ Contacts fusionnés. Total: {ContactList.Count}");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("❌ Nom de fichier invalide.");
                                }
                                break;
                            case 16: // Informations fichier JSON
                                JsonContactManager.AfficherInfosFichierJson();
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

                // Sauvegarde automatique avant fermeture (JSON prioritaire)
                Logger.LogInfo("Sauvegarde automatique avant fermeture");
                JsonContactManager.SauvegarderContactsJson(ContactList);
                GestionContact.GestionContact.SauvegarderContacts(ContactList); // Backup TXT
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
        /// Charge les contacts avec priorité au format JSON
        /// </summary>
        private static List<Contact> ChargerContactsAvecPriorite()
        {
            // Essayer d'abord le JSON
            var contactsJson = JsonContactManager.ChargerContactsJson();
            if (contactsJson.Count > 0)
            {
                Logger.LogInfo("Contacts chargés depuis JSON", $"Nombre: {contactsJson.Count}");
                return contactsJson;
            }

            // Sinon, charger depuis TXT
            var contactsTxt = GestionContact.GestionContact.ChargerContacts();
            if (contactsTxt.Count > 0)
            {
                Logger.LogInfo("Contacts chargés depuis TXT", $"Nombre: {contactsTxt.Count}");
                Console.WriteLine("🔄 Migration automatique vers JSON...");
                JsonContactManager.SauvegarderContactsJson(contactsTxt);
            }

            return contactsTxt;
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
                7 => "Sauvegarder TXT",
                8 => "Trier les contacts",
                9 => "Afficher les logs",
                10 => "Statistiques des logs",
                11 => "Nettoyer les logs",
                12 => "Sauvegarder JSON",
                13 => "Charger JSON",
                14 => "Exporter JSON",
                15 => "Importer JSON",
                16 => "Infos fichier JSON",
                0 => "Quitter",
                _ => "Option inconnue"
            };
        }
    }
}
