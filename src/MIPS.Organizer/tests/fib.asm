main:   addi   $sp, $zero, 2048
        lw     $s0, 1024($zero)
        add    $a0, $s0, $zero
        jal    fib
        halt
fib:    addi   $sp, $sp, -12  # prologue
        sw     $ra, 8($sp)
        sw     $s0, 4($sp)
        sw     $a0, 0($sp)
        bne    $a0, $zero, test2  # first statement
        xor    $v0, $v0, $v0
        j      rtn
test2:  addi   $t0, $zero, 1  # start of second block
        bne    $a0, $t0, test3
        add    $v0, $t0, $zero
        j      rtn
test3:  addi   $a0, $a0, -1  # start of third block
        jal    fib
        add    $s0, $v0, $zero
        addi   $a0, $a0, -1
        jal    fib
        add    $v0, $s0, $v0
rtn:    lw     $a0, 0($sp)  # epilogue
        lw     $s0, 4($sp)
        lw     $ra, 8($sp)
        addi   $sp, $sp, 12
        jr     $ra


# int fib(int n)
# {
#     if (n == 0)
#         return 0;
#     else if (n == 1)
#         return 1;
#     else
#         return fib(n - 1) + fib(n-2);
# }

# s0 := fib(n - 1)
# v0 := return value
# a0 := arguement

# data begin
# 1024 2
# 00000000 00000000 00000000 00010100
# data end
