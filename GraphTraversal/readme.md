This is the program to find paths in a directed weighted graph given a file containing edges

#### Example
```
# file contains:
Snowdon Parc 6
Snowdon Lionel-Groulx 4
Parc Jean-Talon 2
Lionel-Groulx Jean-Talon 7
```

```powershell
# open the program
> GraphTraversal.exe file.txt
# finds connected vertices from Snowdon
INPUT: t Snowdon
# list paths
INPUT: p
# result
PATH FROM Snowdon TO Snowdon IS IMMEDIATE.
PATH FROM Snowdon TO Lionel-Groulx:
Snowdon -> Lionel-Groulx
COST 4.
PATH FROM Snowdon TO Parc:
Snowdon -> Parc
COST 6.
PATH FROM Snowdon TO Jean-Talon:
Snowdon -> Parc -> Jean-Talon
COST 8.
```

