====================================================================
YASS - Yet Another Sokoban Solver and Optimizer - For Small Levels
Version 2.122 - March 20, 2010
Copyright (c) 2010 by Brian Damgaard, Denmark
E-mail: BrianDamgaard@jubii.dk
====================================================================

Sokoban(r) Registered Trademark of Falcon Co., Ltd., Japan
Sokoban Copyright (c) 1982-2010 by Hiroyuki Imabayashi, Japan
Sokoban Copyright (c) 1989, 1990, 2001-2010 by Falcon Co., Ltd., Japan

License
--------
YASS - Yet Another Sokoban Solver and Optimizer
Copyright (c) 2010 by Brian Damgaard, Denmark

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

Credits and acknowledgements
-----------------------------
Many people have contributed to this program, and I'm particularly
indebted to:

Lee J Haywood for providing most of the basic deadlock patterns,
for sharing a lot of ideas, and for a long-standing correspondence
on Sokoban issues.

Matthias Meger for sharing a lot of ideas and for a long-standing
correspondence on solvers, optimizers, and deadlock-detection.

Sebastien Gouezel for inventing the "Vicinity search" optimization
method, and generously sharing information on the method and its
implementation. By sharing his ideas and insights on the subject,
he has made a significant and lasting contribution to the Sokoban
game itself, transcending the implementation of the algorithm in
the YASS optimizer.

The Program
------------
The primary purpose of program is to find push-optimal solutions.
The found solutions may not be fully optimal in the sense that
there may be other solutions of the same length using fewer
non-pushing player-moves.

Additionally, the program can search for improvements of existing
solutions. Here is an example showing the parameters:

YASS LevelFileName -search optimize -optimize moves

Solving Sokoban levels is a complicated task for a computer program,
hence, the program can only handle small levels.

Usage
------
Usage: YASS <filename> [options]

Options:
  -deadlocks <number>                : deadlock-sets complexity level 0-3, default 1
  -fallback  <y|n>                   : optimizer fallback strategy, default "disabled"
  -help                              : this overview
  -level     <number> [ - <number> ] : levels to process, default "all"
  -log                               : save search information to a logfile
  -maxpushes <number> (million)      : search limit, default none
  -maxdepth  <number>                : search limit, default (and max.) 600 pushes
  -maxtime   <seconds>               : search limit, default (and max.) 49 days
  -memory    <size>|available (MiB)  : defaults to half of physical memory
  -optimize  <moves|pushes|pushesonly|boxlines/moves|boxlines/pushes> : default "pushes"
  -order     g|p|r|v| ...  : optimizer methods: Global, Permutations, Rearran., Vicinity
  -packingorder <number>             : packing order threshold, default 9 boxes  
  -pretty    <no|yes>                : do small optimizations, default "yes"   
  -prompt    <no|yes>                : confirm messages, default "yes"
  -quick     <no|yes>                : optimizer quick vicinity-search enabled
  -search    <method>                : backward/forward/optimize/perimeter (default)
  -stop      <no|yes>                : stop when a solution has been found, default "no"
  -vicinity  <number> ...            : vicinity squares per box (maximum 4 boxes)

Tip:
  Solving small goal-room themed levels with 9 or more boxes may require
  disabling packing-order search by setting the threshold to a higher value.
