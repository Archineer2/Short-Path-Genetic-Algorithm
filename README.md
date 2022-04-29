# Short-Path-Genetic-Algorithm
A Windows Forms App which uses genetic algorithms to find a short path between 2D points

## Description
This program find a short (not guaranteed to be the shortest) path connecting 10 points across three sets of coordinates.
It utilizes a genetic algorithm method.
- Population Size:   10
- Generations:       2500
- Mutation Rate:     25%
- Crossover Type:    Ordered without wrap-around
- Mutation Type:     Swap two locations
- Fitness Function:  [ [ SUM (Dist^2) ] / (Dist^2) ] / [ SUM [ [ SUM (Dist^2) ] / (Dist^2) ] ]

The C# code is in Form1.cs
