using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using KRPC.Client.Services.SpaceCenter;
using ScottPlot.Plottable;

namespace KspAutopilot;

public partial class MainWindow : Window {
    private DataLogger logger;
    
    public MainWindow() {
        InitializeComponent();
        
        this.logger = AvaPlot1.Plot.AddDataLogger();

        var task = Task.Run((async () => await Work()));
    }

    private async Task Work() {
        using var connection = new KRPC.Client.Connection();
        var sc = connection.SpaceCenter();
        var vessel = sc.ActiveVessel;
        var flight = vessel.Flight();
        var utStream = connection.AddStream((() => sc.UT));
        var altitudeStream = connection.AddStream((() => flight.SurfaceAltitude));
        
        while (true) {
            var ut = utStream.Get();
            var altitude = altitudeStream.Get();
            
            this.logger.Add(ut, altitude); 
            this.AvaPlot1.Plot.AxisAuto();
            Dispatcher.UIThread.Invoke((() => this.AvaPlot1.Refresh()));

            await Task.Delay(100);
        }
    }
}