using System;
using System.Collections.Generic;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CTime3.Apps.WPF.Models;
using Wpf.Ui.Common.Interfaces;

namespace CTime3.Apps.WPF.ViewModels
{
    public partial class DataViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized;

        [ObservableProperty]
        private IEnumerable<DataColor>? _colors;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        private void InitializeViewModel()
        {
            var random = new Random();
            var colorCollection = new List<DataColor>();

            for (var i = 0; i < 8192; i++)
#pragma warning disable CA5394 // Do not use insecure randomness
                colorCollection.Add(new DataColor
                {
                    Color = new SolidColorBrush(Color.FromArgb(
                        (byte)200,
                        (byte)random.Next(0, 250),
                        (byte)random.Next(0, 250),
                        (byte)random.Next(0, 250)))
                });
#pragma warning restore CA5394 // Do not use insecure randomness

            Colors = colorCollection;

            _isInitialized = true;
        }
    }
}
