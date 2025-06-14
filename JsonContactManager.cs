using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ContactNamespace;
using ContactManager.Logging;

namespace ContactManager.Storage
{
    /// <summary>
    /// Gestionnaire de sauvegarde/chargement des contacts au format JSON
    /// Utilise System.Text.Json pour une sérialisation moderne et performante
    /// </summary>
    public static class JsonContactManager
    {
        private static readonly string _jsonFileName = "contacts.json";
        private static readonly string _backupFileName = "contacts_backup.json";
        
        // Options de sérialisation JSON pour un format lisible
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // Format indenté pour la lisibilité
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Convention camelCase
            Converters = { new JsonStringEnumConverter() }, // Énumérations en string
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Sauvegarde la liste des contacts au format JSON
        /// </summary>
        /// <param name="contacts">Liste des contacts à sauvegarder</param>
        /// <returns>True si la sauvegarde a réussi, False sinon</returns>
        public static bool SauvegarderContactsJson(List<Contact> contacts)
        {
            try
            {
                Logger.LogInfo("Début de sauvegarde JSON", $"Nombre de contacts: {contacts.Count}");
                
                // Créer une sauvegarde de l'ancien fichier si il existe
                if (File.Exists(_jsonFileName))
                {
                    File.Copy(_jsonFileName, _backupFileName, overwrite: true);
                    Logger.LogDebug("Sauvegarde de sécurité créée", _backupFileName);
                }

                // Créer un objet de métadonnées pour enrichir le JSON
                var contactData = new ContactData
                {
                    Version = "2.0",
                    DateSauvegarde = DateTime.Now,
                    NombreContacts = contacts.Count,
                    Contacts = contacts
                };

                // Sérialiser en JSON
                string jsonString = JsonSerializer.Serialize(contactData, _jsonOptions);
                
                // Écrire dans le fichier
                File.WriteAllText(_jsonFileName, jsonString);
                
                Console.WriteLine($"✅ Contacts sauvegardés en JSON avec succès !");
                Console.WriteLine($"📁 Fichier: {_jsonFileName}");
                Console.WriteLine($"📊 {contacts.Count} contacts sauvegardés");
                
                Logger.LogSuccess("Sauvegarde JSON terminée", 
                    $"Fichier: {_jsonFileName} | Contacts: {contacts.Count} | Taille: {new FileInfo(_jsonFileName).Length} bytes");
                
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException("SauvegarderContactsJson", ex);
                Console.WriteLine($"❌ Erreur lors de la sauvegarde JSON : {ex.Message}");
                
                // Restaurer la sauvegarde si possible
                if (File.Exists(_backupFileName))
                {
                    try
                    {
                        File.Copy(_backupFileName, _jsonFileName, overwrite: true);
                        Logger.LogInfo("Sauvegarde de sécurité restaurée");
                    }
                    catch (Exception restoreEx)
                    {
                        Logger.LogException("RestaurerSauvegarde", restoreEx);
                    }
                }
                
                return false;
            }
        }

        /// <summary>
        /// Charge la liste des contacts depuis le fichier JSON
        /// </summary>
        /// <returns>Liste des contacts chargés</returns>
        public static List<Contact> ChargerContactsJson()
        {
            var contacts = new List<Contact>();
            
            try
            {
                Logger.LogInfo("Début de chargement JSON", $"Fichier: {_jsonFileName}");
                
                if (!File.Exists(_jsonFileName))
                {
                    Logger.LogInfo("Fichier JSON non trouvé", "Création d'une liste vide");
                    Console.WriteLine("📝 Aucun fichier JSON trouvé, création d'une nouvelle liste.");
                    return contacts;
                }

                // Lire le contenu du fichier
                string jsonString = File.ReadAllText(_jsonFileName);
                
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    Logger.LogWarning("Fichier JSON vide");
                    Console.WriteLine("⚠️  Fichier JSON vide, création d'une nouvelle liste.");
                    return contacts;
                }

                // Désérialiser le JSON
                var contactData = JsonSerializer.Deserialize<ContactData>(jsonString, _jsonOptions);
                
                if (contactData?.Contacts != null)
                {
                    contacts = contactData.Contacts;
                    
                    Console.WriteLine($"✅ Contacts chargés depuis JSON avec succès !");
                    Console.WriteLine($"📁 Fichier: {_jsonFileName}");
                    Console.WriteLine($"📊 {contacts.Count} contacts chargés");
                    Console.WriteLine($"📅 Dernière sauvegarde: {contactData.DateSauvegarde:dd/MM/yyyy HH:mm:ss}");
                    Console.WriteLine($"🔢 Version: {contactData.Version}");
                    
                    Logger.LogSuccess("Chargement JSON terminé", 
                        $"Contacts chargés: {contacts.Count} | Version: {contactData.Version} | Date: {contactData.DateSauvegarde}");
                }
                else
                {
                    Logger.LogWarning("Structure JSON invalide", "Contacts null");
                    Console.WriteLine("⚠️  Structure JSON invalide, création d'une nouvelle liste.");
                }
            }
            catch (JsonException jsonEx)
            {
                Logger.LogException("ChargerContactsJson_JsonException", jsonEx);
                Console.WriteLine($"❌ Erreur de format JSON : {jsonEx.Message}");
                Console.WriteLine("🔄 Tentative de récupération depuis la sauvegarde...");
                
                // Essayer de charger depuis la sauvegarde
                contacts = TenterRecuperationSauvegarde();
            }
            catch (Exception ex)
            {
                Logger.LogException("ChargerContactsJson", ex);
                Console.WriteLine($"❌ Erreur lors du chargement JSON : {ex.Message}");
                
                // Essayer de charger depuis la sauvegarde
                contacts = TenterRecuperationSauvegarde();
            }
            
            return contacts;
        }

        /// <summary>
        /// Tente de récupérer les contacts depuis le fichier de sauvegarde
        /// </summary>
        /// <returns>Liste des contacts récupérés ou liste vide</returns>
        private static List<Contact> TenterRecuperationSauvegarde()
        {
            try
            {
                if (File.Exists(_backupFileName))
                {
                    Logger.LogInfo("Tentative de récupération depuis la sauvegarde");
                    
                    string backupJson = File.ReadAllText(_backupFileName);
                    var contactData = JsonSerializer.Deserialize<ContactData>(backupJson, _jsonOptions);
                    
                    if (contactData?.Contacts != null)
                    {
                        Console.WriteLine($"✅ Récupération réussie depuis la sauvegarde !");
                        Console.WriteLine($"📊 {contactData.Contacts.Count} contacts récupérés");
                        
                        Logger.LogSuccess("Récupération depuis sauvegarde réussie", 
                            $"Contacts: {contactData.Contacts.Count}");
                        
                        return contactData.Contacts;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("TenterRecuperationSauvegarde", ex);
                Console.WriteLine($"❌ Impossible de récupérer depuis la sauvegarde : {ex.Message}");
            }
            
            Logger.LogWarning("Récupération impossible", "Création d'une liste vide");
            Console.WriteLine("📝 Création d'une nouvelle liste vide.");
            return new List<Contact>();
        }

        /// <summary>
        /// Exporte les contacts vers un fichier JSON avec un nom personnalisé
        /// </summary>
        /// <param name="contacts">Liste des contacts à exporter</param>
        /// <param name="fileName">Nom du fichier d'export</param>
        /// <returns>True si l'export a réussi</returns>
        public static bool ExporterContacts(List<Contact> contacts, string fileName)
        {
            try
            {
                Logger.LogInfo("Début d'export personnalisé", $"Fichier: {fileName} | Contacts: {contacts.Count}");
                
                var contactData = new ContactData
                {
                    Version = "2.0",
                    DateSauvegarde = DateTime.Now,
                    NombreContacts = contacts.Count,
                    Contacts = contacts,
                    TypeExport = "Export personnalisé"
                };

                string jsonString = JsonSerializer.Serialize(contactData, _jsonOptions);
                File.WriteAllText(fileName, jsonString);
                
                Console.WriteLine($"✅ Export réussi vers {fileName}");
                Logger.LogSuccess("Export personnalisé terminé", $"Fichier: {fileName}");
                
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException("ExporterContacts", ex);
                Console.WriteLine($"❌ Erreur lors de l'export : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Importe les contacts depuis un fichier JSON
        /// </summary>
        /// <param name="fileName">Nom du fichier à importer</param>
        /// <returns>Liste des contacts importés</returns>
        public static List<Contact> ImporterContacts(string fileName)
        {
            try
            {
                Logger.LogInfo("Début d'import personnalisé", $"Fichier: {fileName}");
                
                if (!File.Exists(fileName))
                {
                    Console.WriteLine($"❌ Fichier {fileName} non trouvé.");
                    Logger.LogWarning("Fichier d'import non trouvé", fileName);
                    return new List<Contact>();
                }

                string jsonString = File.ReadAllText(fileName);
                var contactData = JsonSerializer.Deserialize<ContactData>(jsonString, _jsonOptions);
                
                if (contactData?.Contacts != null)
                {
                    Console.WriteLine($"✅ Import réussi depuis {fileName}");
                    Console.WriteLine($"📊 {contactData.Contacts.Count} contacts importés");
                    
                    Logger.LogSuccess("Import personnalisé terminé", 
                        $"Fichier: {fileName} | Contacts: {contactData.Contacts.Count}");
                    
                    return contactData.Contacts;
                }
                
                Logger.LogWarning("Structure JSON invalide lors de l'import", fileName);
                Console.WriteLine("⚠️  Structure JSON invalide.");
                return new List<Contact>();
            }
            catch (Exception ex)
            {
                Logger.LogException("ImporterContacts", ex);
                Console.WriteLine($"❌ Erreur lors de l'import : {ex.Message}");
                return new List<Contact>();
            }
        }

        /// <summary>
        /// Affiche les informations sur le fichier JSON
        /// </summary>
        public static void AfficherInfosFichierJson()
        {
            try
            {
                if (File.Exists(_jsonFileName))
                {
                    var fileInfo = new FileInfo(_jsonFileName);
                    Console.WriteLine("\n📋 Informations du fichier JSON:");
                    Console.WriteLine($"📁 Nom: {fileInfo.Name}");
                    Console.WriteLine($"📏 Taille: {fileInfo.Length} bytes");
                    Console.WriteLine($"📅 Dernière modification: {fileInfo.LastWriteTime:dd/MM/yyyy HH:mm:ss}");
                    
                    // Essayer de lire les métadonnées
                    try
                    {
                        string jsonString = File.ReadAllText(_jsonFileName);
                        var contactData = JsonSerializer.Deserialize<ContactData>(jsonString, _jsonOptions);
                        
                        if (contactData != null)
                        {
                            Console.WriteLine($"🔢 Version: {contactData.Version}");
                            Console.WriteLine($"📊 Nombre de contacts: {contactData.NombreContacts}");
                            Console.WriteLine($"💾 Date de sauvegarde: {contactData.DateSauvegarde:dd/MM/yyyy HH:mm:ss}");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("⚠️  Impossible de lire les métadonnées");
                    }
                }
                else
                {
                    Console.WriteLine("📝 Aucun fichier JSON trouvé.");
                }
                
                if (File.Exists(_backupFileName))
                {
                    var backupInfo = new FileInfo(_backupFileName);
                    Console.WriteLine($"\n🔄 Sauvegarde disponible: {backupInfo.Name} ({backupInfo.Length} bytes)");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("AfficherInfosFichierJson", ex);
                Console.WriteLine($"❌ Erreur lors de la lecture des informations : {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Classe pour structurer les données JSON avec métadonnées
    /// </summary>
    public class ContactData
    {
        public string Version { get; set; } = "2.0";
        public DateTime DateSauvegarde { get; set; }
        public int NombreContacts { get; set; }
        public string? TypeExport { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }
} 