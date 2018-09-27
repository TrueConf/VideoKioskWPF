using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace VideoKioskWPF
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        Settings settings;
        XmlSerializer formatter = new XmlSerializer(typeof(Settings));
        public SettingsWindow()
        {
            InitializeComponent();
            LoadLastSettings();
        }

        public void LoadLastSettings()
        {
            if (settings == null)
                settings = new Settings();
            if (File.Exists("settings.xml"))
                using (FileStream fs = new FileStream("settings.xml", FileMode.OpenOrCreate))
                {
                    settings = (Settings)formatter.Deserialize(fs);
                }
            DataContext = settings;
            checkBoxLogging.IsChecked = settings.isLogged;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (
                !String.IsNullOrEmpty(txtPassword.Password))
            {
                settings.isLogged = checkBoxLogging.IsChecked.Value;
                settings.password = txtPassword.Password;
                using (FileStream fs = new FileStream("settings.xml", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, settings);
                }
                Close();
            }
            else
            {
                MessageBox.Show("Password cannot be empty");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
