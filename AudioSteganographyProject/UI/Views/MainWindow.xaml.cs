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
using AudioSteganographyProject.Classes;
using AudioSteganographyProject.UI.Models;

namespace AudioSteganographyProject.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainWindowModel mainModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            mainModel = new MainWindowModel(this);
            setCurrentPage(mainModel.currentPage);
        }

        private void onHideSongButton_Click(object sender, RoutedEventArgs e)
        {
            this.setCurrentPage(PageKeys.HideSong);
        }

        private void onReadHiddenInfoButton_Click(object sender, RoutedEventArgs e)
        {
            this.setCurrentPage(PageKeys.ReadHiddenInfo);
        }

        private void onAboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.setCurrentPage(PageKeys.About);
        }

        public void setCurrentPage(PageKeys page)
        {
            mainModel.updateModel();
            mainModel.setCurrentPage(page);
            MainDisplayFrame.Content = mainModel.getCurrentPage();
        }
    }
}
