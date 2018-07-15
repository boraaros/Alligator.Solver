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
|principal variation| :heavy_check_mark: |mtd-f search| :heavy_check_mark: |enhanced transposition cutoff| :x: |
|transposition table| :heavy_check_mark: |late move reduction| :x: |probcut| :x: |
|killer heuristic| :heavy_check_mark: |null move heuristic| :x: |parallel search tree| :x: |
