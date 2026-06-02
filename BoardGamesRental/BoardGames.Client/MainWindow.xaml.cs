using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using BoardGames.Client.Models;
using Newtonsoft.Json;

namespace BoardGames.Client
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            LoadGamesAsync();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadGamesAsync();
        }

        private async Task LoadGamesAsync()
        {
            try
            {
                // Запрашиваем данные у нашего бэкенд-сервера
                string jsonResponse = await _httpClient.GetStringAsync("http://localhost:5000/api/games");

                // Расшифровываем JSON текст в C# объекты
                var games = JsonConvert.DeserializeObject<List<BoardGame>>(jsonResponse);

                // Закидываем список игр в таблицу на экране
                DgGames.ItemsSource = games;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
