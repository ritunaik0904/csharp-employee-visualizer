using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScottPlot;

class WorkLog
{
    public string employee { get; set; }
    public double hoursWorked { get; set; }
}

class Summary
{
    public string Name { get; set; }
    public double TotalHours { get; set; }
}

class Program
{
    static async Task Main()
    {
        var url = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
        var http = new HttpClient();
        var raw = await http.GetStringAsync(url);
        var logs = JsonSerializer.Deserialize<List<WorkLog>>(raw) ?? new List<WorkLog>();
        var grouped = logs.GroupBy(x => x.employee)
                          .Select(g => new Summary { Name = g.Key, TotalHours = g.Sum(x => x.hoursWorked) })
                          .OrderByDescending(x => x.TotalHours)
                          .ToList();
        var values = grouped.Select(x => x.TotalHours).ToArray();
        var labels = grouped.Select(x => x.Name).ToArray();
        var plt = new ScottPlot.Plot(600, 400);
        plt.Title("Time Distribution");
        plt.PlotPie(values, labels);
        string filename = $"chart_{System.DateTime.Now:MMdd_HHmm}.png";
        plt.SaveFig(filename);
    }
}