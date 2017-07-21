using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace MasTicket
{
    public class ContactsViewModel
    {
        private readonly IContactService _contactService;

        public ContactsViewModel()
        {
            _contactService = DependencyService.Get<IContactService>();
        }

        public void LoadContacts()
        {
            Contacts = _contactService.All().Result;
        }

        public IEnumerable<UserContact> Contacts { get; set; }
    }

    public interface IContactService
    {
        Task<IEnumerable<UserContact>> All();
        UserContact Find(string searchString);
    }

    public class UserContact
    {
        public UserContact(string firstName, string lastName, string emailId)
        {
            FirstName = firstName;
            LastName = lastName;
            EmailId = emailId;
        }

        public bool IsSelected { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
    }
}
