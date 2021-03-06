﻿using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MovieExplorer.Data.Quiz;
using MovieExplorer.Services.Quiz;

namespace MovieExplorer
{
    public sealed partial class QuizPage
    {
        private readonly Dictionary<int, string> _answeredQuestions;
        private readonly List<QuizItem> _quizItems;

        public QuizPage()
        {
            InitializeComponent();
            var quizService = new QuizService(new QuizRepository());
            _quizItems = quizService.GetAll();

            QuizItems.ItemsSource = _quizItems;

            _answeredQuestions = new Dictionary<int, string>();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var originalSource = (RadioButton) e.OriginalSource;
            var dataContext = (PossibleAnswer) originalSource.DataContext;

            _answeredQuestions[dataContext.Index] = dataContext.Answer;
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var countOfCorrectAnswers =
                _answeredQuestions.Count(answers => _quizItems.Any(x => answers.Value == x.CorrectAnswer));
            if (countOfCorrectAnswers == _quizItems.Count)
            {
                var element = new MediaElement();
                var folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var file = await folder.GetFileAsync("clapping.wav");
                var stream = await file.OpenAsync(FileAccessMode.Read);
                element.SetSource(stream, "");
                element.Play();

                Submit.Visibility = Visibility.Collapsed;
                Feedback.Text = "All answers correct. Well done!";
                return;
            }

            Feedback.Text = $"{countOfCorrectAnswers}/{_quizItems.Count} answers correct. Good try!";
        }
    }
}