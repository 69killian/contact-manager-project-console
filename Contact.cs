using System;

namespace ContactNamespace
{
    class Contact
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        public Contact()
        {
            Nom = string.Empty;
            Prenom = string.Empty;
            Email = string.Empty;
            Telephone = string.Empty;
        }

        public Contact(string nom, string prenom, string email, string telephone)
        {
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Telephone = telephone;
        }

        public void AjouterContact()
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
    }
}
