using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Project.Login
{
    public class HomeVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private string username = string.Empty;
        private string password = string.Empty;
        private string username2 = string.Empty;
        private string password2 = string.Empty;
        private DelegateCommand loginCommand;
        private DelegateCommand registrateCommand;
        private HomeWindow homeWin = null;
        private string errorMsgUsername = string.Empty;
        private string errorMsgPassword = string.Empty;
        private string errorMsgUsername2 = string.Empty;
        private string errorMsgPassword2 = string.Empty;
        private Nalog account;

        public HomeVM()
        {
            this.Account = new Nalog();
        }

        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value;
                this.RaisePropertyChanged("Username");
            }
        }

        public string Username2
        {
            get
            {
                return this.username2;
            }

            set
            {
                this.username2 = value;
                this.RaisePropertyChanged("Username2");
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
                this.RaisePropertyChanged("Password");
            }
        }

        public string Password2
        {
            get
            {
                return this.password2;
            }

            set
            {
                this.password2 = value;
                this.RaisePropertyChanged("Password2");
            }
        }

        public Nalog Account
        {
            get
            {
                return account;
            }

            set
            {
                account = value;
            }
        }

        public HomeWindow HomeWin
        {
            get
            {
                return homeWin;
            }

            set
            {
                homeWin = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand RegistrateCommand
        {
            get
            {
                if (this.registrateCommand == null)
                {
                    this.registrateCommand = new DelegateCommand(this.RegistrateCommandAction, this.CanRegistrate);
                }

                return this.registrateCommand;
            }
        }

        public DelegateCommand LoginCommand
        {
            get
            {
                if (this.loginCommand == null)
                {
                    this.loginCommand = new DelegateCommand(this.LoginCommandAction, this.CanLogin);
                }

                return this.loginCommand;
            }
        }

        private bool CanLogin()
        {
            if (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(this.Password))
            {
                return false;
            }

            if (this.Password.Length < 5)
            {
                return false;
            }

            return true;
        }

        private void LoginCommandAction()
        {
            if (this.HomeWin != null && this.LogIn(this.Username, this.Password))
            {
                this.HomeWin.Close();
            }
            else
            {
                this.Account = null;
            }
        }

        private bool LogIn(string username, string password)
        {
            try
            {
                using (var db = new EuroleagueEntities3())
                {
                    this.Account = db.Nalogs.Where(x => x.IME_NLG == username).FirstOrDefault();

                    if (Account != null)
                    {
                        if (!password.Equals(Account.SFR_NLG))
                        {
                            this.errorMsgUsername = string.Empty;
                            this.errorMsgPassword = "Incorect password.";
                            this.Password = this.Password;
                            this.Username = this.Username;
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        this.errorMsgPassword = string.Empty;
                        this.errorMsgUsername = "User with username <" + username + "> doesn't exists.";
                        this.Username = this.Username;
                        this.Password = this.Password;
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CanRegistrate()
        {
            if (string.IsNullOrWhiteSpace(this.Username2) || string.IsNullOrWhiteSpace(this.Password2))
            {
                return false;
            }

            if (this.Password2.Length < 5)
            {
                return false;
            }

            return true;
        }

        private void RegistrateCommandAction()
        {
            if (this.HomeWin != null && this.Registration(this.Username2, this.Password2))
            {
                this.HomeWin.Close();
            }
            else
            {
                this.Account = null;
            }
        }

        public bool Registration(string username, string password)
        {
            try
            {
                using (var db = new EuroleagueEntities3())
                {
                    this.Account = db.Nalogs.Where(x => x.IME_NLG == username).FirstOrDefault();

                    if (Account == null)
                    {
                        Account = new Nalog() { IME_NLG = username, SFR_NLG = password, ULOGA_NLG = Roles.User.ToString() };
                        db.Nalogs.Add(Account);
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        this.errorMsgPassword2 = string.Empty;
                        this.errorMsgUsername2 = "User with username <" + username + "> already exists.";
                        this.Username2 = this.Username2;
                        this.Password2 = this.Password2;
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region IDataErrorInfo


        private ICommand lostFocusCommand;

        public ICommand LostFocusCommand
        {
            get { return lostFocusCommand ?? (lostFocusCommand = new DelegateCommand<string>(ChangeFocusName)); }
        }

        private void ChangeFocusName(string element)
        {
            if (element.Equals("UsernameLogin"))
            {
                this.isUsernameLoginFocus = true;
                RaisePropertyChanged(this.Username);
            }
            else if (element.Equals("PasswordLogin"))
            {
                this.isPasswordLoginFocus = true;
                RaisePropertyChanged(this.Password);
            }
            else if (element.Equals("UsernameRegister"))
            {
                this.isUsernameRegisterFocus = true;
                RaisePropertyChanged(this.Username2);
            }
            else if (element.Equals("PasswordRegister"))
            {
                this.isPasswordRegisterFocus = true;
                RaisePropertyChanged(this.Password2);
            }
        }

        private bool isUsernameLoginFocus = false;
        private bool isPasswordLoginFocus = false;
        private bool isUsernameRegisterFocus = false;
        private bool isPasswordRegisterFocus = false;

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
        }

        public string this[string columnName]
        {
            get
            {
                error = string.Empty;
                if (columnName.Equals("Username") && this.isUsernameLoginFocus)
                {
                    if (string.IsNullOrWhiteSpace(this.Username))
                    {
                        error = "Username is required!";
                    }
                    if (!string.IsNullOrEmpty(this.errorMsgUsername))
                    {
                        error = this.errorMsgUsername;
                        this.errorMsgUsername = string.Empty;
                    }
                }
                else if (columnName.Equals("Password") && this.isPasswordLoginFocus)
                {
                    if (string.IsNullOrWhiteSpace(this.Password))
                    {
                        error = "Password is required!";
                    }
                    else if (this.Password.Length < 5)
                    {
                        error = "Password must have at least 5 characters!";
                        this.errorMsgPassword = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(this.errorMsgPassword))
                    {
                        error = this.errorMsgPassword;
                    }
                }
                else if (columnName.Equals("Username2") && this.isUsernameRegisterFocus)
                {
                    if (string.IsNullOrWhiteSpace(this.Username2))
                    {
                        error = "Username is required!";
                    }
                    if (!string.IsNullOrEmpty(this.errorMsgUsername2))
                    {
                        error = this.errorMsgUsername2;
                        this.errorMsgUsername2 = string.Empty;
                    }
                }
                else if (columnName.Equals("Password2") && this.isPasswordRegisterFocus)
                {
                    if (string.IsNullOrWhiteSpace(this.Password2))
                    {
                        error = "Password is required!";
                    }
                    else if (this.Password.Length < 5)
                    {
                        error = "Password must have at least 5 characters!";
                        this.errorMsgPassword2 = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(this.errorMsgPassword2))
                    {
                        error = this.errorMsgPassword2;
                    }
                }
                
                this.LoginCommand.RaiseCanExecuteChanged();
                this.RegistrateCommand.RaiseCanExecuteChanged();

                return error;
            }
        }
        #endregion

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
