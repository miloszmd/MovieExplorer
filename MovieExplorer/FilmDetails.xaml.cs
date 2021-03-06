﻿using System;
using System.Net.Http;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MovieExplorer.Data.Film;
using MovieExplorer.Data.Trailer;
using MovieExplorer.Services.Film;
using MovieExplorer.Services.Trailer;

namespace MovieExplorer
{
    public sealed partial class FilmDetails
    {
        private readonly FilmService _filmService;
        private readonly TrailerService _trailerService;

        public FilmDetails()
        {
            InitializeComponent();

            _filmService = new FilmService(new FilmRepository(new HttpClient()));
            _trailerService = new TrailerService(new TrailerRepository(new HttpClient()));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadFilmDetails((string) e.Parameter);
        }

        private async void LoadFilmDetails(string identifier)
        {
            var filmDetailsResponse = await _filmService.FindDetails(identifier);
            var filmDetails = filmDetailsResponse.FilmDetails;

            PosterBackground.Source = new BitmapImage(new Uri(filmDetails.Poster));
            Poster.Source = new BitmapImage(new Uri(filmDetails.Poster));
            Title.Text = filmDetails.Title;
            Rating.Text = filmDetails.Ratings;
            Genre.Text = filmDetails.Genre;
            Released.Text = filmDetails.Released;
            Plot.Text = filmDetails.Plot;

            ToggleProgressRing();

            var filmTrailer = await _trailerService.FindTrailerFor(filmDetails.Title, filmDetails.Year);

            if (filmTrailer.Trailer != null)
            {
                Trailer.Source = new Uri(filmTrailer.Trailer);
                Trailer.Visibility = Visibility;
            }
            else
            {
                NoTrailer.Visibility = Visibility.Visible;
            }

            ToggleProgressRing();
        }

        private void ToggleProgressRing()
        {
            TrailerSearchRing.IsActive = !TrailerSearchRing.IsActive;
            TrailerSearchRing.Visibility = TrailerSearchRing.IsActive ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var speechSynthesis = new SpeechSynthesizer();
            var speechStrem = await speechSynthesis.SynthesizeTextToStreamAsync(Plot.Text);

            Speech.SetSource(speechStrem, speechStrem.ContentType);
            Speech.Play();
        }
    }
}