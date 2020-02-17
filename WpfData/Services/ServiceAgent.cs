using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfData.DAL;
using WpfData.Models;

namespace WpfData.Services
{
    public interface IServiceAgent
    {
        void GetPeople(Action<ObservableCollection<Person>, Exception> completed);
        void Flush(ObservableCollection<Person> list, Action<Exception> completed);
    }

    public class ServiceAgent : IServiceAgent
    {
        public void GetPeople(Action<ObservableCollection<Person>, Exception> completed)
        {
            try
            {
                ObservableCollection<Person> people = Core_DAL.FetchPeople();
                completed(people, null);
            }
            catch (Exception e)
            {
                completed(null, e);
            }
        }

        public void Flush(ObservableCollection<Person> list, Action<Exception> completed)
        {
            try
            {
                Core_DAL.Flush(list);
                completed(null);
            }
            catch (Exception e)
            {
                completed(e);
            }
        }
    }
}
