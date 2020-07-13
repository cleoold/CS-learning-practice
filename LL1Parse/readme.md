This is a small program that reads in an LL(1) grammar, outputs its Nullable, First, Follow sets and parse table. The input is provided via stdin. Each line represents
a production rule of the form `A -> b` where `b` may be a sequence, delimited by space, of symbols. Additionally, the character `|` is special if it appears on the RHS
of a line: it will serve as the union operator so the rule in which this character appears will be split into corresponding components. After the rules test cases can be
inserted.

example input file:
```
S' -> BoF S EoF
S  -> x | y | z | ( S op S )
op -> + | - | * | /

BoF ( x - ( y * z ) ) EoF
```

output:
```powershell
> Get-Content .\input | .\LL1Parse.exe | Set-Content .\output -Encoding utf8
Non-terminals: {op, S, S'}
Terminals    : {-, (, ), *, /, +, BoF, EoF, x, y, z}
Start        : S'

Productions:
0. S' -> BoF S EoF
1. S -> x
2. S -> y
3. S -> z
4. S -> ( S op S )
5. op -> +
6. op -> -
7. op -> *
8. op -> /

Nullable:
S'   false
S    false
op   false

First:
op   {-, *, /, +}
S    {(, x, y, z}
S'   {BoF}

Follow:
op   {(, x, y, z}
S    {-, ), *, /, +, EoF}
S'   {}

Parse table:
     -   (   )   *   /   +   BoF   EoF   x   y   z   
op   6           7   8   5
S        4                               1   2   3  
S'                           0
Grammar is LL1 parsable.

Derivation of "BoF ( x - ( y * z ) ) EoF":
BoF S EoF                   (0. S' -> BoF S EoF)
BoF ( S op S ) EoF          (4. S -> ( S op S ))
BoF ( x op S ) EoF          (1. S -> x)
BoF ( x - S ) EoF           (6. op -> -)
BoF ( x - ( S op S ) ) EoF  (4. S -> ( S op S ))
BoF ( x - ( y op S ) ) EoF  (2. S -> y)
BoF ( x - ( y * S ) ) EoF   (7. op -> *)
BoF ( x - ( y * z ) ) EoF   (3. S -> z)
```
