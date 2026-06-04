using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace BoardGames.Client
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var loginData = new { Login = login, Password = password };
                string json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://localhost:5000/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    // Если авторизация успешна, открываем главное окно
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close(); // Закрываем окно логина
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
