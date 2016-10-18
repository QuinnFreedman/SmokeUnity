using UnityEngine;
using System.Collections.Generic;
using System;

public class AStar
{

    private static Node[,] graph = null;

    public static void InitGraph(int[,] level)
    {
        if (level == null)
        {
            Debug.LogError("Pathfinding graph can't be initialized with a null level");
            return;
        }
        graph = new Node[level.GetLength(0), level.GetLength(1)];
        for (int y = 0; y < level.GetLength(0); y++)
        {
            for (int x = 0; x < level.GetLength(1); x++)
            {
                graph[y, x] = new Node(x, y, level[y, x] <= 1);
            }
        }

    }

    static List<Point> GetPath(Point startpoint, Point endpoint, int maxIterations)
    {
        if (graph == null)
        {
            InitGraph(CreateWorld.level);
        }
        return GetPath(startpoint, endpoint, maxIterations, graph);
    }

    static List<Point> GetPath(Point startpoint, Point endpoint, int maxIterations, Node[,] nodes)
    {
        var path = new List<Node>();
        var openList = new List<Node>();
        var closedList = new List<Node>();

        nodes[startpoint.y, startpoint.x].h = distanceBetween(nodes[startpoint.y, startpoint.x], nodes[endpoint.y, endpoint.x]);
        closedList.Add(nodes[startpoint.y, startpoint.x]);
        nodes[startpoint.y, startpoint.x].g = 0;

        int g = 0;
        bool done = false;
        int itt = 0;
        do
        {
            for (int h = 0; h < 4; h++)
            {
                int localX = closedList[g].x;
                int localY = closedList[g].y;

                if (h == 0)
                {
                    localY++;
                }
                else if (h == 1)
                {
                    localX++;
                }
                else if (h == 2)
                {
                    localY--;
                }
                else if (h == 3)
                {
                    localX--;
                }
                //if h exists
                if (localX >= 0 && localY >= 0 && localY < nodes.GetLength(0) && localX < nodes.GetLength(1))
                {
                    //if h is not on open or closed list or is impassable
                    if (!openList.Contains(nodes[localY, localX]) && !closedList.Contains(nodes[localY, localX]) && nodes[localY, localX].isPassable == true)
                    {

                        nodes[localY, localX].parent = closedList[g]; // parent current node to new node
                        nodes[localY, localX].g = closedList[g].g + 1; // set move cost
                        nodes[localY, localX].h = distanceBetween(nodes[localY, localX], nodes[endpoint.y, endpoint.x]);
                        nodes[localY, localX].f = nodes[localY, localX].g + nodes[localY, localX].h;

                        openList.Add(nodes[localY, localX]); // add new node to open list



                    }
                    else if (closedList.Contains(nodes[localY, localX]))
                    {
                        if (closedList[g].g + 1 < nodes[localY, localX].g)
                        {
                            nodes[localY, localX].parent = closedList[g];
                            nodes[localY, localX].g = closedList[g].g + 1;
                            nodes[localY, localX].f = nodes[localY, localX].g + nodes[localY, localX].h;
                        }
                    }
                    else if (localY == endpoint.y && localX == endpoint.x)
                    {
                        nodes[endpoint.y, endpoint.x].parent = closedList[g];

                        done = true;
                    }
                }
            }

            //loop through openList - find lowest f
            Node lowestF = null;

            if (openList.Count > 0)
            {
                foreach (Node test in openList)
                {
                    if (lowestF == null || test.f < lowestF.f)
                    {
                        lowestF = test;
                    }
                }
            }
            else
            {
                done = true;
            }


            if (!closedList.Contains(lowestF))
            {
                closedList.Add(lowestF); //move next node to be checked to closed list
            }
            if (openList.Contains(lowestF))
            {
                openList.Remove(lowestF); //and remove from open list
            }

            //loop do again
            g++;
            itt++;

        } while (done == false && (maxIterations == 0 || itt < maxIterations));

        //Trace back
        if (done == true)
        {
            Node currentTile = nodes[endpoint.y, endpoint.x].parent;
            if (currentTile != null)
            {
                while (currentTile.parent != null)
                {
                    //Console.log(currentTile.parent.toString()+": "+currentTile.parent.stringify());
                    path.Add(currentTile);
                    currentTile = currentTile.parent;
                }
            }
            else
            {
                Debug.Log("pathing error");
                return null;
            }
        }
        else
        {
            Debug.Log("pathing error");
            return null;
        }
        List<Point> pointPath = new List<Point>(path.Count);
        for (int i = path.Count - 1; i >= 0; i--)
        {
            pointPath.Add((Point)path[i]);
        }
        return pointPath;
    }

    private static int distanceBetween(Point a, Point b)
    {
        int d = Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        return d;
    }

    internal class Node : Point
    {
        //float gf; //float move cost
        //float ff; //float f
        public int h;//heuristic
        public int g;//movement cost
        public int f;//g+h
        public bool isPassable;
        public Node parent;

        public Node(int x, int y, bool passable) : base(x, y)
        {
            isPassable = passable;
        }

        public Node(int x, int y) : base(x, y) { }
    }
}
