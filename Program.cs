using System;
using ContactNamespace;

namespace ContactManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Contact contact = new Contact();
            contact.AjouterContact();

            List<Contact> ContactList = new List<Contact>();

            ContactList.Add(contact);

            
            /*foreach (Contact c in ContactList)
            {
                Console.WriteLine(c.Nom);
                Console.WriteLine(c.Prenom);
                Console.WriteLine(c.Email);
                Console.WriteLine(c.Telephone);
            }*/
        }
    }
}
