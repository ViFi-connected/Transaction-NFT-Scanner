# TransactionNftScanner

TransactionNftScanner is a .NET 9 application that scans transactions and downloads associated NFT images.

## Prerequisites

- .NET 9 SDK

## Configuration

### appsettings.json

Ensure you have an `appsettings.json` file in the root of your project with the following structure:

`{ "HttpClientSettings": { "Blackfrost": { "BaseAddress": "https://cardano-mainnet.blockfrost.io/api/v0/", "Timeout": 30 }, "Ipfs": { "BaseAddress": "https://ipfs.io/", "Timeout": 30 } } }`

### Environment Variables

Set the `PROJECT_ID` environment variable to your Blockfrost project ID.

#### Windows Command Prompt

`set PROJECT_ID=your_project_id`

#### Windows PowerShell

`$env:PROJECT_ID="your_project_id"`

#### macOS/Linux Terminal

`export PROJECT_ID=your_project_id`

## Running the Application

1. Open the solution in Visual Studio or your preferred IDE.
2. Ensure the environment variable `PROJECT_ID` is set.
3. Run the application.

Alternatively, you can run the application from the command line:

`dotnet run --project TransactionNftScanner`

## Project Structure

- **TransactionNftScanner**: Main application project.
- **TransactionNftScanner.Tests**: Unit tests for the application.

## Key Files

- `Program.cs`: Entry point of the application.
- `HttpClientHelper.cs`: Helper class for making HTTP requests.
- `App.cs`: Main application logic.

## Dependencies

- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Http
- Polly.Extensions.Http
- System.Drawing.Common

## License

This project is licensed under the MIT License.