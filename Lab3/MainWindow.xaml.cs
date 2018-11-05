using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab3.ViewModel;

namespace Lab3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ContactViewModel contactViewModel;
        public MainWindow()
        {
            InitializeComponent();
            contactViewModel = ServiceLocator.Current.GetInstance<ContactViewModel>(); // Получение текущей ViewModel
            Closing += OnWindowClosing;
        }
        
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Сохранение контактов при закрытии приложения
            contactViewModel.SaveContactsMethod();
        }
    }
}
