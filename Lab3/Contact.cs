using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Lab3
{
    public class Contact : ObservableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set<string>(() => this.Name, ref _name, value); }
        }

        private string _workPhone;
        public string WorkPhone
        {
            get { return _workPhone; }
            set { Set<string>(() => this.WorkPhone, ref _workPhone, value); }
        }

        private string _homePhone;
        public string HomePhone
        {
            get { return _homePhone; }
            set { Set<string>(() => this.HomePhone, ref _homePhone, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { Set<string>(() => this.Email, ref _email, value); }
        }

        private string _skype;
        public string Skype
        {
            get { return _skype; }
            set { Set<string>(() => this.Skype, ref _skype, value); }
        }

        private DateTime _birthday;
        public DateTime Birthday
        {
            get { return _birthday; }
            set { Set<DateTime>(() => this.Birthday, ref _birthday, value); }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { Set<string>(() => this.Comment, ref _comment, value); }
        }

        public override string ToString()
        {
            return this.Name;
        }

        // Вычисление возраста
        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - Birthday.Year;
                if (today < Birthday.AddYears(age))
                    age--;
                return age;
            }
        }
        
        // Вычисление оставшихся дней до дня рождения
        public int RemainingDays
        {
            get
            {
                DateTime today = DateTime.Today;
                if (today == Birthday.Date.AddYears(Age)) return 0; // Если день рождения - сегодня, вернуть 0
                DateTime nextBirthday = Birthday.Date.AddYears(Age + 1);

                TimeSpan differrence = nextBirthday - today;

                return Convert.ToInt32(differrence.TotalDays);
            }
        }

        public static ObservableCollectionExtended<Contact> GetSampleContracts()
        {
            ObservableCollectionExtended<Contact> contacts = new ObservableCollectionExtended<Contact>();
            contacts.Add(new Contact
            {
                Name = "Vasya",
                WorkPhone = "111111",
                HomePhone = "112112",
                Email = "testMail.@mail.ku",
                Skype = "111112",
                Birthday = new DateTime(1995, 9, 24),
                Comment = "Comment1"
            });
            contacts.Add(new Contact
            {
                Name = "Petya",
                WorkPhone = "222222",
                HomePhone = "223223",
                Email = "testMail2.@mail.ku",
                Skype = "222223",
                Birthday = new DateTime(1994, 10, 4),
                Comment = "Comment2"
            });
            contacts.Add(new Contact
            {
                Name = "Vova",
                WorkPhone = "333333",
                HomePhone = "334334",
                Email = "testMail2.@mail.ku",
                Skype = "333334",
                Birthday = new DateTime(1996, 1, 15),
                Comment = "Comment3"
            });
            return contacts;
        }
    }
}
