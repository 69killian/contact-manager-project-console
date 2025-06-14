using System;
using System.Collections.Generic;
using System.IO;
using ContactNamespace;
using System.Linq;
using ContactManager.Logging;

namespace GestionContact
{
    /// <summary>
    /// Classe héritant de Contact et contenant toutes les méthodes de gestion
    /// Avec système de logging intégré pour tracer toutes les actions
    /// </summary>
    public class GestionContact : Contact
    {
        // Chemin du fichier de sauvegarde des contacts
        private static string fichierContacts = "contacts.txt";

        /// <summary>
        /// Permet à l'utilisateur d'ajouter un nouveau contact en saisissant ses informations
        /// Vérifie les doublons avant l'ajout
        /// </summary>
        public void AjouterContact()
        {
            try
            {
                Logger.LogInfo("Début d'ajout de contact (méthode instance)");
                
                Console.WriteLine("=== Ajout d'un nouveau contact ===");
                Console.WriteLine("Entrez le nom du contact : ");
                Nom = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Entrez le prenom du contact : ");
                Prenom = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Entrez l'email du contact : ");
                Email = Console.ReadLine() ?? string.Empty;
                Console.WriteLine("Entrez le numero de telephone du contact : ");
                Telephone = Console.ReadLine() ?? string.Empty;

                Console.WriteLine($"\nContact créé avec succès !");
                Console.WriteLine($"Nom complet: {this.ToString()}");
                Console.WriteLine($"ID unique: {this.Id}");

                Logger.LogContactAction("AJOUT", this.ToString(), this.Id);
                Logger.LogSuccess("Contact ajouté avec succès", $"Nom: {Nom} {Prenom}");
            }
            catch (Exception ex)
            {
                Logger.LogException("AjouterContact", ex);
                Console.WriteLine($"Erreur lors de l'ajout du contact : {ex.Message}");
            }
        }

        /// <summary>
        /// Version statique pour ajouter un contact avec vérification des doublons
        /// </summary>
        public static void AjouterContactAvecVerification(List<Contact> ContactList)
        {
            try
            {
                Logger.LogInfo("Début d'ajout de contact avec vérification");
                
                Console.WriteLine("=== Ajout d'un nouveau contact ===");
                Console.Write("Entrez le nom du contact : ");
                string nom = Console.ReadLine() ?? string.Empty;
                Console.Write("Entrez le prénom du contact : ");
                string prenom = Console.ReadLine() ?? string.Empty;
                Console.Write("Entrez l'email du contact : ");
                string email = Console.ReadLine() ?? string.Empty;

                // Vérifier les doublons
                if (ContactExiste(ContactList, nom, prenom, email))
                {
                    Logger.LogWarning("Tentative d'ajout d'un doublon détectée", $"{nom} {prenom} - {email}");
                    Console.WriteLine("⚠️  Un contact avec ces informations existe déjà !");
                    Console.Write("Voulez-vous continuer quand même ? (O/N) : ");
                    string? reponse = Console.ReadLine()?.ToLower();
                    if (reponse != "o" && reponse != "oui")
                    {
                        Logger.LogInfo("Ajout annulé par l'utilisateur", "Doublon détecté");
                        Console.WriteLine("Ajout annulé.");
                        return;
                    }
                    Logger.LogInfo("Utilisateur a choisi de continuer malgré le doublon");
                }

                Console.Write("Entrez le numéro de téléphone du contact : ");
                string telephone = Console.ReadLine() ?? string.Empty;

                var nouveauContact = new Contact(nom, prenom, email, telephone);
                ContactList.Add(nouveauContact);

                Console.WriteLine($"\n✅ Contact créé avec succès !");
                Console.WriteLine($"Nom complet: {nouveauContact.ToString()}");
                Console.WriteLine($"ID unique: {nouveauContact.Id}");

                Logger.LogContactAction("AJOUT_AVEC_VERIFICATION", nouveauContact.ToString(), nouveauContact.Id);
                Logger.LogSuccess("Contact ajouté avec succès", $"Total contacts: {ContactList.Count}");
            }
            catch (Exception ex)
            {
                Logger.LogException("AjouterContactAvecVerification", ex);
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
                Logger.LogInfo("Début de suppression de contact", $"Nombre de contacts: {ContactList.Count}");
                
                Console.WriteLine("\nListe des contacts :");
                Console.WriteLine("--------------------");
                for (int i = 0; i < ContactList.Count; i++)
                {
                    Contact c = ContactList[i];
                    Console.WriteLine($"[{i + 1}] {c.ToString()} (ID: {c.Id.ToString()[..8]}...)");
                }
                Console.WriteLine("Nombre de contacts : " + ContactList.Count);

                Console.WriteLine("\nEntrez le numéro du contact à supprimer : ");
                if (int.TryParse(Console.ReadLine(), out int choix) && choix >= 1 && choix <= ContactList.Count)
                {
                    Contact contactASupprimer = ContactList[choix - 1];
                    Logger.LogInfo("Contact sélectionné pour suppression", $"{contactASupprimer.ToString()} | ID: {contactASupprimer.Id}");
                    
                    Console.WriteLine($"\nVoulez-vous vraiment supprimer le contact : {contactASupprimer.ToString()} ? (O/N)");
                    string? reponse = Console.ReadLine()?.ToLower() ?? "n";

                    if (reponse == "o" || reponse == "oui")
                    {
                        ContactList.RemoveAt(choix - 1);
                        Console.WriteLine("Contact supprimé avec succès !");
                        
                        Logger.LogContactAction("SUPPRESSION", contactASupprimer.ToString(), contactASupprimer.Id);
                        Logger.LogSuccess("Contact supprimé avec succès", $"Contacts restants: {ContactList.Count}");
                    }
                    else
                    {
                        Console.WriteLine("Suppression annulée !");
                        Logger.LogInfo("Suppression annulée par l'utilisateur", contactASupprimer.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Numéro de contact invalide !");
                    Logger.LogWarning("Numéro de contact invalide saisi", $"Valeur: {choix}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("SupprimerContact", ex);
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
                Logger.LogInfo("Début de modification de contact", $"Nombre de contacts: {ContactList.Count}");
                
                if (ContactList.Count == 0)
                {
                    Console.WriteLine("Aucun contact à modifier !");
                    Logger.LogWarning("Tentative de modification avec liste vide");
                    return;
                }

                Console.WriteLine("\nListe des contacts :");
                Console.WriteLine("--------------------");
                for (int i = 0; i < ContactList.Count; i++)
                {
                    Contact c = ContactList[i];
                    Console.WriteLine($"[{i + 1}] {c.ToString()} (ID: {c.Id.ToString()[..8]}...)");
                }

                Console.WriteLine("\nEntrez le numéro du contact à modifier : ");
                if (int.TryParse(Console.ReadLine(), out int choix) && choix >= 1 && choix <= ContactList.Count)
                {
                    int index = choix - 1;
                    Contact c = ContactList[index];
                    
                    Logger.LogInfo("Contact sélectionné pour modification", $"{c.ToString()} | ID: {c.Id}");
                    
                    // Sauvegarder les valeurs originales pour le log
                    string originalInfo = c.ToString();
                    
                    Console.WriteLine($"\nModification du contact : {c.ToString()}");
                    Console.WriteLine($"ID: {c.Id}");
                    Console.WriteLine("(Appuyez sur Entrée sans rien écrire pour garder la valeur actuelle)");

                    Console.Write($"Nom actuel : {c.Nom}\nNouveau nom : ");
                    string nouveauNom = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(nouveauNom)) 
                    {
                        Logger.LogInfo($"Modification nom: {c.Nom} -> {nouveauNom}", $"ID: {c.Id}");
                        c.Nom = nouveauNom;
                    }

                    Console.Write($"Prénom actuel : {c.Prenom}\nNouveau prénom : ");
                    string nouveauPrenom = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(nouveauPrenom)) 
                    {
                        Logger.LogInfo($"Modification prénom: {c.Prenom} -> {nouveauPrenom}", $"ID: {c.Id}");
                        c.Prenom = nouveauPrenom;
                    }

                    Console.Write($"Email actuel : {c.Email}\nNouvel email : ");
                    string nouvelEmail = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(nouvelEmail)) 
                    {
                        Logger.LogInfo($"Modification email: {c.Email} -> {nouvelEmail}", $"ID: {c.Id}");
                        c.Email = nouvelEmail;
                    }

                    Console.Write($"Téléphone actuel : {c.Telephone}\nNouveau téléphone : ");
                    string nouveauTelephone = Console.ReadLine() ?? "";
                    if (!string.IsNullOrEmpty(nouveauTelephone)) 
                    {
                        Logger.LogInfo($"Modification téléphone: {c.Telephone} -> {nouveauTelephone}", $"ID: {c.Id}");
                        c.Telephone = nouveauTelephone;
                    }

                    Console.WriteLine("\nContact modifié avec succès !");
                    
                    Logger.LogContactAction("MODIFICATION", $"{originalInfo} -> {c.ToString()}", c.Id);
                    Logger.LogSuccess("Contact modifié avec succès", $"Nouvelles infos: {c.ToString()}");
                }
                else
                {
                    Console.WriteLine("Numéro de contact invalide !");
                    Logger.LogWarning("Numéro de contact invalide pour modification", $"Valeur: {choix}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("ModifierContact", ex);
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
                Logger.LogInfo("Début de recherche de contact par nom");
                
                Console.WriteLine("Entrez le nom du contact à rechercher : ");
                string nom = Console.ReadLine() ?? string.Empty;
                
                Logger.LogInfo("Recherche en cours", $"Nom recherché: '{nom}'");
                
                Contact? contact = ContactList.Find(c => c.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase));
                if (contact != null)
                {
                    Console.WriteLine($"Contact trouvé : {contact.ToString()}");
                    Console.WriteLine($"ID: {contact.Id}");
                    
                    Logger.LogSuccess("Contact trouvé", $"{contact.ToString()} | ID: {contact.Id}");
                }
                else
                {
                    Console.WriteLine("Contact non trouvé !");
                    Logger.LogWarning("Contact non trouvé", $"Nom recherché: '{nom}'");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("RechercherContact", ex);
                Console.WriteLine($"Erreur lors de la recherche : {ex.Message}");
            }
        }

        /// <summary>
        /// Recherche un contact par son ID unique
        /// </summary>
        public static void RechercherContactParId(List<Contact> ContactList)
        {
            try
            {
                Logger.LogInfo("Début de recherche de contact par ID");
                
                Console.WriteLine("Entrez l'ID du contact à rechercher (ou les premiers caractères) : ");
                string idInput = Console.ReadLine() ?? string.Empty;
                
                Logger.LogInfo("Recherche par ID en cours", $"ID recherché: '{idInput}'");
                
                Contact? contact = null;
                
                // Essayer d'abord une correspondance exacte
                if (Guid.TryParse(idInput, out Guid id))
                {
                    contact = ContactList.Find(c => c.Id.Equals(id));
                    Logger.LogDebug("Recherche par ID complet", $"ID: {id}");
                }
                else if (idInput.Length >= 8)
                {
                    // Recherche par début d'ID (au moins 8 caractères)
                    contact = ContactList.Find(c => c.Id.ToString().StartsWith(idInput, StringComparison.OrdinalIgnoreCase));
                    Logger.LogDebug("Recherche par ID partiel", $"Début ID: '{idInput}'");
                }
                else
                {
                    Logger.LogWarning("ID trop court pour la recherche", $"Longueur: {idInput.Length}");
                }
                
                if (contact != null)
                {
                    Console.WriteLine($"Contact trouvé : {contact.ToString()}");
                    Console.WriteLine($"ID complet: {contact.Id}");
                    
                    Logger.LogSuccess("Contact trouvé par ID", $"{contact.ToString()} | ID: {contact.Id}");
                }
                else
                {
                    Console.WriteLine("Contact non trouvé ! Vérifiez l'ID ou utilisez au moins 8 caractères.");
                    Logger.LogWarning("Contact non trouvé par ID", $"ID recherché: '{idInput}'");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("RechercherContactParId", ex);
                Console.WriteLine($"Erreur lors de la recherche : {ex.Message}");
            }
        }

        /// <summary>
        /// Trie la liste des contacts par nom ou prénom, dans l'ordre ascendant ou descendant
        /// </summary>
        public static void TrierContacts(List<Contact> ContactList)
        {
            try
            {
                Logger.LogInfo("Début de tri des contacts", $"Nombre de contacts: {ContactList.Count}");
                
                if (ContactList.Count == 0)
                {
                    Console.WriteLine("Aucun contact à trier !");
                    Logger.LogWarning("Tentative de tri avec liste vide");
                    return;
                }

                Console.WriteLine("\nTrier les contacts par :");
                Console.WriteLine("1. Nom");
                Console.WriteLine("2. Prénom");
                Console.Write("Votre choix : ");

                if (!int.TryParse(Console.ReadLine(), out int choix) || (choix != 1 && choix != 2))
                {
                    Console.WriteLine("Choix invalide !");
                    Logger.LogWarning("Choix de tri invalide", $"Valeur: {choix}");
                    return;
                }

                Console.Write("Ordre ascendant (A) ou descendant (D) ? ");
                string? ordre = Console.ReadLine()?.ToUpper();
                
                if (ordre != "A" && ordre != "D")
                {
                    Console.WriteLine("Choix invalide !");
                    Logger.LogWarning("Ordre de tri invalide", $"Valeur: {ordre}");
                    return;
                }

                string critere = choix == 1 ? "Nom" : "Prénom";
                string direction = ordre == "A" ? "Ascendant" : "Descendant";
                
                Logger.LogInfo($"Tri en cours", $"Critère: {critere} | Direction: {direction}");

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
                    Console.WriteLine($"{c.ToString()} (ID: {c.Id.ToString()[..8]}...)");
                }
                Console.WriteLine("------------------------");
                Console.WriteLine("Contacts triés avec succès !");
                
                Logger.LogSuccess("Tri terminé avec succès", $"Critère: {critere} | Direction: {direction} | Contacts: {ContactList.Count}");
            }
            catch (Exception ex)
            {
                Logger.LogException("TrierContacts", ex);
                Console.WriteLine($"Erreur lors du tri : {ex.Message}");
            }
        }

        /// <summary>
        /// Vérifie s'il existe déjà un contact avec les mêmes informations (évite les doublons)
        /// </summary>
        public static bool ContactExiste(List<Contact> ContactList, string nom, string prenom, string email)
        {
            try
            {
                Logger.LogDebug("Vérification d'existence de contact", $"{nom} {prenom} - {email}");
                
                bool existe = ContactList.Any(c => 
                    c.Nom.Equals(nom, StringComparison.OrdinalIgnoreCase) &&
                    c.Prenom.Equals(prenom, StringComparison.OrdinalIgnoreCase) &&
                    c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                
                if (existe)
                {
                    Logger.LogWarning("Contact existant détecté", $"{nom} {prenom} - {email}");
                }
                else
                {
                    Logger.LogDebug("Contact non existant", $"{nom} {prenom} - {email}");
                }
                
                return existe;
            }
            catch (Exception ex)
            {
                Logger.LogException("ContactExiste", ex);
                return false; // En cas d'erreur, on considère que le contact n'existe pas
            }
        }

        /// <summary>
        /// Affiche la liste complète des contacts
        /// </summary>
        public static void ListerContacts(List<Contact> ContactList)
        {
            try
            {
                Logger.LogInfo("Affichage de la liste des contacts", $"Nombre: {ContactList.Count}");
                
                Console.WriteLine("\nListe des contacts :");
                Console.WriteLine("--------------------");
                foreach (Contact c in ContactList)
                {
                    Console.WriteLine($"{c.ToString()} (ID: {c.Id.ToString()[..8]}...)");
                }
                Console.WriteLine("Nombre de contacts : " + ContactList.Count);
                Console.WriteLine("--------------------");
                
                Logger.LogSuccess("Liste des contacts affichée", $"Total: {ContactList.Count} contacts");
            }
            catch (Exception ex)
            {
                Logger.LogException("ListerContacts", ex);
                Console.WriteLine($"Erreur lors de l'affichage de la liste : {ex.Message}");
            }
        }

        /// <summary>
        /// Sauvegarde la liste des contacts dans un fichier texte
        /// Format : ID;Nom;Prénom;Email;Téléphone (un contact par ligne)
        /// </summary>
        public static void SauvegarderContacts(List<Contact> ContactList)
        {
            try
            {
                Logger.LogInfo("Début de sauvegarde", $"Nombre de contacts: {ContactList.Count}");
                
                using (StreamWriter writer = new StreamWriter(fichierContacts))
                {
                    foreach (Contact c in ContactList)
                    {
                        writer.WriteLine($"{c.Id};{c.Nom};{c.Prenom};{c.Email};{c.Telephone}");
                    }
                }
                Console.WriteLine("Contacts sauvegardés avec succès !");
                
                Logger.LogSuccess("Sauvegarde terminée", $"Fichier: {fichierContacts} | Contacts: {ContactList.Count}");
            }
            catch (Exception ex)
            {
                Logger.LogException("SauvegarderContacts", ex);
                Console.WriteLine($"Erreur lors de la sauvegarde : {ex.Message}");
            }

        }

        /// <summary>
        /// Charge la liste des contacts depuis le fichier texte
        /// Crée une liste vide si le fichier n'existe pas
        /// Format attendu : ID;Nom;Prénom;Email;Téléphone ou Nom;Prénom;Email;Téléphone (rétrocompatibilité)
        /// </summary>
        public static List<Contact> ChargerContacts()
        {
            List<Contact> contacts = new List<Contact>();
            try
            {
                Logger.LogInfo("Début de chargement des contacts", $"Fichier: {fichierContacts}");
                
                if (File.Exists(fichierContacts))
                {
                    int contactsCharges = 0;
                    int anciensFormats = 0;
                    
                    using (StreamReader reader = new StreamReader(fichierContacts))
                    {
                        string? ligne;
                        while ((ligne = reader.ReadLine()) != null)
                        {
                            string[] donnees = ligne.Split(';');
                            
                            if (donnees.Length == 5) // Nouveau format avec ID
                            {
                                if (Guid.TryParse(donnees[0], out Guid id))
                                {
                                    contacts.Add(new Contact(id, donnees[1], donnees[2], donnees[3], donnees[4]));
                                    contactsCharges++;
                                }
                                else
                                {
                                    Logger.LogWarning("ID invalide dans le fichier", $"Ligne: {ligne}");
                                }
                            }
                            else if (donnees.Length == 4) // Ancien format sans ID (rétrocompatibilité)
                            {
                                contacts.Add(new Contact(donnees[0], donnees[1], donnees[2], donnees[3]));
                                contactsCharges++;
                                anciensFormats++;
                            }
                            else
                            {
                                Logger.LogWarning("Format de ligne invalide", $"Ligne: {ligne}");
                            }
                        }
                    }
                    
                    Console.WriteLine("Contacts chargés avec succès !");
                    
                    Logger.LogSuccess("Chargement terminé", 
                        $"Contacts chargés: {contactsCharges} | Anciens formats: {anciensFormats}");
                    
                    if (anciensFormats > 0)
                    {
                        Logger.LogInfo("Migration automatique effectuée", 
                            $"{anciensFormats} contacts migrés vers le nouveau format avec ID");
                    }
                }
                else
                {
                    Logger.LogInfo("Fichier de contacts non trouvé", "Création d'une liste vide");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("ChargerContacts", ex);
                Console.WriteLine($"Erreur lors du chargement : {ex.Message}");
            }
            return contacts;
        }
    }
} 