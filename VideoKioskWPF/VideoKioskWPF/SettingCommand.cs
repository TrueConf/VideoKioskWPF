using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VideoKioskWPF
{
    public class SettingCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        SettingsWindow settingsWindow;

        public SettingCommand(SettingsWindow sw)
        {
            settingsWindow = sw;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            settingsWindow.LoadLastSettings();
            settingsWindow.ShowDialog();
        }
    }
}
