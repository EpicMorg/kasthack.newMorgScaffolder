/*
    Q-A:
        What does this script do?
            It generates build files for atlassian ecosystem
        What software does this script depend on?
            * .net, version 5+
            * dotnet-script, any relevant version
        How to run this script?
            * run json-fetcher first
            * dotnet script <script_name>
        What was the last time this script was tested and confiremed to work?
            2021-11-21
        Who can help with this script?
            @kasthack. Available on github / telegram / twitter or anything else. Google me.
*/
using System.Linq;
using System.IO;
using System.Text.Json;

var mappings = new (string template, string json)[]{
    (
        //jira 4-6
        template: "jira",               // path to template directory
        json: "jira-software",          // json file name without extension
        archiveType: ".tar.gz"          // archive extension to download for versions
    ),
    (
        //jira 7-8
        template: "jira",
        json: "jira",
        archiveType: ".tar.gz"
    ),
    //unused, stubs
    (
        template: "bitbucket",
        json: "stash",
        archiveType: ".tar.gz"
    ),
    (
        template: "confluence",
        json: "confluence",
        archiveType: ".tar.gz"
    ),
    (
        template: "crucible",
        json: "crucible",
        archiveType: ".tar.gz"
    ),
    (
        template: "fisheye",
        json: "fisheye",
        archiveType: ".tar.gz"
    ),
};

var rootPath = "atlassian";
var jdkVersion = "11";

//code=============================================================================

var jsonPath = Path.Combine(rootPath, "json");
var templatePath = Path.Combine(rootPath, "templates");
var outputPath = Path.Combine(rootPath, "output");
var releasesToInclude = new[] { "current", "archived" };

foreach (var mapping in mappings)
{
    if (!Directory.Exists(Path.Combine(templatePath, mapping.template)))
    {
        Console.WriteLine($"{mapping.template} does not exist. Skipping");
        continue;
    }
    var allVersions = releasesToInclude
       .Select(r => Path.Combine(jsonPath, r, $"{mapping.json}.json"))
       .Where(f => File.Exists(f))
       .Select(file => File.ReadAllText(file)["downloads(".Length..^1])
       .SelectMany(jsonData => JsonSerializer.Deserialize<ResponseItem[]>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))
       .ToArray();
    var versions =
        allVersions
        .Where(a => a.ZipUrl != null && a.ZipUrl.ToString().EndsWith(mapping.archiveType) && !a.ZipUrl.ToString().Contains("-war"))
        .ToArray();
    Console.WriteLine($"Found {versions.Length} versions for {mapping.template}");
    foreach (var version in versions)
    {
        var majorVersion = version.Version.Split('.').First();
        var versionPath = Path.Combine(outputPath, mapping.template, majorVersion, version.Version);
        if (Directory.Exists(versionPath))
        {
            Console.WriteLine($"{mapping.template} {version.Version} already exists. Skipping");
            continue;
        }
        else
        {
            Directory.CreateDirectory(versionPath);
        }

        var versionTemplatePath = Path.Combine(templatePath, mapping.template, majorVersion);
        if (!Directory.Exists(versionTemplatePath))
        {
            Console.WriteLine($"Template for {mapping.template} version {majorVersion} does not exist. Skipping.");
            continue;
        }
        Console.WriteLine($"Writing to {versionPath}");
        CopyFilesRecursively(new DirectoryInfo(versionTemplatePath), new DirectoryInfo(versionPath));
        File.WriteAllText(
                    Path.Combine(versionPath, ".env"),
                    @$"
RELEASE={version.Version}
DOWNLOAD_URL={version.ZipUrl}
JDK_VERSION={jdkVersion}
PRODUCT={mapping.template}
");
    }
}

var productPath = Path.Combine(outputPath, "shared");
var sharedTemplatePath = Path.Combine(jsonPath, "shared");
if (Directory.Exists(sharedTemplatePath))
{
    var output = Directory.CreateDirectory(productPath);
    CopyFilesRecursively(
        new DirectoryInfo(sharedTemplatePath),
        output
    );
}

void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
{
    foreach (DirectoryInfo dir in source.GetDirectories())
        CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
    foreach (FileInfo file in source.GetFiles())
        file.CopyTo(Path.Combine(target.FullName, file.Name));
}

public class ResponseItem
{
    public string Description { get; set; }
    public string Edition { get; set; }
    public Uri ZipUrl { get; set; }
    public string TarUrl { get; set; }
    public string Md5 { get; set; }
    public string Size { get; set; }
    public string Released { get; set; }
    public string Type { get; set; }
    public string Platform { get; set; }
    public string Version { get; set; }
    public Uri ReleaseNotes { get; set; }
    public Uri UpgradeNotes { get; set; }
}