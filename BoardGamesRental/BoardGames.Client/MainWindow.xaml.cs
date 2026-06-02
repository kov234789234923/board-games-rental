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
            _ = LoadGamesAsync();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadGamesAsync();
        }

        
        private async void BtnRent_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = DgGames.SelectedItem as BoardGame;
            if (selectedGame == null)
            {
                MessageBox.Show("Пожалуйста, выберите настольную игру из таблицы!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var response = await _httpClient.PostAsync($"http://localhost:5000/api/games/rent/{selectedGame.Id}", null);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic? result = JsonConvert.DeserializeObject(content);
                    MessageBox.Show(result?.message?.ToString() ?? "Успешно оформлено!", "Прокат", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка сервера: {content}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сети: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private async void BtnSale_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = DgGames.SelectedItem as BoardGame;
            if (selectedGame == null)
            {
                MessageBox.Show("Пожалуйста, выберите настольную игру из таблицы!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var response = await _httpClient.PostAsync($"http://localhost:5000/api/games/sell/{selectedGame.Id}", null);
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic? result = JsonConvert.DeserializeObject(content);
                    MessageBox.Show(result?.message?.ToString() ?? "Успешно продано!", "Продажа", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка сервера: {content}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сети: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadGamesAsync()
        {
            try
            {
                string jsonResponse = await _httpClient.GetStringAsync("http://localhost:5000/api/games");
                var games = JsonConvert.DeserializeObject<List<BoardGame>>(jsonResponse);
                DgGames.ItemsSource = games;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
