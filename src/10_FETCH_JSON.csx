/*
    Q-A:
        What does this script do?
            It downloads atlassian product jsons to "atlassian/json" directory with current/archived/eap subdirectories
        What software does this script depend on?
            * .net, version 5+
            * dotnet-script, any relevant version
            * anglesharp 1.0
        How to run this script?
            dotnet script <script_name>
        What was the last time this script was tested and confiremed to work?
            2021-11-21
        Who can help with this script?
            @kasthack. Available on github / telegram / twitter or anything else. Google me.
*/

#r "nuget: AngleSharp, 1.0.0-alpha-71"
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using AngleSharp.Html.Parser;


var client = new HttpClient();
var outputPath = "atlassian/json";

await DownloadSimpleUrls();
await DownloadSourceTree();

async Task DownloadSourceTree(){

    var meths = new (string release, Func<Task<string>> method)[] {
        (release: "current", method: DownloadSourceTreeCurrent),
        (release: "archived", method: DownloadSourceTreeArchive),
    };

    foreach(var (release, method) in meths)
    {
        var dir = Path.Combine(outputPath, release);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        await File.WriteAllTextAsync(
            Path.Combine(dir, "sourcetree.json"),
            await method()
        );
        Console.WriteLine($"Downloaded Sourcetree {release} json");
    }
}

async Task<string> DownloadSourceTreeArchive() =>
    "downloads(" + JsonSerializer.Serialize(
            new HtmlParser()
                .ParseDocument(
                    await client
                        .GetStringAsync("https://www.sourcetreeapp.com/download-archives")
                        .ConfigureAwait(false))
            .QuerySelectorAll(".wpl tr div>a")
            .Select(row => new { Version = row.TextContent, ZipUrl = row.GetAttribute("href") })
            .ToArray()) + ")";

async Task<string> DownloadSourceTreeCurrent(){
    var source = await client
        .GetStringAsync("https://www.sourcetreeapp.com")
        .ConfigureAwait(false);

    var (startText, endText) = ("{ \"type\":\"imkt.components.SourcetreeDownload\"", "</scri");
    var startIndex = source.IndexOf(startText, System.StringComparison.Ordinal);
    var endIndex = source.IndexOf(endText, startIndex, System.StringComparison.Ordinal);
    source = source[startIndex..endIndex];

    var json = JsonSerializer.Deserialize<SourceTreeWrap>(source, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    var (a, b, c) = json.Params;
    var urls = new[] { a, b, c };

    return
        "downloads(" +
        JsonSerializer.Serialize(
        urls
            .Select(a => new Uri(a))
            .Select(a => new
            {
                ZipUrl = a.ToString(),
                Version = new[] { "sourcetree", "enterprise", "setup", }
                    .Aggregate(Path.GetFileNameWithoutExtension(a.AbsolutePath), (s, ptr) => s.Replace(ptr, "", StringComparison.OrdinalIgnoreCase))
                    .TrimStart('-', '_')
            })
            .ToArray())+ ")";
}

async Task DownloadSimpleUrls(){
    var urls = new [] {
        "https://my.atlassian.com/download/feeds/archived/bamboo.json",
        "https://my.atlassian.com/download/feeds/archived/confluence.json",
        "https://my.atlassian.com/download/feeds/archived/crowd.json",
        "https://my.atlassian.com/download/feeds/archived/crucible.json",
        "https://my.atlassian.com/download/feeds/archived/fisheye.json",
        "https://my.atlassian.com/download/feeds/archived/jira-core.json",
        "https://my.atlassian.com/download/feeds/archived/jira-servicedesk.json",
        "https://my.atlassian.com/download/feeds/archived/jira-software.json",
        "https://my.atlassian.com/download/feeds/archived/jira.json",
        "https://my.atlassian.com/download/feeds/archived/stash.json",
        "https://my.atlassian.com/download/feeds/current/bamboo.json",
        "https://my.atlassian.com/download/feeds/current/confluence.json",
        "https://my.atlassian.com/download/feeds/current/crowd.json",
        "https://my.atlassian.com/download/feeds/current/crucible.json",
        "https://my.atlassian.com/download/feeds/current/fisheye.json",
        "https://my.atlassian.com/download/feeds/current/jira-core.json",
        "https://my.atlassian.com/download/feeds/current/jira-servicedesk.json",
        "https://my.atlassian.com/download/feeds/current/jira-software.json",
        "https://my.atlassian.com/download/feeds/current/stash.json",
        "https://my.atlassian.com/download/feeds/eap/bamboo.json",
        "https://my.atlassian.com/download/feeds/eap/confluence.json",
        "https://my.atlassian.com/download/feeds/eap/jira-servicedesk.json",
        "https://my.atlassian.com/download/feeds/eap/jira.json",
        "https://my.atlassian.com/download/feeds/eap/stash.json",
    };
    foreach(var url in urls)
    {
        var outputPathSegments = url.Split('/').TakeLast(2).ToArray();
        var dir = Path.Combine(outputPath, outputPathSegments[0]);
        if (!Directory.Exists(dir)){
            Directory.CreateDirectory(dir);
        }
        try{
            await File.WriteAllTextAsync(
                Path.Combine(new[]{outputPath}.Concat(outputPathSegments).ToArray()),
                await client.GetStringAsync(url)
            );
            Console.WriteLine($"Downloaded {url}");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Failed to download {url}: {ex.Message}");
        }
    }
}

record SourceTreeWrap(SourceTreeParams Params);
record SourceTreeParams(string MacUrl, string WindowsUrl, string EnterpriseUrl);