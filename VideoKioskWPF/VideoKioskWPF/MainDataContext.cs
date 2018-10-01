using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoKioskWPF
{
    class MainDataContext : INotifyPropertyChanged
    {
        ImageSource _imageBG;
        ImageSource _imageClickBG;
        string _messageHeader;
        string _message;
        public ImageSource ImageBG
        {
            get
            {
                return _imageBG;
            }
            set
            {
                _imageBG = value;
                RaisePropertyChanged("ImageBG");
            }
        }
        public ImageSource ImageClickBG
        {
            get
            {
                return _imageClickBG;
            }
            set
            {
                _imageClickBG = value;
                RaisePropertyChanged("ImageClickBG");
            }
        }
        public string MessageHeader
        {
            get
            {
                return _messageHeader;
            }
            set
            {
                _messageHeader = value;
                RaisePropertyChanged("MessageHeader");
            }
        }
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainDataContext()
        {
            _imageBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Connecting.png"));
            _imageClickBG = new BitmapImage(new Uri("pack://application:,,,/Resources/Connecting-click.png"));            
        }
    }
}
