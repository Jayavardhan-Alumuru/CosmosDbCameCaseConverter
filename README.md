
# CosmosDB Converter

A utility tool designed to convert property names in Azure Cosmos DB documents to camelCase format. This tool processes all documents in a specified container, converting property names at all levels (including nested objects and arrays) to camelCase while preserving the data values.

## Features

- Automatically converts all property names to camelCase format
- Handles nested objects and arrays recursively
- Preserves original data values
- Processes entire collections in bulk
- Maintains document IDs during conversion
- Provides progress feedback during processing

## Prerequisites

- .NET 6.0 or later
- Azure Cosmos DB account
- Azure Cosmos DB connection string with write permissions

## Configuration

Before running the application, you need to configure the following settings in the Program.cs file:
- `ConnectionString`: Your Azure Cosmos DB connection string
- `DatabaseId`: The name of your database
- `ContainerId`: The name of your container

## Getting Started

1. Clone this repository
2. Update the configuration settings in Program.cs with your Cosmos DB details
3. Build the project:
   ```
   dotnet build
   ```
4. Run the application:
   ```
   dotnet run
   ```

## How It Works

The tool:
1. Connects to your Cosmos DB container
2. Retrieves all documents
3. For each document:
   - Converts all property names to camelCase format
   - Processes nested objects and arrays recursively
   - Maintains the original data values
   - Updates the document in the container
4. Provides progress feedback in the console

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
