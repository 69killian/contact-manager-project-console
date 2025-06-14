using System;
using System.Collections.Generic;
using System.IO;

namespace ContactManager.Logging
{
    /// <summary>
    /// Classe de logging pour tracer toutes les actions du gestionnaire de contacts
    /// </summary>
    public static class Logger
    {
        private static readonly string _logFileName = "contact_manager.log";
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Types de logs disponibles
        /// </summary>
        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Success,
            Debug
        }

        /// <summary>
        /// Écrit un message dans le fichier de log avec horodatage
        /// </summary>
        /// <param name="level">Niveau de log</param>
        /// <param name="message">Message à logger</param>
        /// <param name="details">Détails supplémentaires optionnels</param>
        public static void Log(LogLevel level, string message, string? details = null)
        {
            try
            {
                lock (_lockObject)
                {
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var logEntry = $"[{timestamp}] [{level.ToString().ToUpper()}] {message}";
                    
                    if (!string.IsNullOrEmpty(details))
                    {
                        logEntry += $" | Détails: {details}";
                    }

                    using (var writer = new StreamWriter(_logFileName, append: true))
                    {
                        writer.WriteLine(logEntry);
                    }
                }
            }
            catch (Exception)
            {
                // En cas d'erreur de logging, on ignore silencieusement
                // pour éviter les conflits pendant les tests
            }
        }

        /// <summary>
        /// Log d'information
        /// </summary>
        public static void LogInfo(string message, string? details = null)
        {
            Log(LogLevel.Info, message, details);
        }

        /// <summary>
        /// Log d'avertissement
        /// </summary>
        public static void LogWarning(string message, string? details = null)
        {
            Log(LogLevel.Warning, message, details);
        }

        /// <summary>
        /// Log d'erreur
        /// </summary>
        public static void LogError(string message, string? details = null)
        {
            Log(LogLevel.Error, message, details);
        }

        /// <summary>
        /// Log de succès
        /// </summary>
        public static void LogSuccess(string message, string? details = null)
        {
            Log(LogLevel.Success, message, details);
        }

        /// <summary>
        /// Log de débogage
        /// </summary>
        public static void LogDebug(string message, string? details = null)
        {
            Log(LogLevel.Debug, message, details);
        }

        /// <summary>
        /// Log spécialisé pour les actions sur les contacts
        /// </summary>
        public static void LogContactAction(string action, string contactInfo, Guid? contactId = null)
        {
            var details = contactId.HasValue ? $"ID: {contactId}" : "ID: Non spécifié";
            LogInfo($"Action contact: {action}", $"{contactInfo} | {details}");
        }

        /// <summary>
        /// Log spécialisé pour les erreurs avec exception
        /// </summary>
        public static void LogException(string context, Exception ex)
        {
            LogError($"Exception dans {context}", $"{ex.GetType().Name}: {ex.Message}");
        }

        /// <summary>
        /// Affiche les dernières entrées du log
        /// </summary>
        /// <param name="numberOfLines">Nombre de lignes à afficher (par défaut 10)</param>
        public static void ShowRecentLogs(int numberOfLines = 10)
        {
            try
            {
                if (!File.Exists(_logFileName))
                {
                    Console.WriteLine("📝 Aucun fichier de log trouvé.");
                    return;
                }

                var lines = File.ReadAllLines(_logFileName);
                var startIndex = Math.Max(0, lines.Length - numberOfLines);
                
                Console.WriteLine($"\n📋 Dernières {Math.Min(numberOfLines, lines.Length)} entrées du log:");
                Console.WriteLine("".PadRight(80, '='));
                
                for (int i = startIndex; i < lines.Length; i++)
                {
                    Console.WriteLine(lines[i]);
                }
                
                Console.WriteLine("".PadRight(80, '='));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la lecture du log: {ex.Message}");
            }
        }

        /// <summary>
        /// Nettoie le fichier de log (garde seulement les N derniers jours)
        /// </summary>
        /// <param name="daysToKeep">Nombre de jours à conserver</param>
        public static void CleanupLogs(int daysToKeep = 30)
        {
            try
            {
                if (!File.Exists(_logFileName))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var lines = File.ReadAllLines(_logFileName);
                var filteredLines = new List<string>();

                foreach (var line in lines)
                {
                    // Extraire la date de la ligne de log
                    if (line.Length > 19 && DateTime.TryParse(line.Substring(1, 19), out DateTime logDate))
                    {
                        if (logDate >= cutoffDate)
                        {
                            filteredLines.Add(line);
                        }
                    }
                    else
                    {
                        // Garder les lignes qui ne peuvent pas être parsées
                        filteredLines.Add(line);
                    }
                }

                File.WriteAllLines(_logFileName, filteredLines);
                LogInfo($"Nettoyage du log effectué", $"Logs antérieurs à {daysToKeep} jours supprimés");
            }
            catch (Exception ex)
            {
                LogException("CleanupLogs", ex);
            }
        }

        /// <summary>
        /// Initialise le système de logging avec un message de démarrage
        /// </summary>
        public static void Initialize()
        {
            LogInfo("=== DÉMARRAGE DU GESTIONNAIRE DE CONTACTS ===", $"Version 2.0 avec logging");
        }

        /// <summary>
        /// Finalise le système de logging avec un message d'arrêt
        /// </summary>
        public static void Shutdown()
        {
            LogInfo("=== ARRÊT DU GESTIONNAIRE DE CONTACTS ===", "Session terminée");
        }

        /// <summary>
        /// Obtient des statistiques sur le fichier de log
        /// </summary>
        public static void ShowLogStats()
        {
            try
            {
                if (!File.Exists(_logFileName))
                {
                    Console.WriteLine("📝 Aucun fichier de log trouvé.");
                    return;
                }

                var lines = File.ReadAllLines(_logFileName);
                var fileInfo = new FileInfo(_logFileName);
                
                var infoCount = lines.Count(l => l.Contains("[INFO]"));
                var warningCount = lines.Count(l => l.Contains("[WARNING]"));
                var errorCount = lines.Count(l => l.Contains("[ERROR]"));
                var successCount = lines.Count(l => l.Contains("[SUCCESS]"));

                Console.WriteLine("\n📊 Statistiques du log:");
                Console.WriteLine($"📁 Fichier: {_logFileName}");
                Console.WriteLine($"📏 Taille: {fileInfo.Length} bytes");
                Console.WriteLine($"📝 Total d'entrées: {lines.Length}");
                Console.WriteLine($"ℹ️  Info: {infoCount}");
                Console.WriteLine($"✅ Succès: {successCount}");
                Console.WriteLine($"⚠️  Avertissements: {warningCount}");
                Console.WriteLine($"❌ Erreurs: {errorCount}");
                Console.WriteLine($"📅 Dernière modification: {fileInfo.LastWriteTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la lecture des statistiques: {ex.Message}");
            }
        }
    }
} 