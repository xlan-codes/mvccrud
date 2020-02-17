using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfData.Services;
using WpfData.ViewModels;
using WPFMVVMSQLCRUD;

namespace WpfData.Views
{
    /// <summary>
    /// Interaction logic for PeopleView.xaml
    /// </summary>
    public partial class PeopleView : UserControl
    {
        private PeopleViewModel _vm;

        public PeopleView()
        {
            InitializeComponent();
            IServiceAgent sa = new ServiceAgent();
            _vm = new PeopleViewModel(sa);
            this.DataContext = _vm;

            _vm.LoadPeople();
        }

        private void AddButtonClicked(object sender, RoutedEventArgs e)
        {
            AddPerson addPerson = new AddPerson(this.DataContext);
            addPerson.Show();

        }        
        
        private void EditButtonClicked(object sender, RoutedEventArgs e)
        {
            AddPerson addPerson = new AddPerson(this.DataContext);
            var data = (PeopleViewModel)this.DataContext;
            data.EditPerson();
            addPerson.Show();

        }
    }
}
