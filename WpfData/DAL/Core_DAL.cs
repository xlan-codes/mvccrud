using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfData.Models;

namespace WpfData.DAL
{
    public static class Core_DAL
    {
        public static ObservableCollection<Person> FetchPeople()
        {
            return People.GetPeople();
        }

        public static void Flush(ObservableCollection<Person> people)
        {
            People.Flush(people);
        }
        
        //public static void Delete(int UserId)
        //{
        //    People.Delete(UserId);
        //}
    }
}
