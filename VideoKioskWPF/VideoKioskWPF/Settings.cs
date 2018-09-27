using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoKioskWPF
{
    [Serializable]
    public class Settings : INotifyPropertyChanged
    {
        public string server;
        public string login;
        public string password;
        public int camera;
        public int microphone;
        public int speaker;
        public int chat;
        public bool isLogged;
        public string logPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Server
        {
            get { return server; }
            set
            {
                server = value;
                RaisePropertyChanged("Server");
            }
        }
        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                if (String.IsNullOrEmpty(value))
                    throw new ApplicationException("Login cannot be empty");

                RaisePropertyChanged("Login");
            }
        }      
        public string Camera
        {
            get { return camera.ToString(); }
            set
            {
                int i = 0;
                int.TryParse(value, out i);
                camera = i;
                if (!int.TryParse(value, out i))
                    throw new ApplicationException("Camera index must be integer");

                RaisePropertyChanged("Camera");
            }
        }
        
        public string Microphone
        {
            get { return microphone.ToString(); }
            set
            {
                int i = 0;
                int.TryParse(value, out i);
                microphone = i;
                if (!int.TryParse(value, out i))
                    throw new ApplicationException("Microphone index must be integer");

                RaisePropertyChanged("Microphone");
            }
        }
        public string Speaker
        {
            get { return speaker.ToString(); }
            set
            {
                int i = 0;
                int.TryParse(value, out i);
                speaker = i;
                if (!int.TryParse(value, out i))
                    throw new ApplicationException("Speaker index must be integer");

                RaisePropertyChanged("Speaker");
            }
        }

        public string ChatTimeout
        {
            get { return chat.ToString(); }
            set
            {
                int i = 0;
                int.TryParse(value, out i);
                chat = i;
                if (!int.TryParse(value, out i))
                    throw new ApplicationException("Chat timeout must be integer");

                RaisePropertyChanged("ChatTimeout");
            }
        }

        public string LogPath
        {
            get { return logPath; }
            set
            {
                logPath = value;

                RaisePropertyChanged("LogPath");
            }
        }

        public Settings()
        {
            server = "";
        login = "";
        password = "";
        camera = 0;
        microphone = 0;
        speaker = 0;
        chat = 5;
        isLogged = false;
        logPath = "";
    }        
    }
}
