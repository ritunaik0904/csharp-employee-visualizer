using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

class log
{
    public string employee { get; set; }
    public double hoursWorked { get; set; }
}

class Program
{
    static async Task Main()
    {
        var url = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
        var http = new HttpClient();
        var raw = await http.GetStringAsync(url);
        var stuff = JsonSerializer.Deserialize<List<log>>(raw);
        var grouped = stuff.GroupBy(x => x.employee)
                           .Select(g => new { name = g.Key, total = g.Sum(x => x.hoursWorked) })
                           .OrderByDescending(x => x.total)
                           .ToList();
        makeHtml(grouped);
    }

    static void makeHtml(List<dynamic> people)
    {
        string html = @"<html><head><style>body { font-family: sans-serif; background: #f0f0f0; padding: 20px; }
table { border-collapse: collapse; width: 70%; } th, td { border: 1px solid #aaa; padding: 8px; }
th { background: #333; color: #fff; } .low { background: #ffd6d6; }</style></head><body><h2>Work Stats</h2><table><tr><th>Name</th><th>Hrs</th></tr>";
        foreach (var p in people)
        {
            var row = p.total < 100 ? " class='low'" : "";
            html += $"<tr{row}><td>{p.name}</td><td>{p.total:F1}</td></tr>";
        }
        html += "</table></body></html>";
        File.WriteAllText("worktable.html", html);
    }
}