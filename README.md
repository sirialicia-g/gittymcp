## GittyMcp

GittyMcp (or Gittan) is my own testing ground for creating a custom MCP server which interacts with GitHub. There are plenty of premade, more elaborate MCP servers out there that exactly does the same thing (and more), but why not try out and do one yourself?

## Features

- Create and monitor issues for repo of choice without leaving the IDE

## Setup

1. Create a PAT (personal access token) for your GitHub accout.

2. Set environment variable:

   ```powershell
   [Environment]::SetEnvironmentVariable("GITHUB__PAT", "your-token", "User")
   ```

3. If that doesnt do the trick, you could also create an `appsettings.Development.json`, something in this fashion:
   ```json
   {
     "GitHub": {
       "Pat": "ghp_your_pat_here",
       "BaseUrl": "https://api.github.com/"
     },
     "Logging": {
       "LogLevel": {
         "GittyMcp": "Debug"
       }
     }
   }
   ```

```
4. Have fun!
```
