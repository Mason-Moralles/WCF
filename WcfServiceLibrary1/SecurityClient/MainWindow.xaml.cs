using System;
using System.Security;
using System.ServiceModel;
using System.Windows;
using SecurityClient.SecurityRef;   // это namespace Service Reference

namespace SecurityClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoRequest_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Clear();

            var username = LoginTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            var client = new SecurityServiceClient();

            // передаём логин/пароль в WCF
            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = password;

            try
            {
                // 1. Общая информация (для всех авторизованных)
                var info = client.GetPublicInfo();
                OutputTextBox.AppendText("Public: " + info + Environment.NewLine);

                // 2. Секрет только для роли Manager
                var secret = client.GetManagerSecret();
                OutputTextBox.AppendText("Manager: " + secret + Environment.NewLine);
            }
            catch (FaultException<SecurityException> ex)
            {
                OutputTextBox.AppendText("Security error: " + ex.Detail.Message + Environment.NewLine);
            }
            catch (FaultException ex)
            {
                OutputTextBox.AppendText("Fault: " + ex.Message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                OutputTextBox.AppendText("Error: " + ex + Environment.NewLine);
            }
            finally
            {
                try { client.Close(); } catch { client.Abort(); }
            }
        }
    }
}
