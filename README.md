# GitHub Letter Frequency Analyzer
This is a .NET 6 console application that calculates the frequency of each letter in all JavaScript and TypeScript files of GitHub repository lodash/lodash. 
It uses the GitHub API to fetch repository content with no additional or external packages installed.

## Setup Instructions

### Clone the Repository
```
git clone https://github.com/Mirak42/GitHubLetterFrequency.git
cd GitHubLetterFrequency
```
### Create a GitHub Personal Access Token
The GitHub API has rate limits for requests, depending on the type of authentication.
Only 60 requests per hour for unauthenticated calls, this is why a personal access token is recommended.

* Go to your GitHub account settings: [GitHub Personal Access Tokens](https://github.com/settings/tokens).
* Generate a new token with read-only access to public repositories.
* Copy and keep the generated token.

### Set Up the Access Token
Set up the environment variable GITHUB_TOKEN. Some options are:
* Using launchSettings.json (for local development):
Add the token under the **environmentVariables** section:
```
{
  "profiles": {
    "GitHubLetterFrequency": {
      "commandName": "Project",
      "environmentVariables": {
        "GITHUB_TOKEN": "your_token_here"
      }
    }
  }
}
```
* Using a System Environment Variable:
  + On Windows:
  ```
  $env:GITHUB_TOKEN = "your_token_here"
  ```
  + On macOS/Linux:
  ```
  export GITHUB_TOKEN="your_token_here"
  ```
### Run the Application
```
dotnet run --project GitHubLetterFrequency.csproj
```


