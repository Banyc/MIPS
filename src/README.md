# src

- `MIPS.Organizer`
    - combine functionality of virtual machine and interpreter
- `MIPS.Simulator`
    - virtual machine for MIPS

## Add reference

```
dotnet add MIPS.Interpreter/MIPS.Interpreter.csproj reference MIPS.Shared/MIPS.Shared.csproj
dotnet add MIPS.Simulator/MIPS.Simulator.csproj reference MIPS.Shared/MIPS.Shared.csproj

dotnet add MIPS.Console/MIPS.Console.csproj reference MIPS.Shared/MIPS.Shared.csproj
dotnet add MIPS.Console/MIPS.Console.csproj reference MIPS.Simulator/MIPS.Simulator.csproj
dotnet add MIPS.Console/MIPS.Console.csproj reference MIPS.Interpreter/MIPS.Interpreter.csproj
```
