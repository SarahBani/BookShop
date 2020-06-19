using BookShop.ViewModels;
using System.Windows;

namespace BookShop.Views
{
    public partial class MainWindow : Window
    {

        #region Constructors

        public MainWindow()
        {
            this.DataContext = new MainViewModel();
        }

        #endregion /Constructors
    
    }
}
