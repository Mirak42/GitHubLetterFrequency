using System.Text.Json;
using System.Text;
using System;

const string GitHubApiBaseUrl = "https://api.github.com";
const string GitHubRepositoryOwner = "lodash";
const string GitHubRepositoryName = "lodash";

Console.WriteLine("Loading GitHub stats - lodash/lodash repository...");

//Load the GitHub personal access token from environment variables.
var personalAccessToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

// Create an HttpClient and set the necessary headers.
using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHubLetterFrequency");

// Include the access token only if provided.
if (!string.IsNullOrWhiteSpace(personalAccessToken))
{
    httpClient.DefaultRequestHeaders.Add("Authorization", $"token {personalAccessToken}");
}

// Get all files in the repository recursively.
var files = await GetGitHubRepositoryFiles(httpClient);

// Filter only JavaScript and TypeScript files.
var jsTsFiles = files.Where(file => file.EndsWith(".js") || file.EndsWith(".ts"))
                .ToList();

var letterFrequencies = new Dictionary<char, int>();

// Get all file contents in parallel.
var fileContents = await Task.WhenAll(jsTsFiles.Select(async file =>
{
    return await GetFileContent(httpClient, file);
}));

// Count letter occurences in each file content.
foreach (var content in fileContents)
{
    CountLetterOccurrences(content, letterFrequencies);
}

// Output the letter frequency stats.
PrintLetterFrequencyStatistics(letterFrequencies);


// Get all files in the repository.
async Task<List<string>> GetGitHubRepositoryFiles(HttpClient httpClient)
{
    try
    {
        List<string> files = new();
        var getRepoTreeUrl = $"{GitHubApiBaseUrl}/repos/{GitHubRepositoryOwner}/{GitHubRepositoryName}/git/trees/main?recursive=1";

        var response = await httpClient.GetStringAsync(getRepoTreeUrl);
        using var jsonDocument = JsonDocument.Parse(response);
        JsonElement tree = jsonDocument.RootElement.GetProperty("tree");

        foreach (var node in tree.EnumerateArray())
        {
            // Get only files, ignoring other nodes (ex: folders).
            if (node.GetProperty("type").GetString() == "blob")
            {
                files.Add(node.GetProperty("path").GetString()!);
            }
        }

        return files;
    }
    catch (Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Something wrong happened: {exception.Message}");
        Console.ResetColor();
        Environment.Exit(1);
        return null;
    }
}

// Get the content of a specific file using its path.
async Task<string> GetFileContent(HttpClient httpClient, string filePath)
{
    try
    {
        var url = $"{GitHubApiBaseUrl}/repos/{GitHubRepositoryOwner}/{GitHubRepositoryName}/contents/{filePath}";
        var response = await httpClient.GetStringAsync(url);

        using var jsonDoc = JsonDocument.Parse(response);
        var content = jsonDoc.RootElement.GetProperty("content").GetString();
        return DecodeBase64String(content!);
    }
    catch (Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Something wrong happened: {exception.Message}");
        Console.ResetColor();
        Environment.Exit(1);
        return null;
    }
}

// Decode Base64 content.
string DecodeBase64String(string base64)
{
    byte[] bytes = Convert.FromBase64String(base64);
    return Encoding.UTF8.GetString(bytes);
}

// Count the frequency of each letter in a given file content.
void CountLetterOccurrences(string content, Dictionary<char, int> letterFrequency)
{
    foreach (char ch in content.Where(ch => char.IsLetter(ch)))
    {
        var lowerCh = char.ToLower(ch);
        if (!letterFrequency.ContainsKey(lowerCh))
        {
            letterFrequency[lowerCh] = 0;
        }
        letterFrequency[lowerCh]++;
    }
}

// Print the letter frequency statis.
void PrintLetterFrequencyStatistics(Dictionary<char, int> letterFrequency)
{
    foreach (var entry in letterFrequency.OrderByDescending(kvp => kvp.Value))
    {
        Console.WriteLine($"{entry.Key}: {entry.Value}");
    }
}
