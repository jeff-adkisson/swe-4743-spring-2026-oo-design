# CrazyEights Assignment 1 Solution

This solution implements a console-based version of the card game Crazy 8’s using object-oriented design principles.

This assignment is not about game sophistication. It is explicitly about demonstrating correct and intentional use of:

- Interfaces
- Abstract classes
- Concrete classes
- Polymorphism
- Dynamic dispatch
- Encapsulation
- Composition Root Pattern


## Running the Application from CLI

From the root directory, use the following commands:

```bash
cd src/CrazyEights
 (requires [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or later, and C# 10 or later)
dotnet run
```

## Building and Running with Docker

From the root directory:

```bash
cd src/CrazyEights
docker build -t crazy-eights -f Dockerfile .
docker run --rm crazy-eights
```

> Ensure Docker is installed and running on your system.

## Environment Requirements

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or later
- C# 10 or later
- [Docker](https://www.docker.com/get-started) (optional, for containerized builds)
- Compatible operating system: Windows, macOS, or Linux
- Internet connection (for dependency downloads)

## Screenshots

![image-20260119203248615](README.assets/image-20260119203248615.png)

![image-20260119202952201](README.assets/image-20260119202952201.png)
