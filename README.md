# Alligator.Solver
The abstract core library of artificial intelligence for different two-player zero-sum games.

[![Build status](https://ci.appveyor.com/api/projects/status/vm0rtqw2nk1pcv0p/branch/master?svg=true)](https://ci.appveyor.com/project/boraaros/alligator-solver)
[![NuGet](https://img.shields.io/nuget/v/Alligator.Solver.svg)](https://www.nuget.org/packages/Alligator.Solver)
[![NuGet](https://img.shields.io/nuget/dt/Alligator.Solver.svg)](https://github.com/boraaros/Alligator.Solver)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/boraaros/Alligator.Solver/blob/master/LICENSE)

### Algorithms and heuristics

|name|status|name|status|name|status|
|-----|:---:|-----|:---:|-----|:---:|
|negamax algorithm| :heavy_check_mark: |history heuristic| :heavy_check_mark: |futility pruning| :x: |
|alpha-beta pruning| :heavy_check_mark: |null window search| :heavy_check_mark: |tactical & counter moves| :x: |
|iterative deepening search| :heavy_check_mark: |quiescence search| :heavy_check_mark: |internal iterative deepening| :x: |
|principal variation| :heavy_check_mark: |best node search| :heavy_check_mark: |enhanced transposition cutoff| :x: |
|transposition table| :heavy_check_mark: |late move reduction| :x: |probcut| :x: |
|killer heuristic| :heavy_check_mark: |null move heuristic| :x: |parallel search tree| :x: |

### How to use the abstract solver

First you need to model your specified board game.

__Move__ - define the possible moves (for example with source or/and target cells or/and with piece type, ..)
* the move shouldn't contain information about which player moves
* you must override `GetHashCode` and `Equals` methods, because 'reference equals' isn't enough

__Position__ - implement `IPosition<TMove>` interface (typically with game board, move history, who is the next player, ..)
* Sometimes `Identifier` is only a very strong hash key, because the number of different positions is greater than 2^64 (for example chess)
* `IsQuiet` can reduce horizont effect, but initially can be true by default
* Computation of static evaluation value is difficult, but very important for optimization

__Rules__ - implement `IRules<TPosition, TMove>` with specified game logics
* `InitialPosition` method always should create new instance

Then you can use the abstract solver component.

__SolverConfiguration__ - implement `ISolverConfiguration` interface
* Set the maximum thinking time per move
* parallelization config isn't used yet

__SolverFactory__ - provides new alpha-beta solver intances
* for details see the tic-tac-toe demo in source code
