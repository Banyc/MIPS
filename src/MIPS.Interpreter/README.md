# MIPS Interpreter

## build

```
dotnet restore
dotnet run
```

## Instructions

### Read-from-file Mode

1. Run the APP
1. place your MIPS code under the same directory as the APP
1. Type in `r`
1. Type in something like
    ```
    -s a.asm -o a.txt -n -h -p
    ```
1. go check the new file `a.txt`

### Interactive Mode

1. Run the APP
1. Type in `i`
1. Just follow the in-app instructions

## Customize your language

Go to the `Generator` folder

## Supported Instructions

- add
- addi
- addiu
- addu
- and
- andi
- beq
- bgtz
- bne
- j
- jal
- jr
- lb
- lbu
- lh
- lhu
- lui
- lw
- nor
- or
- ori
- sb
- sh
- sll
- slt
- sltu
- slti
- sra
- srl
- sub
- sw
- xor
- move
- bgt
- halt
