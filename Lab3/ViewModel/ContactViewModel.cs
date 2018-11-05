using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Lab3.ViewModel
{
    class ContactViewModel : ViewModelBase
    {
        // Список контактов
        private ObservableCollectionExtended<Contact> _contacts;
        public ObservableCollectionExtended<Contact> Contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                RaisePropertyChanged(() => Contacts);
            }
        }

        // Список букв алфавита для фильтра по именам
        public string[] AlphabetChars
        {
            get { return ",А,Б,В,Г,Д,Е,Ё,Ж,З,И,Й,К,Л,М,Н,О,П,Р,С,Т,У,Ф,Х,Ц,Ч,Ш,Щ,Ы,Э,Ю,Я".Split(','); }
        }

        // Выбранный буква алфавита для фильтра
        private string _alphaFilterChar;
        public Subject<string> AlphaFilterSubject;
        public string AlphaFilterChar
        {
            get { return _alphaFilterChar; }
            set
            {
                _alphaFilterChar = value != null ? value : "";
                RaisePropertyChanged(() => AlphaFilterChar);
                AlphaFilterSubject.OnNext(value);
            }
        }

        // Фильтрация по первой букве имени
        private void AlphaFilter(object sender, FilterEventArgs e)
        {
            Contact contact = e.Item as Contact;
            e.Accepted &= contact.Name != null && contact.Name.StartsWith(AlphaFilterChar);
        }

        // Строка для поиска по свойствам контактов
        private string _searchSubstring;
        public Subject<string> SearchSubstringSubject;
        public string SearchSubstring
        {
            get { return _searchSubstring; }
            set
            {
                _searchSubstring = value;
                RaisePropertyChanged(() => SearchSubstring);
                SearchSubstringSubject.OnNext(value);
            }
        }

        // Фильтр - проверка на содержание поисковой строки в одном из свойств
        private void FilterByWord(object sender, FilterEventArgs e)
        {
            Contact contact = e.Item as Contact;

            bool NotNullAndContains(string s)
            {
                if (s != null && s.IndexOf(SearchSubstring, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                else return false;
            }

            e.Accepted &= NotNullAndContains(contact.Name)
                          || NotNullAndContains(contact.WorkPhone)
                          || NotNullAndContains(contact.HomePhone)
                          || NotNullAndContains(contact.Email)
                          || NotNullAndContains(contact.Skype)
                          || NotNullAndContains(contact.Comment);
        }

        // Отфильтрованные по алфавиту и поисковой строке контакты
        private CollectionViewSource _filteredContacts;
        public CollectionViewSource FilteredContacts
        {
            get { return _filteredContacts; }
        }

        // Следующие дни рождения
        private CollectionViewSource _nextBirthdays;
        public CollectionViewSource NextBirthdays
        {
            get { return _nextBirthdays; }
        }

        // Выбранный контакт
        private Contact _selectedContact;
        public Subject<Contact> SelectedContactSubject;
        public Contact SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                RaisePropertyChanged(() => SelectedContact);
                SelectedContactSubject.OnNext(value);
            }
        }

        // Команда для удаления выбранного контакта
        private RelayCommand _removeContactCommand;
        public RelayCommand RemoveContactCommand
        {
            get
            {
                return _removeContactCommand ??
                       (_removeContactCommand = new RelayCommand(RemoveContactMethod, () => SelectedContact != null)); // Если контакт не выбран - команда недоступна
            }
            private set { _removeContactCommand = value; }
        }

        // Метод для удаления контакта
        public void RemoveContactMethod()
        {
            if (SelectedContact == null) return;

            Contacts.Remove(SelectedContact);
            this.RaisePropertyChanged(() => this.Contacts);
        }

        // Команда для добавления нового контакта
        public ICommand AddContactCommand { get; private set; }

        // Метод для добавления контакта
        public void AddContactMethod()
        {
            Contact c = new Contact { Name = "Новый контакт" };
            _contacts.Add(c);
            this.RaisePropertyChanged(() => this.Contacts);
            SelectedContact = c;
        }

        // Метод для загрузки контактов из файла (или создания пустой коллекции, если файла нет)
        public void LoadContactsMethod()
        {
            if (File.Exists(@"contacts.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                using (JsonReader reader = new JsonTextReader(new StreamReader(@"contacts.json")))
                {
                    Contacts = serializer.Deserialize<ObservableCollectionExtended<Contact>>(reader);
                }
            }
            else
            {
                Contacts = new ObservableCollectionExtended<Contact>();
            }

            // Перепривязывание и обновление CollectionView
            _filteredContacts.Source = _contacts;
            _filteredContacts.View.Refresh();

            _nextBirthdays.Source = _contacts;
            _nextBirthdays.View.Refresh();
        }

        // Метод для сохранения контактов в файл
        public void SaveContactsMethod()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(@"contacts.json")))
            {
                serializer.Serialize(writer, Contacts);
            }
        }

        // Инициализация значений
        private void InitValues()
        {
            AlphaFilterSubject = new Subject<string>();
            _alphaFilterChar = "";
            AlphaFilterSubject.Subscribe(af => _filteredContacts.View.Refresh());

            SearchSubstringSubject = new Subject<string>();
            _searchSubstring = "";
            SearchSubstringSubject.Subscribe(ss => _filteredContacts.View.Refresh());

            SelectedContactSubject = new Subject<Contact>();
            _selectedContact = null;
            SelectedContactSubject.Subscribe(sc => RemoveContactCommand.RaiseCanExecuteChanged());

            _filteredContacts = new CollectionViewSource();
            _filteredContacts.Filter += new FilterEventHandler(FilterByWord);
            _filteredContacts.Filter += new FilterEventHandler(AlphaFilter);

            _nextBirthdays = new CollectionViewSource();
            _nextBirthdays.SortDescriptions.Add(new SortDescription("RemainingDays", ListSortDirection.Ascending));

            AddContactCommand = new RelayCommand(AddContactMethod);

        }

        public ContactViewModel()
        {
            InitValues();
            LoadContactsMethod();
        }
    }
}
