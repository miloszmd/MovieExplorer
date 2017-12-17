﻿using System.Collections.Generic;
using System.Net.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MovieExplorer.Data.Film;
using MovieExplorer.Helpers;
using MovieExplorer.Services.Film;
using MovieExplorer.ViewModels;

namespace MovieExplorer
{
    public sealed partial class HomePage
    {
        public FilmModel FilmModel { get; set; }

        private readonly FilmService _filmService;

        public HomePage()
        {
            InitializeComponent();
            _filmService = new FilmService(new FilmRepository(new HttpClient()));

            FilmModel = new FilmModel();
        }

        private void FilmLayout_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var filmModel = (FilmModel)e.ClickedItem;
            ParentFrameHelper.Navigate(this, typeof(FilmDetails), filmModel.Identifier);
        }

        private async void Submit_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleProgressRing();
            var findFilmByTitleResponse = await _filmService.FindByTitle(Query.Text);

            if (findFilmByTitleResponse.HasError)
            {
                ToggleProgressRing();
                Feedback.Text = findFilmByTitleResponse.Error.UserMessage;
                Feedback.Visibility = Visibility.Visible;
                FilmResults.ItemsSource = new List<FilmModel>();
                return;
            }

            Feedback.Visibility = Visibility.Collapsed;
            FilmResults.ItemsSource = findFilmByTitleResponse.Films;

            ToggleProgressRing();
        }

        private void ToggleProgressRing()
        {
            ProgressRing.IsActive = !ProgressRing.IsActive;
            ProgressRing.Visibility = ProgressRing.IsActive ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}