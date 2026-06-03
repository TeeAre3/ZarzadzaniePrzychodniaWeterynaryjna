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
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;
using ZarzadzaniePrzychodniaWeterynaryjna.Services;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class StatisticsViewModel : ObservableObject
    {
        private readonly StatisticsRepository _statisticsRepository;

        [ObservableProperty] private ObservableCollection<InvoiceDto> _invoiceList = [];
        [ObservableProperty] private decimal _totalRevenue = 0;

        [ObservableProperty] private ISeries[] _chartSeries = [];
        [ObservableProperty] private Axis[] _xAxis = [];
        [ObservableProperty] private Axis[] _yAxis = [];

        public StatisticsViewModel(StatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
            LoadStatistics();
            WeakReferenceMessenger.Default.Register<ConsultationEndedMessage>(this, (r, m) => LoadStatistics());
        }

        private void LoadStatistics()
        {
            var invoices = _statisticsRepository.GetInvoices();

            InvoiceList = new ObservableCollection<InvoiceDto>(invoices);
            TotalRevenue = invoices.Sum(f => f.TotalAmmount);

            GenerateChart(invoices);
        }

        private void GenerateChart(List<InvoiceDto> invoices)
        {
            var revenueByDay = invoices
                .GroupBy(f => f.Date.Date)
                .OrderBy(g => g.Key)
                .TakeLast(7)
                .ToList();

            var dates = revenueByDay.Select(g => g.Key.ToString("dd.MM")).ToArray();
            var values = revenueByDay.Select(g => (double)g.Sum(f => f.TotalAmmount)).ToArray();
            var maxRevenue = values.Length > 0 ? values.Max() : 0;

            ChartSeries =
            [
                new ColumnSeries<double>
                {
                    Name = "Przychód (zł)",
                    Values = values,
                    Fill = new SolidColorPaint(SKColors.Teal),
                    MaxBarWidth = 40,
                    DataLabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => $"{point.Model:N0} zł"
                }
            ];

            XAxis =
            [
                new Axis
                {
                    Labels = dates,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 14
                }
            ];

            YAxis =
            [
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = maxRevenue * 1.2,
                    LabelsPaint = new SolidColorPaint(SKColors.Gray),
                    TextSize = 14
                }
            ];
        }
    }
}