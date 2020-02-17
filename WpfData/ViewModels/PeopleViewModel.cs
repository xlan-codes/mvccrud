using System;
using System.Windows;
using System.Threading;
using System.Collections.ObjectModel;
using SimpleMvvmToolkit;
using WpfData.Services;
using WpfData.Models;
using System.Collections.Generic;
using System.Linq;

namespace WpfData.ViewModels
{
    public class PeopleViewModel : ViewModelBase<PeopleViewModel>
    {
        private IServiceAgent _serviceAgent;

        public PeopleViewModel() { }

        public PeopleViewModel(IServiceAgent serviceAgent)
        {
            this.Birthdate = DateTime.Now;
            this._serviceAgent = serviceAgent;
        }

        #region Notifications

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Properties

        private string firstName;
        private string lastName;
        private string birthplace;
        private string martialStatus;
        private string searchTextbox;
        private string phone;
        private DateTime birthdate;
        private int userId;
        private bool employed;
        private char gender;

        private static readonly KeyValuePair<string, string>[] martialStatusList = {
            new KeyValuePair<string, string>("I Martuar", "I Martuar"),
            new KeyValuePair<string, string>("Beqar", "Beqar"),
            new KeyValuePair<string, string>("Ve", "Ve"),
            new KeyValuePair<string, string>("I Ndare", "I Ndare"),
        };

        public KeyValuePair<string, string>[] MartialStatusList
        {
            get
            {
                return martialStatusList;
            }
        }

        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                NotifyPropertyChanged(m => m.FirstName);
            }
        }
        public string SearchTextBox
        {
            get { return searchTextbox; }
            set
            {
                searchTextbox = value;
                NotifyPropertyChanged(m => m.SearchTextBox);
            }
        }

        public int UserId {
            get { return userId; }
            set
            {
                userId = value;
                NotifyPropertyChanged(m => m.UserId);
            }
        }
        public string LastName {
            get { return lastName; }
            set
            {
                lastName = value;
                NotifyPropertyChanged(m => m.LastName);
            }
        }
        public string Birthplace {
            get { return birthplace; }
            set
            {
                birthplace = value;
                NotifyPropertyChanged(m => m.Birthplace);
            }
        }
        public DateTime Birthdate {
            get { return birthdate; }
            set
            {
                birthdate = value;
                NotifyPropertyChanged(m => m.Birthdate);
            }
        }
        public string MartialStatus {
            get { return martialStatus; }
            set
            {
                martialStatus = value;
                NotifyPropertyChanged(m => m.MartialStatus);
            }
        }
        public bool Employed
        {
            get { return employed; }
            set
            {
                employed = value;
                NotifyPropertyChanged(m => m.Employed);
            }
        }
        public char Gender {
            get { return gender; }
            set
            {
                gender = value;
                NotifyPropertyChanged(m => m.Gender);
            }
        }
        public string Phone {
            get { return phone; }
            set
            {
                phone = value;
                NotifyPropertyChanged(m => m.Phone);
            }
        }

        bool isMale;
        bool isFemale;
        public bool IsMale {
            get { return isMale; }
            set
            {
                isMale = true;
                Gender = 'M';
                NotifyPropertyChanged(m => m.IsMale);
            }
        }

        public bool IsFemale {
            get { return isFemale; }
            set
            {
                isFemale = value;
                Gender = 'F';
                NotifyPropertyChanged(m => m.IsFemale);
            }
        }


        private ObservableCollection<Person> people;
        public ObservableCollection<Person> People
        {
            get { return people; }
            set
            {
                people = value;
                NotifyPropertyChanged(m => m.People);
            }
        }

        private Person selectedPerson;
        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                selectedPerson = value;
                //this.Message = selectedPerson.FirstName;
                NotifyPropertyChanged(m => m.SelectedPerson);
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                NotifyPropertyChanged(m => m.message);
            }
        }

        #endregion

        #region Methods

        public void LoadPeople()
        {
            _serviceAgent.GetPeople((people, error) => PeopleLoaded(people, error));
        }
        private void AddPerson()
        {
            //if(this.SelectedPerson.Mode == emMode.update)
            //{
            //    this.SelectedPerson.FirstName = this.FirstName;
            //    this.SelectedPerson.LastName = this.LastName;
            //    this.SelectedPerson.Birthplace = this.Birthplace;
            //    this.SelectedPerson.Birthdate = this.Birthdate;
            //    this.SelectedPerson.Employed = this.Employed;
            //    this.SelectedPerson.Phone = this.Phone;
            //    if (this.SelectedPerson.Gender == 'M')
            //        this.IsMale = true;
            //    else
            //        this.IsFemale = true;

            //    this.SelectedPerson.MartialStatus = this.MartialStatus;
            //    _serviceAgent.Flush(this.People, (error) => PeopleFlushed(error));
            //} else
            //{            Message = "";
            Boolean isValid = ValidateInput();
            if(isValid && UserId == 0)
            {
                this.People.Insert(0, new Person { FirstName = FirstName, LastName = LastName, Birthdate = Birthdate, Birthplace = Birthplace, Gender = Gender, Phone = Phone, MartialStatus = MartialStatus, Employed = Employed });
                _serviceAgent.Flush(this.People, (error) => PeopleFlushed(error));
                Message = "";
                this.CloseWindow();
            } else if(isValid)
            {
                Person person = new Person { Mode = emMode.update, UserId = this.UserId, FirstName = FirstName, LastName = LastName, Birthdate = Birthdate, Birthplace = Birthplace, Gender = Gender, Phone = Phone, MartialStatus = MartialStatus, Employed = Employed };
                var found = People.FirstOrDefault(x => x.UserId == this.UserId);
                int i = People.IndexOf(found);
                People[i] = person;
                _serviceAgent.Flush(this.People, (error) => PeopleFlushed(error));
                this.CloseWindow();
            }

            //}

        }

        private void CloseWindow()
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item.DataContext == this) item.Close();
            }
        }


        public void EditPerson()
        {

            Boolean isValid = ValidateInput();
            this.UserId = this.SelectedPerson.UserId;
            this.FirstName = this.SelectedPerson.FirstName;
            this.LastName = this.SelectedPerson.LastName;
            this.Birthplace = this.SelectedPerson.Birthplace;
            this.Birthdate = this.SelectedPerson.Birthdate;
            this.Employed = this.SelectedPerson.Employed;
            this.Phone = this.SelectedPerson.Phone;
            if (this.SelectedPerson.Gender == 'M')
                this.IsMale = true;
            else
                this.IsFemale = true;

            this.MartialStatus = this.SelectedPerson.MartialStatus;

            this.SelectedPerson.Mode = emMode.update;
            Message = "";
            //_serviceAgent.Flush(this.People, (error) => PeopleFlushed(error));
        }

        private Boolean ValidateInput()
        {
            this.Message = "";
            if(this.birthplace.Equals(""))
            {
                Message = "Vendos Venddlindjen";
                return false;
            }            
            
            if(this.firstName.Equals(""))
            {
                Message = "Vendos Emerin";
                return false;
            }            
            
            if(this.lastName.Equals(""))
            {
                Message = "Vendos Mbiemrin";
                return false;
            }      
            
            if(this.Phone.Equals(""))
            {
                Message = "Vendos Numerin e telefonit";
                return false;
            }
            
            if(!this.isMale && !isFemale)
            {
                Message = "Zgjidhni gjinin";
                return false;
            }

            return true;
        }

        private void DeletePerson()
        {
            if(selectedPerson != null)
            {
                this.SelectedPerson.Mode = emMode.delete;
                _serviceAgent.Flush(this.People, (error) => PeopleFlushed(error));
                _serviceAgent.GetPeople((people, error) => PeopleLoaded(people, error));
            }
            
        }

        private void Search()
        {
            if(SearchTextBox != null)
            {
                _serviceAgent.GetPeople((personlist, error) => {
                var result = from person in personlist where person.FirstName.Contains(SearchTextBox) select person;
                var people = new ObservableCollection<Person>(result);
                    PeopleLoaded(people, error); 
                });
            } 
            
        }

        #endregion

        #region Callbacks

        private void PeopleLoaded(ObservableCollection<Person> people, Exception error)
        {
            if (error == null)
            {
                this.People = people;
                NotifyError("Loaded", null);
            }
            else
            {
                NotifyError(error.Message, error);
            }
            // isbusy = false;
        }

        private void PeopleFlushed(Exception error)
        {
            if (error == null)
                NotifyError("Flushed", null);
            else
                NotifyError(error.Message, error);
        }

        #endregion

        #region Commands

        private DelegateCommand addCommand;
        public DelegateCommand AddCommand
        {
            get
            {
                Message = "";
                this.FirstName = "";
                this.LastName = "";
                this.Birthplace = "";
                //this.Gender 
                this.Phone = "";
                this.Birthdate = DateTime.Now;
                this.Employed = false;
                this.MartialStatus = "";
                this.UserId = 0;
                if (addCommand == null)
                    addCommand = new DelegateCommand(AddPerson);
                return addCommand;
            }
            private set
            {
                addCommand = value;
            }
        }

        private DelegateCommand editCommand;
        public DelegateCommand EditCommand
        {
            get
            {
                if (editCommand == null)
                {
                    editCommand = new DelegateCommand(EditPerson);
                }
                Message = "";
                return editCommand;
            }
            private set
            {
                editCommand = value;
            }
        }

        private DelegateCommand deleteCommand;
        public DelegateCommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new DelegateCommand(DeletePerson);
                return deleteCommand;
            }
            private set
            {
                deleteCommand = value;
            }
        }

        private DelegateCommand searchCommand;
        public DelegateCommand SearchCommand
        {
            get
            {
                if (searchCommand == null) searchCommand = new DelegateCommand(Search);
                return searchCommand;
            }
            private set
            {
                searchCommand = value;
            }
        }

        #endregion 

        #region Helpers

        // Helper method to notify View of an error
        private void NotifyError(string message, Exception error)
        {
            this.Message = message;
            // Notify view of an error
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}