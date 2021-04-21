using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Symmetrisk_Encryption_Example
{
    class MainViewModel : INotifyPropertyChanged
    {
        private Encryption encryption;
        private ObservableCollection<EncryptionType> ecryptionOptions;
        private EncryptionType selectedEncryptionType;
        private string planTextUTF8 = "";
        private string encryptedBase64 = "";
        private string encryptionTime;
        private string decryptionTime;

        public ICommand GenerateCommand { get; set; }
        public ICommand EncryptCommand { get; set; }
        public ICommand DecryptCommand { get; set; }

        public string DecryptionTime
        {
            get { return "Decryption Time: " + decryptionTime; }
            set { decryptionTime = value;
                OnPropertyChanged("DecryptionTime");
            }
        }
        public string EncryptionTime
        {
            get { return "Encryption Time: " + encryptionTime; }
            set { encryptionTime = value;
                OnPropertyChanged("EncryptionTime");
            }
        }
        public string IV
        {
            get { return Convert.ToBase64String(encryption.IV); }
            set { encryption.IV = Convert.FromBase64String(value); }
        }
        public string Key
        {
            get { return Convert.ToBase64String(encryption.Key); }
            set { encryption.Key = Convert.FromBase64String(value); }
        }
        public string EncryptedHex
        {
            get
            {
                try
                {
                    return BitConverter.ToString(Convert.FromBase64String(EncryptedBase64));
                }
                catch { return "invalid input"; }
            }
            set { }
        }
        public string EncryptedBase64
        {
            get { return encryptedBase64; }
            set
            {
                encryptedBase64 = value;
                OnPropertyChanged("EncryptedBase64");
                OnPropertyChanged("encryptedHex");
            }
        }
        public string PlanTextHex
        {
            get
            {
                try
                {
                    return BitConverter.ToString(Encoding.UTF8.GetBytes(planTextUTF8));
                }
                catch { return "invalid input"; }
            }
            set { }
        }
        public string PlanTextUTF8
        {
            get { return planTextUTF8; }
            set
            {
                planTextUTF8 = value;
                OnPropertyChanged("PlanTextUTF8");
                OnPropertyChanged("PlanTextHex");
            }
        }
        public EncryptionType SelectedEncryptionType
        {
            get { return selectedEncryptionType; }
            set 
            { 
                selectedEncryptionType = value;
                encryption.SelectAlgorithm(selectedEncryptionType.Type);
                GenerateSet();
                DecryptionTime = "";
                EncryptionTime = "";
            }
        }
        public ObservableCollection<EncryptionType> EcryptionOptions
        {
            get { return ecryptionOptions; }
            set { ecryptionOptions = value; }
        }


        public MainViewModel()
        {
            encryption = new Encryption();
            GenerateCommand = new RelayCommand(new Action<object>(GenerateSet));
            EncryptCommand = new RelayCommand(new Action<object>(Encrypt));
            DecryptCommand = new RelayCommand(new Action<object>(Decrypt));
            // Filles the options with the definent supported encryption types
            EcryptionOptions = new ObservableCollection<EncryptionType>() {
                new EncryptionType() { KeySize=128, Name = "AES (128bit)", Type = EncryptionTypeEnum.AES },
                new EncryptionType() { KeySize=192, Name = "AES (192bit)", Type = EncryptionTypeEnum.AES },
                new EncryptionType() { KeySize=256, Name = "AES (256bit)", Type = EncryptionTypeEnum.AES},
                new EncryptionType() { KeySize=64, Name = "DES (64bit)" , Type = EncryptionTypeEnum.DES },
                new EncryptionType() { KeySize=192, Name = "Triple DES (192bit)", Type = EncryptionTypeEnum.TripleDES },
                new EncryptionType() { KeySize=128, Name = "Rijndael (128bit)", Type = EncryptionTypeEnum.Rijndael },
                new EncryptionType() { KeySize=256, Name = "Rijndael (256bit)", Type = EncryptionTypeEnum.Rijndael },
            };
            SelectedEncryptionType = EcryptionOptions[0];
        }

        /// <summary>
        /// Generates IV & Key in the right lenght
        /// </summary>
        public void GenerateSet(object obj = null)
        {
            encryption.GenerateSet(SelectedEncryptionType.KeySize);
            OnPropertyChanged("IV");
            OnPropertyChanged("Key");

        }

        /// <summary>
        /// encrypts the input if it is vaild
        /// </summary>
        public void Encrypt(object obj = null)
        {
            if (PlanTextUTF8 != "" && PlanTextHex != "invalid input")
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(PlanTextUTF8);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                byte[] encryptedBytes = encryption.Encrypt(messageBytes);
                stopwatch.Stop();
                EncryptionTime = stopwatch.Elapsed.ToString();
                EncryptedBase64 = Convert.ToBase64String(encryptedBytes);
            }
        }
        /// <summary>
        /// Decrypts the input if it is valid
        /// </summary>
        public void Decrypt(object obj = null)
        {
            if (EncryptedBase64 != "" && EncryptedHex != "invalid input")
            {
                byte[] encryptedBytes = Convert.FromBase64String(EncryptedBase64);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                byte[] messageBytes = encryption.Decrypt(encryptedBytes);
                stopwatch.Stop();
                DecryptionTime = stopwatch.Elapsed.ToString();
                PlanTextUTF8 = Encoding.UTF8.GetString(messageBytes);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
