using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using ZarzadzaniePrzychodniaWeterynaryjna.DTOs;
using ZarzadzaniePrzychodniaWeterynaryjna.Services;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class StatystykiViewModel : ObservableObject
    {
        private readonly StatystykiService _statystykiService = new();

        [ObservableProperty] private ObservableCollection<FakturaDto> _listaFaktur = new();
        [ObservableProperty] private decimal _calkowityPrzychod = 0;

        [ObservableProperty] private ISeries[] _wykresSeria = Array.Empty<ISeries>();
        [ObservableProperty] private Axis[] _osX = Array.Empty<Axis>();
        [ObservableProperty] private Axis[] _osY = Array.Empty<Axis>();

        public StatystykiViewModel()
        {
            ZaladujStatystyki();
            WeakReferenceMessenger.Default.Register<WizytaZakonczonaMessage>(this, (r, m) => ZaladujStatystyki());
        }

        private void ZaladujStatystyki()
        {
            var faktury = _statystykiService.PobierzFaktury();

            ListaFaktur = new ObservableCollection<FakturaDto>(faktury);
            CalkowityPrzychod = faktury.Sum(f => f.Suma);

            GenerujWykres(faktury);
        }

        private void GenerujWykres(List<FakturaDto> faktury)
        {
            var przychodyPoDniach = faktury
                .GroupBy(f => f.Data.Date)
                .OrderBy(g => g.Key)
                .TakeLast(7)
                .ToList();

            var daty = przychodyPoDniach.Select(g => g.Key.ToString("dd.MM")).ToArray();
            var wartosci = przychodyPoDniach.Select(g => (double)g.Sum(f => f.Suma)).ToArray();
            var maxPrzychód = wartosci.Length > 0 ? wartosci.Max() : 0;

            WykresSeria = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Przychód (zł)",
                    Values = wartosci,
                    Fill = new SolidColorPaint(SKColors.Teal),
                    MaxBarWidth = 40,
                    DataLabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => $"{point.Model:N0} zł"
                }
            };

            OsX = new Axis[]
            {
                new Axis
                {
                    Labels = daty,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 14
                }
            };

            OsY = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = maxPrzychód * 1.2,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 14
                }
            };
        }
    }
}