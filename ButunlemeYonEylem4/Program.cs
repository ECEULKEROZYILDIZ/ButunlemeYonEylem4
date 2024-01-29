using System;
using System.Collections.Generic;
using Google.OrTools.ConstraintSolver;

public class TspCities
{
   class DataModel
    { 
        public int[,] DistanceMatrix = new int[5, 5]
          {
                { 0, 13, 22, 16, 6 },
                { 13, 0, 29, 20, 8 },
                { 22, 29, 0, 11, 30 },
                { 16, 20, 11, 0, 20 },
                { 6, 8, 30, 20, 0 }
          };

        public int VehicleNumber = 1;
        public int Depot = 1;
    };

    static void PrintSolution(in RoutingModel routing, in RoutingIndexManager manager, in Assignment solution)
    {
        Console.WriteLine("Objective: {0} miles", solution.ObjectiveValue());

        Console.WriteLine("Route:");
        long routeDistance = 0;
        var index = routing.Start(0);
        while (routing.IsEnd(index) == false)
        {
            Console.Write("{0} -> ", manager.IndexToNode((int)index));
            var previousIndex = index;
            index = solution.Value(routing.NextVar(index));
            routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
        }
        Console.WriteLine("{0}", manager.IndexToNode((int)index));
        Console.WriteLine("Route distance: {0}miles", routeDistance);
    }

    public static void Main(String[] args)
    {
        DataModel data = new DataModel(); 
        RoutingIndexManager manager =
            new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);
     
        RoutingModel routing = new RoutingModel(manager);

        int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
        {
            var fromNode = manager.IndexToNode(fromIndex);
            var toNode = manager.IndexToNode(toIndex);
            return data.DistanceMatrix[fromNode, toNode];
        });

        routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);
        RoutingSearchParameters searchParameters =
            operations_research_constraint_solver.DefaultRoutingSearchParameters();
        searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc; 
        Assignment solution = routing.SolveWithParameters(searchParameters);       
        PrintSolution(routing, manager, solution);    
    }
}
