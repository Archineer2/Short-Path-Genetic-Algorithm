// Shortest Path via Genetic Algorithm Program by Ryan Desacola
// 
// This program find the shortest path connecting 10 points across three sets of coordinates
// It utilizes a genetic algorithm method
// * Population Size:   10
// * Generations:       2500
// * Mutation Rate:     25%
// * Crossover Type:    Ordered without wrap-around
// * Mutation Type:     Swap two locations
// * Fitness Function:  [ [ SUM (Dist^2) ] / (Dist^2) ] / [ SUM [ [ SUM (Dist^2) ] / (Dist^2) ] ]

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS689HW01
{
    public partial class Form1 : Form
    {
    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 
    // VARIABLES AND CONSTANTS
    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 

        // Initial Data (3 Sets of Coordinates)
        int[,] set01 = new int[10, 2] { { 834, 707 }, { 843, 626 }, { 140, 733 }, { 109, 723 }, { 600, 747 },
                                        { 341, 94 }, { 657, 197 }, { 842, 123 }, { 531, 194 }, { 286, 336 } };

        int[,] set02 = new int[10, 2] { { 8, 377 }, { 450, 352 }, { 519, 290 }, { 398, 604 }, { 417,496 },
                                        { 57,607 }, { 119,4 }, { 166,663 }, { 280,622 }, { 531,571 } };

        int[,] set03 = new int[10, 2] { { 518, 995 }, { 590, 935 }, { 600, 985 }, { 151, 225 }, { 168, 657 }, 
                                        { 202, 454 }, { 310, 717 }, { 425, 802 }, { 480, 940 }, { 300, 1035 } };

        // Distance Tables (10x10 of distances between each coordinate)
        double[,] distanceTable01;
        double[,] distanceTable02;
        double[,] distanceTable03;

        // Shortest Routes (Array of 10 positions: 0 1 2 3 4 5 6 7 8 9)
        int[] shortestPath01;
        int[] shortestPath02;
        int[] shortestPath03;

        // Population Arrays
        int[,] population01;
        int[,] population02;
        int[,] population03;

        // Flags
        bool searchCompleted01 = false;
        bool searchCompleted02 = false;
        bool searchCompleted03 = false;

        // Graph Size
        int xyMaximum01 = 1200;
        int xyMaximum02 = 1200;
        int xyMaximum03 = 1200;

        // Simulated Annealing Constants
        double TOLERANCE = 0.10;
        double COOLING_RATE = 0.01;

        // Genetic Algorithm Constants
        int POPULATION_SIZE = 10;
        int GENERATIONS = 2500;
        double MUTATION = 0.25;

    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 
    // INITIALIZER FUNCTIONS
    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 

        // Initializer
        public Form1()
        {
            InitializeComponent();
            ClearImage(Color.DarkGray);

            Console.WriteLine("Started");

            xyMaximum01 = CalculateGraphMax(set01);
            xyMaximum02 = CalculateGraphMax(set02);
            xyMaximum03 = CalculateGraphMax(set03);

            GenerateDistanceTable(set01, ref distanceTable01);
            GenerateDistanceTable(set02, ref distanceTable02);
            GenerateDistanceTable(set03, ref distanceTable03);

            // DebugTest();
        }

    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 
    // PICTURE BOX DRAWING FUNCTIONS
    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 

        // Applies a single color to the entire picturebox
        private void ClearImage(Color color)
        {
            // Clear the PictureBox with color
            Bitmap bm = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height);
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.Clear(color);
                pictureBox1.Image = bm;
            }
        }

        private void DrawGrid(Color color, int graphMax)
        {
            Bitmap bm = (Bitmap)pictureBox1.Image;

            int height = bm.Height;
            int width = bm.Width;

            using (Graphics g = Graphics.FromImage(bm))
            {
                for (int i = 0; i < graphMax; i++)
                {
                    if (i % 100 == 99)
                    {
                        for (int j = 0; j < graphMax; j++)
                        {
                            int x = i * width / graphMax;
                            int y = j * height / graphMax;

                            bm.SetPixel(x, y, color);
                        }

                        for (int j = 0; j < graphMax; j++)
                        {
                            int x = j * width / graphMax;
                            int y = i * height / graphMax;

                            bm.SetPixel(x, y, color);
                        }
                    }
                }

                pictureBox1.Image = bm;
            }
        }

        // Creates 3x3 pixel boxes at each coordinate of the given input set of the given color
        private void PlotPoints(int[,] inputSet, Color color, int graphMax)
        {
            Bitmap bm = (Bitmap)pictureBox1.Image;

            int height = bm.Height;
            int width = bm.Width;

            using (Graphics g = Graphics.FromImage(bm))
            {
                for (int i = 0; i < 10; i++)
                {
                    int x = inputSet[i,0] * width / graphMax;
                    int y = inputSet[i,1] * height / graphMax;

                    x--;
                    y--;

                    for (int j = 0; j < 3; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            bm.SetPixel(x + k, height - (y + j), color);
                        }
                    }
                }

                pictureBox1.Image = bm;
            }
        }

        // Draws a WHITE line between two coordinates
        private void DrawLine(int x1, int y1, int x2, int y2, int graphMax)
        {
            Bitmap bm = (Bitmap)pictureBox1.Image;

            int height = bm.Height;
            int width = bm.Width;

            using (Graphics g = Graphics.FromImage(bm))
            {
                if (x1 > x2)
                {
                    int temp = x2;
                    x2 = x1;
                    x1 = temp;
                    temp = y2;
                    y2 = y1;
                    y1 = temp;
                }

                double slope = (double)(y2 - y1) / (double)(x2 - x1);
                double invSlope = (double)(x2 - x1) / (double)(y2 - y1);

                if (slope < 1 && slope > -1)
                {
                    for (int i = x1; i < x2; i++)
                    {
                        int x = i * width / graphMax;
                        int y = y1 + (int)((i-x1) * slope);
                        y = y * height / graphMax;

                        bm.SetPixel(x, height - (y), Color.White);
                    }
                }
                else 
                {
                    if (y1 < y2)
                    {
                        for (int i = y1; i < y2; i++)
                        {
                            int y = i * height / graphMax;
                            int x = x1 + (int)((i-y1) * invSlope);
                            x = x * width / graphMax;

                            bm.SetPixel(x, height - (y), Color.White);
                        }
                    }
                    else
                    {
                        for (int i = y2; i < y1; i++)
                        {
                            int y = i * height / graphMax;
                            int x = x2 + (int)((i-y2) * invSlope);
                            x = x * width / graphMax;

                            bm.SetPixel(x, height - (y), Color.White);
                        }
                    }
                }

                pictureBox1.Image = bm;
            }
        }

        // Draws the lines connecting each point in order in the given path
        private void DrawPath(int[,] inputSet, int[] path, int graphMax)
        {
            DrawLine(inputSet[path[0], 0], inputSet[path[0], 1], inputSet[path[1], 0], inputSet[path[1], 1], graphMax);
            DrawLine(inputSet[path[1], 0], inputSet[path[1], 1], inputSet[path[2], 0], inputSet[path[2], 1], graphMax);
            DrawLine(inputSet[path[2], 0], inputSet[path[2], 1], inputSet[path[3], 0], inputSet[path[3], 1], graphMax);
            DrawLine(inputSet[path[3], 0], inputSet[path[3], 1], inputSet[path[4], 0], inputSet[path[4], 1], graphMax);
            DrawLine(inputSet[path[4], 0], inputSet[path[4], 1], inputSet[path[5], 0], inputSet[path[5], 1], graphMax);
            DrawLine(inputSet[path[5], 0], inputSet[path[5], 1], inputSet[path[6], 0], inputSet[path[6], 1], graphMax);
            DrawLine(inputSet[path[6], 0], inputSet[path[6], 1], inputSet[path[7], 0], inputSet[path[7], 1], graphMax);
            DrawLine(inputSet[path[7], 0], inputSet[path[7], 1], inputSet[path[8], 0], inputSet[path[8], 1], graphMax);
            DrawLine(inputSet[path[8], 0], inputSet[path[8], 1], inputSet[path[9], 0], inputSet[path[9], 1], graphMax);
        }

    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 
    // DISTANCE CALCULATING FUNCTIONS
    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 

        // Calculates the scale of the graph
        private int CalculateGraphMax(int[,] inputSet)
        {
            int temp = inputSet[0, 0];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (temp < inputSet[i, j])
                    {
                        temp = inputSet[i, j];
                    }
                }
            }

            temp /= 100;
            temp++;
            temp *= 100;

            return temp;
        }

        // Generates a 10x10 table of distances between each point
        private void GenerateDistanceTable(int[,] inputSet, ref double[,] distTable)
        {
            distTable = new double[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int tempX = (inputSet[i, 0] - inputSet[j, 0]);
                    tempX *= tempX;
                    int tempY = (inputSet[i, 1] - inputSet[j, 1]);
                    tempY *= tempY;

                    distTable[i, j] = Math.Sqrt(tempX + tempY);
                }
            }
        }

        // Calculates the sum of the distances of the given path and returns it
        private double CalculateTotalDistance(double[,] distTable, int[] route)
        {
            return distTable[route[0], route[1]] + distTable[route[1], route[2]] + distTable[route[2], route[3]]
                 + distTable[route[3], route[4]] + distTable[route[4], route[5]] + distTable[route[5], route[6]]
                 + distTable[route[6], route[7]] + distTable[route[7], route[8]] + distTable[route[8], route[9]];
        }

    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 
    // SHORTEST PATH SEARCHING FUNCTIONS
    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 

        // ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
        // EXHAUSTIVE SEARCH 
        // ---- ---- ---- ---- ---- ---- ---- ---- ---- ----

        // Exhaustively searches each path to find the shortest route
        private void ExhaustiveSearch(int[,] inputSet, double[,] distTable, ref int[] shortestRoute, int graphMax, int id)
        {
            // As I couldn't figure out how to iterate through each permutation without repetition
            // I simply when through every combination with repetition and ignored the ones that didn't fit

            int debugCounter = 0;

            shortestRoute = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            double shortestDistance = CalculateTotalDistance(distTable, shortestRoute);

            int[] currentRoute = new int[10];
            double currentDistance;

            int tenThousandCounter = 0;
            int progressBarCounter = 0;

            if (id == 1)
            {
                progressBar1.Value = progressBarCounter;
            }
            else if (id == 2)
            {
                progressBar2.Value = progressBarCounter;
            }
            else if (id == 3)
            {
                progressBar3.Value = progressBarCounter;
            }
            Application.DoEvents();

            for (int a = 0; a < 10; a++)
            {
                for (int b = 0; b < 10; b++)
                {
                    for (int c = 0; c < 10; c++)
                    {
                        for (int d = 0; d < 10; d++)
                        {
                            for (int e = 0; e < 10; e++)
                            {
                                for (int f = 0; f < 10; f++)
                                {
                                    for (int g = 0; g < 10; g++)
                                    {
                                        for (int h = 0; h < 10; h++)
                                        {
                                            for (int i = 0; i < 10; i++)
                                            {
                                                for (int j = 0; j < 10; j++)
                                                {
                                                    // Update Progress Bar
                                                    tenThousandCounter++;
                                                    if (tenThousandCounter == 10000)
                                                    {
                                                        tenThousandCounter = 0;
                                                        progressBarCounter++;
                                                        if (id == 1)
                                                        {
                                                            progressBar1.Value = progressBarCounter;
                                                        }
                                                        else if (id == 2)
                                                        {
                                                            progressBar2.Value = progressBarCounter;
                                                        }
                                                        else if (id == 3)
                                                        {
                                                            progressBar3.Value = progressBarCounter;
                                                        }
                                                        Application.DoEvents();
                                                    }

                                                    // Skip All Repetitions
                                                    if (a == b || a == c || a == d || a == e || a == f || a == g || a == h || a == i || a == j)
                                                    {
                                                        continue;
                                                    }

                                                    if (b == c || b == d || b == e || b == f || b == g || b == h || b == i || b == j)
                                                    {
                                                        continue;
                                                    }

                                                    if (c == d || c == e || c == f || c == g || c == h || c == i || c == j)
                                                    {
                                                        continue;
                                                    }

                                                    if (d == e || d == f || d == g || d == h || d == i || d == j)
                                                    {
                                                        continue;
                                                    }

                                                    if (e == f || e == g || e == h || e == i || e == j)
                                                    {
                                                        continue;
                                                    }

                                                    if (f == g || f == h || f == i || f == j)
                                                    {
                                                        continue;
                                                    }

                                                    if (g == h || g == i || g == j)
                                                    {
                                                        continue;
                                                    }

                                                    if (h == i || h == j)
                                                    {
                                                        continue;
                                                    }

                                                    if (i == j)
                                                    {
                                                        continue;
                                                    }

                                                    // Generate Route
                                                    currentRoute[0] = a;
                                                    currentRoute[1] = b;
                                                    currentRoute[2] = c;
                                                    currentRoute[3] = d;
                                                    currentRoute[4] = e;
                                                    currentRoute[5] = f;
                                                    currentRoute[6] = g;
                                                    currentRoute[7] = h;
                                                    currentRoute[8] = i;
                                                    currentRoute[9] = j;

                                                    // Calculate Total Distance
                                                    currentDistance = CalculateTotalDistance(distTable, currentRoute);

                                                    // Update Shortest Route
                                                    if (shortestDistance > currentDistance)
                                                    {
                                                        // Apply a deep copy instead of a pointer swtich
                                                        shortestRoute[0] = currentRoute[0];
                                                        shortestRoute[1] = currentRoute[1];
                                                        shortestRoute[2] = currentRoute[2];
                                                        shortestRoute[3] = currentRoute[3];
                                                        shortestRoute[4] = currentRoute[4];
                                                        shortestRoute[5] = currentRoute[5];
                                                        shortestRoute[6] = currentRoute[6];
                                                        shortestRoute[7] = currentRoute[7];
                                                        shortestRoute[8] = currentRoute[8];
                                                        shortestRoute[9] = currentRoute[9];

                                                        // Update the minimum distance
                                                        shortestDistance = currentDistance;

                                                        // Update the label
                                                        if (id == 1)
                                                        {
                                                            label1.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                                                               + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                                                               + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                                                               + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                                                               + shortestRoute[8] + ", " + shortestRoute[9] + "}";
                                                        }
                                                        else if (id == 2)
                                                        {
                                                            label2.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                                                               + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                                                               + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                                                               + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                                                               + shortestRoute[8] + ", " + shortestRoute[9] + "}";
                                                        }
                                                        else if (id == 3)
                                                        {
                                                            label3.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                                                               + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                                                               + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                                                               + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                                                               + shortestRoute[8] + ", " + shortestRoute[9] + "}";
                                                        }

                                                        // Draw the new shortest path
                                                        ClearImage(Color.DarkGray);
                                                        DrawGrid(Color.LightGray, graphMax);
                                                        DrawPath(inputSet, shortestRoute, graphMax);
                                                        PlotPoints(inputSet, Color.Black, graphMax);
                                                        Application.DoEvents();

                                                        // Update Debug Console Output
                                                        debugCounter++;
                                                        Console.WriteLine("Route Updated: " + debugCounter);
                                                    }


                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Finished");
        }

        // ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
        // SIMULATED ANNEALING
        // ---- ---- ---- ---- ---- ---- ---- ---- ---- ----

        // Randomly generate a path
        private void GeneratePath(ref int[] path)
        {
            bool[] selected = new bool[10] { false, false, false, false, false, false, false, false, false, false };

            path = new int[10];

            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                // bool assigned = false;

                while (true)
                {
                    double temp = r.NextDouble();
                    int location = (int)(temp * 10);

                    if (!selected[location])
                    {
                        selected[location] = true;
                        path[i] = location;
                        break;
                    }

                    /*
                    if (temp >= 0.0 && temp < 0.1 && !selected[0])
                    {
                        assigned = true;
                        selected[0] = true;
                        path[i] = 0;
                    }

                    if (temp >= 0.1 && temp < 0.2 && !selected[1])
                    {
                        assigned = true;
                        selected[1] = true;
                        path[i] = 1;
                    }

                    if (temp >= 0.2 && temp < 0.3 && !selected[2])
                    {
                        assigned = true;
                        selected[2] = true;
                        path[i] = 2;
                    }

                    if (temp >= 0.3 && temp < 0.4 && !selected[3])
                    {
                        assigned = true;
                        selected[3] = true;
                        path[i] = 3;
                    }

                    if (temp >= 0.4 && temp < 0.5 && !selected[4])
                    {
                        assigned = true;
                        selected[4] = true;
                        path[i] = 4;
                    }

                    if (temp >= 0.5 && temp < 0.6 && !selected[5])
                    {
                        assigned = true;
                        selected[5] = true;
                        path[i] = 5;
                    }

                    if (temp >= 0.6 && temp < 0.7 && !selected[6])
                    {
                        assigned = true;
                        selected[6] = true;
                        path[i] = 6;
                    }

                    if (temp >= 0.7 && temp < 0.8 && !selected[7])
                    {
                        assigned = true;
                        selected[7] = true;
                        path[i] = 7;
                    }

                    if (temp >= 0.8 && temp < 0.9 && !selected[8])
                    {
                        assigned = true;
                        selected[8] = true;
                        path[i] = 8;
                    }

                    if (temp >= 0.9 && temp < 1.0 && !selected[9])
                    {
                        assigned = true;
                        selected[9] = true;
                        path[i] = 9;
                    }

                    if (assigned)
                    {
                        break;
                    }
                    */
                }


            }
        }

        // Finds the shortest path via Simulated Annealing
        private void SimulatedAnnealing(int[,] inputSet, double[,] distTable, ref int[] shortestRoute, int id)
        {
            shortestRoute = new int[10];

            // Generate Initial Path
            int[] currentRoute = new int[10];
            GeneratePath(ref currentRoute);

            // Find the distance of the initial path
            double currentDistance = CalculateTotalDistance(distTable, currentRoute);
            double shortestDistance = currentDistance;

            // Set initial temperature to the distance of initial path
            double temperature = currentDistance;


            for (int i = 0; i < 10; i++)
            {
                shortestRoute[i] = currentRoute[i];
            }

            Random r = new Random();
            // Loop while temperature gradually cools
            while (temperature > 1.0)
            {
                // Create a copy of the current route
                int[] alteredRoute = new int[10];

                for (int i = 0; i < 10; i++)
                {
                    alteredRoute[i] = currentRoute[i];
                }

                // Swap a point in the altered route
                int location01 = (int)(r.NextDouble() * 10);
                int location02 = (int)(r.NextDouble() * 10);

                while (location01 == location02)
                {
                    location02 = (int)(r.NextDouble() * 10);
                }

                int temp = alteredRoute[location01];
                alteredRoute[location01] = alteredRoute[location02];
                alteredRoute[location02] = temp;

                // Find the distance of the altered route
                double alteredDistance = CalculateTotalDistance(distTable, alteredRoute);

                // Console.WriteLine("Current: " + currentDistance + "    Altered: " + alteredDistance);


                // Decide if the altered route is kept based on distance and/or temperature
                if (alteredDistance < currentDistance)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        currentRoute[i] = alteredRoute[i];
                    }

                    currentDistance = alteredDistance;
                    // Console.WriteLine("Updated A");
                }
                else if (((alteredDistance - currentDistance) / temperature) < TOLERANCE)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        currentRoute[i] = alteredRoute[i];
                    }

                    currentDistance = alteredDistance;
                    // Console.WriteLine("Updated B");
                }

                // Record shortest distance and path
                if (currentDistance < shortestDistance)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        shortestRoute[i] = currentRoute[i];
                    }

                    shortestDistance = currentDistance;
                }

                // Decrease temperature by a set percentage
                temperature = temperature * (1.0 - COOLING_RATE);
            }

            // Update progress bar
            if (id == 1)
            {
                progressBar1.Value = progressBar1.Maximum;
            }
            else if (id == 2)
            {
                progressBar2.Value = progressBar2.Maximum;
            }
            else if (id == 3)
            {
                progressBar3.Value = progressBar3.Maximum;
            }

            // Update the label
            if (id == 1)
            {
                label1.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                   + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                   + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                   + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                   + shortestRoute[8] + ", " + shortestRoute[9] + "}";
            }
            else if (id == 2)
            {
                label2.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                   + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                   + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                   + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                   + shortestRoute[8] + ", " + shortestRoute[9] + "}";
            }
            else if (id == 3)
            {
                label3.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                   + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                   + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                   + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                   + shortestRoute[8] + ", " + shortestRoute[9] + "}";
            }

            Application.DoEvents();

            // Debugging output to console
            Console.WriteLine("Finished");
        }

        // ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
        // GENETIC ALGORITHM
        // ---- ---- ---- ---- ---- ---- ---- ---- ---- ----

        // Initializes the population by generating random paths
        private void InitializePopulation(ref int[,] population, int size)
        {
            Random r = new Random();
            for (int i = 0; i < size; i++)
            {
                bool[] selected = new bool[10] { false, false, false, false, false, false, false, false, false, false };

                int[] path = new int[10];

                for (int j = 0; j < 10; j++)
                {
                    while (true)
                    {
                        double temp = r.NextDouble();
                        int location = (int)(temp * 10);

                        if (!selected[location])
                        {
                            selected[location] = true;
                            path[j] = location;
                            break;
                        }
                    }
                }

                // Copy the path into the population array
                for (int j = 0; j < 10; j++)
                {
                    population[i, j] = path[j];
                }
            }
        }

        private void GeneticAlgorithm(int[,] inputSet, double[,] distTable, ref int[,] population, int size, ref int[] shortestRoute, int iterations, int id)
        {
            // Generate Initial Population
            population = new int[size, 10];
            InitializePopulation(ref population, size);

            // Temperary Storage of Current Path
            int[] tempPath = new int[10];

            // Array of total distance of each path in population
            double[] distances = new double[size];

            // Fill Distance Array
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    tempPath[j] = population[i, j];
                }

                distances[i] = CalculateTotalDistance(distTable, tempPath);

                // Console.WriteLine("Distance of Path 0" + (i + 1) + ": " + distances[i]);
            }

            // Shortest Distance and Index in Population
            double shortestDistance = distances[0];
            int shortestPathIndex = -1;
            int[] shortestPath = new int[10];

            // Get Shortest
            for (int i = 0; i < size; i++)
            {
                if (shortestDistance > distances[i])
                {
                    shortestDistance = distances[i];
                    shortestPathIndex = i;
                    for (int j = 0; j < 10; j++)
                    {
                        shortestPath[j] = population[i, j];
                    }
                }
            }

            // DEBUG
            // Console.WriteLine("Path " + (shortestPathIndex + 1) + " is the shortest");

            // Genetic Algorithm Loop a Given Amount of Times
            Random r = new Random();
            for (int limit = 0; limit < iterations; limit++)
            {
                // Calculate the Total Distance^2 of the Populations
                double totalDistance = 0;
                for (int i = 0; i < size; i++)
                {
                    totalDistance += (distances[i] * distances[i]);
                }

                // Get a Fitness Value (Total/Distance^2) to add Weight to Shorter Distances
                double[] fitness = new double[size];
                for (int i = 0; i < size; i++)
                {
                    fitness[i] = totalDistance / (distances[i] * distances[i]);
                }

                // Get the Total of the Fitness Values
                double totalFitness = 0;
                for (int i = 0; i < size; i++)
                {
                    totalFitness += fitness[i];
                }
                
                // Get a Percentage of Each Fitness Value based on the Total
                double[] percentage = new double[size];
                for (int i = 0; i < size; i++)
                {
                    percentage[i] = fitness[i] / totalFitness;

                    // Console.WriteLine("Fitness Percentage 0" + (i + 1) + ": " + percentage[i]);
                }

                /*
                // DEBUG
                for (int i = 0; i < size; i++)
                {
                    Console.WriteLine("Old Population 0" + (i + 1) + ":");
                    for (int j = 0; j < 10; j++)
                    {
                        Console.WriteLine(population[i, j]);
                    }
                }
                */

                // Select Parents (size/2) times
                int[,] tempPopulation = new int[size, 10];
                for (int h = 0; h < (size/2); h++)
                {
                    int parentIndex01 = -1;
                    int parentIndex02 = -1;

                    // Select parent based on roulette wheel
                    double temp = r.NextDouble();
                    double currentPercentage = 0;
                    for (int i = 0; i < size; i++)
                    {
                        currentPercentage += percentage[i];
                        if (temp < currentPercentage)
                        {
                            parentIndex01 = i;
                            break;
                        }
                    }

                    // Select a different parent based on roulette wheel
                    while(true)
                    {
                        temp = r.NextDouble();
                        currentPercentage = 0;
                        for (int i = 0; i < size; i++)
                        {
                            currentPercentage += percentage[i];
                            if (temp < currentPercentage)
                            {
                                parentIndex02 = i;
                                break;
                            }
                        }
                        if (parentIndex01 != parentIndex02)
                        {
                            break;
                        }
                    }

                    // DEBUG
                    // Console.WriteLine("Parent 01: " + (parentIndex01 + 1));
                    // Console.WriteLine("Parent 02: " + (parentIndex02 + 1));

                    int crossoverIndex01 = -1;
                    int crossoverIndex02 = -1;

                    crossoverIndex01 = (int)(r.NextDouble() * 10);
                    while(true)
                    {
                        crossoverIndex02 = (int)(r.NextDouble() * 10);
                        if (crossoverIndex01 != crossoverIndex02)
                        {
                            break;
                        }
                    }

                    if (crossoverIndex01 > crossoverIndex02)
                    {
                        int tempInt = crossoverIndex01;
                        crossoverIndex01 = crossoverIndex02;
                        crossoverIndex02 = tempInt;
                    }

                    // DEBUG
                    // Console.WriteLine("Crossover Index 01: " + (crossoverIndex01));
                    // Console.WriteLine("Crossover Index 02: " + (crossoverIndex02));

                    bool[] filled01 = new bool[] { false, false, false, false, false, false, false, false, false, false };
                    bool[] filled02 = new bool[] { false, false, false, false, false, false, false, false, false, false };

                    for (int j = crossoverIndex01; j < crossoverIndex02; j++)
                    {
                        tempPopulation[h * 2, j] = population[parentIndex01, j];
                        tempPopulation[h * 2 + 1, j] = population[parentIndex02, j];
                        filled01[j] = true;
                        filled02[j] = true;
                    }

                    for (int j = 0; j < 10; j++)
                    {
                        bool allow = true;
                        for (int k = crossoverIndex01; k < crossoverIndex02; k++)
                        {
                            if (population[parentIndex02, j] == tempPopulation[h * 2, k])
                            {
                                allow = false;
                            }
                        }

                        if (allow)
                        {
                            for (int k = 0; k < 10; k++)
                            {
                                if (!filled01[k])
                                {
                                    filled01[k] = true;
                                    tempPopulation[h * 2, k] = population[parentIndex02, j];
                                    break;
                                }
                            }
                        }
                    }


                    for (int j = 0; j < 10; j++)
                    {
                        bool allow = true;
                        for (int k = crossoverIndex01; k < crossoverIndex02; k++)
                        {
                            if (population[parentIndex01, j] == tempPopulation[h * 2 + 1, k])
                            {
                                allow = false;
                            }
                        }

                        if (allow)
                        {
                            for (int k = 0; k < 10; k++)
                            {
                                if (!filled02[k])
                                {
                                    filled02[k] = true;
                                    tempPopulation[h * 2 + 1, k] = population[parentIndex01, j];
                                    break;
                                }
                            }
                        }
                    }



                }

                /*
                // DEBUG
                for (int i = 0; i < size; i++)
                {
                    Console.WriteLine("New Population 0" + (i + 1) + ":");
                    for (int j = 0; j < 10; j++)
                    {
                        Console.WriteLine(tempPopulation[i, j]);
                    }
                }
                */

                
                // Apply Mutation
                for (int i = 0; i < size; i++)
                {
                    double temp = r.NextDouble();
                    if(temp < MUTATION)
                    {
                        // Console.WriteLine("Mutating at Path 0" + (i + 1) + " with new path:");

                        int mutateIndex01 = -1;
                        int mutateIndex02 = -1;

                        mutateIndex01 = (int)(r.NextDouble() * 10);
                        while (true)
                        {
                            mutateIndex02 = (int)(r.NextDouble() * 10);
                            if (mutateIndex01 != mutateIndex02)
                            {
                                break;
                            }
                        }

                        // Console.WriteLine("Swaping " + mutateIndex01 + ": " + tempPopulation[i, mutateIndex01] + " with " + mutateIndex02 + ": " + tempPopulation[i, mutateIndex02]);
                        
                        int tempInt = tempPopulation[i, mutateIndex01];
                        tempPopulation[i, mutateIndex01] = tempPopulation[i, mutateIndex02];
                        tempPopulation[i, mutateIndex02] = tempInt;
                        
                        for (int j = 0; j < 10; j++)
                        {
                            // Console.WriteLine(tempPopulation[i, j]);
                        }
                    }
                }
                

                // Update population to next generation
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        population[i, j] = tempPopulation[i, j];
                    }
                }

                // Fill Distance Array
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        tempPath[j] = population[i, j];
                    }

                    distances[i] = CalculateTotalDistance(distTable, tempPath);

                    // Console.WriteLine("Distance of Path 0" + (i + 1) + ": " + distances[i]);
                }

                // Get Shortest
                for (int i = 0; i < size; i++)
                {
                    if (shortestDistance > distances[i])
                    {
                        shortestDistance = distances[i];
                        shortestPathIndex = i;
                        for (int j = 0; j < 10; j++)
                        {
                            shortestPath[j] = population[i, j];
                        }
                    }
                }

                // DEBUG
                // Console.WriteLine("The shortest distance of " + shortestDistance);

            }

            shortestRoute = new int[10];
            for (int i = 0; i < 10; i++)
            {
                shortestRoute[i] = shortestPath[i];
            }

            // Update progress bar
            if (id == 1)
            {
                progressBar1.Value = progressBar1.Maximum;
            }
            else if (id == 2)
            {
                progressBar2.Value = progressBar2.Maximum;
            }
            else if (id == 3)
            {
                progressBar3.Value = progressBar3.Maximum;
            }

            // Update the label
            if (id == 1)
            {
                label1.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                   + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                   + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                   + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                   + shortestRoute[8] + ", " + shortestRoute[9] + "}";
            }
            else if (id == 2)
            {
                label2.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                   + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                   + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                   + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                   + shortestRoute[8] + ", " + shortestRoute[9] + "}";
            }
            else if (id == 3)
            {
                label3.Text = "{ " + shortestRoute[0] + ", " + shortestRoute[1] + ", "
                                   + shortestRoute[2] + ", " + shortestRoute[3] + ", "
                                   + shortestRoute[4] + ", " + shortestRoute[5] + ", "
                                   + shortestRoute[6] + ", " + shortestRoute[7] + ", "
                                   + shortestRoute[8] + ", " + shortestRoute[9] + "}";
            }

            Application.DoEvents();

            // Debugging output to console
            Console.WriteLine("Finished");
        }

    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 
    // DEBUGGING FUCTIONS
    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 

        // General testing function to allow for easy debuging
        private void DebugTest()
        {
            // If statement to collapse commented out debug code
            if (false)
            {
                // Console.WriteLine("Height: " + pictureBox1.ClientSize.Height);
                // Console.WriteLine("Width:  " + pictureBox1.ClientSize.Width);
                // PlotPoints(set01, Color.White);
                /*
                for(int i = 1; i < 10; i++)
                {
                    DrawLine(set01[0, 0], set01[0, 1], set01[i, 0], set01[i, 1]);
                }
                */
                //PlotPoints(set01, Color.Black);

                /*
                ExhaustiveSearch(set03, distanceTable03, ref shortestPath03);

                Console.WriteLine("Fishied");
                Console.WriteLine(shortestPath03[0]);
                Console.WriteLine(shortestPath03[1]);
                Console.WriteLine(shortestPath03[2]);
                Console.WriteLine(shortestPath03[3]);
                Console.WriteLine(shortestPath03[4]);
                Console.WriteLine(shortestPath03[5]);
                Console.WriteLine(shortestPath03[6]);
                Console.WriteLine(shortestPath03[7]);
                Console.WriteLine(shortestPath03[8]);
                Console.WriteLine(shortestPath03[9]);

                DrawPath(set03, shortestPath03);
                PlotPoints(set03, Color.Black);
                */

                // Set 01 Shortest Path:  { 7 6 8 5 9 3 2 4 0 1 }
                // Set 02 Shortest Path:  { 2 1 4 9 3 8 7 5 0 6 }
                // Set 03 Shortest Path:  { 3 5 4 6 7 8 1 2 0 9 }

                /*
                shortestPath01 = new int[10] { 7, 6, 8, 5, 9, 3, 2, 4, 0, 1 };
                shortestPath02 = new int[10] { 2, 1, 4, 9, 3, 8, 7, 5, 0, 6 };
                shortestPath03 = new int[10] { 3, 5, 4, 6, 7, 8, 1, 2, 0, 9 };


                label1.Text = "{ " + shortestPath01[0] + ", " + shortestPath01[1] + ", "
                                   + shortestPath01[2] + ", " + shortestPath01[3] + ", "
                                   + shortestPath01[4] + ", " + shortestPath01[5] + ", "
                                   + shortestPath01[6] + ", " + shortestPath01[7] + ", "
                                   + shortestPath01[8] + ", " + shortestPath01[9] + "}";

                label2.Text = "{ " + shortestPath02[0] + ", " + shortestPath02[1] + ", "
                                   + shortestPath02[2] + ", " + shortestPath02[3] + ", "
                                   + shortestPath02[4] + ", " + shortestPath02[5] + ", "
                                   + shortestPath02[6] + ", " + shortestPath02[7] + ", "
                                   + shortestPath02[8] + ", " + shortestPath02[9] + "}";

                label3.Text = "{ " + shortestPath03[0] + ", " + shortestPath03[1] + ", "
                                   + shortestPath03[2] + ", " + shortestPath03[3] + ", "
                                   + shortestPath03[4] + ", " + shortestPath03[5] + ", "
                                   + shortestPath03[6] + ", " + shortestPath03[7] + ", "
                                   + shortestPath03[8] + ", " + shortestPath03[9] + "}";

                searchCompleted01 = true;
                searchCompleted02 = true;
                searchCompleted03 = true;
                */

                // DrawGrid(Color.LightGray, xyMaximum03);




                /*
                SimulatedAnnealing(set01, distanceTable01, ref shortestPath01, 1);
                Console.WriteLine("Initial Path 01");
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(shortestPath01[i]);
                }


                SimulatedAnnealing(set02, distanceTable02, ref shortestPath02, 2);
                Console.WriteLine("Initial Path 02");
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(shortestPath02[i]);
                }

                SimulatedAnnealing(set03, distanceTable03, ref shortestPath03, 3);
                Console.WriteLine("Initial Path 03");
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(shortestPath03[i]);
                }
                */
            }

            /*
            int[,] testPopulation = new int[4, 10];
            InitializePopulation(ref testPopulation, 4);
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Path 0" + (i + 1) + ":");
                for (int j = 0; j < 10; j++)
                {
                    Console.WriteLine(testPopulation[i, j]);
                }
            }
            */


            GeneticAlgorithm(set01, distanceTable01, ref population01, POPULATION_SIZE, ref shortestPath01, GENERATIONS, 1);
            searchCompleted01 = true;


        }

    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 
    // WINDOWS FORM APPLICATION FUNCTIONS
    // **** **** **** **** **** **** **** **** **** **** **** **** **** **** **** 

        // Set 01 Button Click
        private void B01_Click(object sender, EventArgs e)
        {
            if (!searchCompleted01)
            {
                searchCompleted01 = true;
                ClearImage(Color.DarkGray);
                DrawGrid(Color.LightGray, xyMaximum01);
                PlotPoints(set01, Color.Black, xyMaximum01);
                // ExhaustiveSearch(set01, distanceTable01, ref shortestPath01, xyMaximum01, 1);
                // SimulatedAnnealing(set01, distanceTable01, ref shortestPath01, 1);
                GeneticAlgorithm(set01, distanceTable01, ref population01, POPULATION_SIZE, ref shortestPath01, GENERATIONS, 1);
            }
            ClearImage(Color.DarkGray);
            DrawGrid(Color.LightGray, xyMaximum01);
            DrawPath(set01, shortestPath01, xyMaximum01);
            PlotPoints(set01, Color.Black, xyMaximum01);
        }

        // Set 02 Button Click
        private void B02_Click(object sender, EventArgs e)
        {
            if (!searchCompleted02)
            {
                searchCompleted02 = true;
                ClearImage(Color.DarkGray);
                DrawGrid(Color.LightGray, xyMaximum02);
                PlotPoints(set02, Color.Black, xyMaximum02);
                // ExhaustiveSearch(set02, distanceTable02, ref shortestPath02, xyMaximum02, 2);
                // SimulatedAnnealing(set02, distanceTable02, ref shortestPath02, 2);
                GeneticAlgorithm(set02, distanceTable02, ref population02, POPULATION_SIZE, ref shortestPath02, GENERATIONS, 2);
            }
            ClearImage(Color.DarkGray);
            DrawGrid(Color.LightGray, xyMaximum02);
            DrawPath(set02, shortestPath02, xyMaximum02);
            PlotPoints(set02, Color.Black, xyMaximum02);
        }

        // Set 03 Button Click
        private void B03_Click(object sender, EventArgs e)
        {
            if (!searchCompleted03)
            {
                searchCompleted03 = true;
                ClearImage(Color.DarkGray);
                DrawGrid(Color.LightGray, xyMaximum03);
                PlotPoints(set03, Color.Black, xyMaximum03);
                // ExhaustiveSearch(set03, distanceTable03, ref shortestPath03, xyMaximum03, 3);
                // SimulatedAnnealing(set03, distanceTable03, ref shortestPath03, 3);
                GeneticAlgorithm(set03, distanceTable03, ref population03, POPULATION_SIZE, ref shortestPath03, GENERATIONS, 3);
            }
            ClearImage(Color.DarkGray);
            DrawGrid(Color.LightGray, xyMaximum03);
            DrawPath(set03, shortestPath03, xyMaximum03);
            PlotPoints(set03, Color.Black, xyMaximum03);
        }

        // Reset Set 01 Button Click - Allows user to retry set 01
        private void B04_Click(object sender, EventArgs e)
        {
            progressBar1.Value = progressBar1.Minimum;
            searchCompleted01 = false;
            label1.Text = "{  }";
            B01_Click(sender, e);
        }

        // Reset Set 02 Button Click - Allows user to retry set 02
        private void B05_Click(object sender, EventArgs e)
        {
            progressBar2.Value = progressBar2.Minimum;
            searchCompleted02 = false;
            label2.Text = "{  }";
            B02_Click(sender, e);
        }

        // Reset Set 03 Button Click - Allows user to retry set 03
        private void B06_Click(object sender, EventArgs e)
        {
            progressBar3.Value = progressBar3.Minimum;
            searchCompleted03 = false;
            label3.Text = "{  }";
            B03_Click(sender, e);
        }
    }
}
