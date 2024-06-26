# Airbox
## Build and Run

You will require [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed in order to run and test the API

Navigate to the location you wish to clone the repo to:

```
git clone https://github.com/AttemptedCollective/Airbox.git
cd Airbox
dotnet build
```

Running the Api
```
cd Airbox.Api.Gateway
dotnet run
```

By default the Api will run on port 5055, this can be changed by editing the LaunchSettings.json

Running the API Unit Tests
```
cd Airbox.Api.Gateway.UnitTests
dotnet test
```

Running the User Storage Unit Tests
```
cd Airbox.Api.Users.Storage.UnitTests
dotnet test
```

## OpenApi
A openapi.json can be found with the static value of the Api

This has been generated through swagger. To regenerate the file, you will need to run the solution in Debug mode as Swagger is not enabled in production to reduce the footprint of the api.
After running in debug mode, navigate to the json generated by swagger.

## Tests
- Tests have been added for all endpoints within the UsersController
- Tests have been added for the Storage mechanism
- TODO: Tests required for core classes
