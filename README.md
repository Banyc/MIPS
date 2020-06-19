# MIPS

## Introduction

![](img/2020-06-19-21-34-09.png)

## Run

1. Set `pwd` to `src/MIPS.Organizer/`
1. Run command `dotnet run`
1. Follow the instructions

## Environment

You will need .NET Core SDK to build and run this code.

Download link: <https://dotnet.microsoft.com/download/dotnet-core>.

## Currently Supported Instructions

Go to `README.md` under directory `MIPS.Simulator`.

## Update SLN

```
dotnet sln add src/MIPS.Shared/MIPS.Shared.csproj
dotnet sln add src/MIPS.Interpreter/MIPS.Interpreter.csproj
dotnet sln add src/MIPS.Simulator/MIPS.Simulator.csproj
dotnet sln add src/MIPS.Organizer/MIPS.Organizer.csproj
```

## Build a shared library

- <https://stackoverflow.com/a/48551334/9920172>
