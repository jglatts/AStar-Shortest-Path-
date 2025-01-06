/**
*       A* Shortest Path Finding C# Implementation
*       Followed this alg:
*          https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
*          
*       Date: 1/6/2025
*       Author: John Glatts
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class Node
{
    public int g;
    public int f;
    public int h;
#nullable enable
    public Node? parent;
    public (int row, int col) position;

    public Node(Node? parent, (int row, int col) position)
    {
        this.parent = parent;
        this.position = position;
        this.g = 0;
        this.f = 0;
        this.h = 0; 
    }
}

public class Maze
{
    public int[,] maze;
    public int rows;
    public int cols;
    public List<(int row, int col)> walls;

    public Maze(int rows, int cols)
    { 
        maze = new int[rows, cols];
        this.rows = rows;
        this.cols = cols;
        for (int i = 0; i < rows; i++)
        { 
            for (int j = 0; j < cols; j++)
                maze[i, j] = 0;
        }

        walls = new List<(int row, int col)>();
        int start_wall_row = 0;
        int start_wall_col = 5;

        for (int i = 0; i < 8; i++)
        {
            maze[start_wall_row + i, start_wall_col] = 1;
            walls.Add((start_wall_row+i, start_wall_col));
        }
    }
}

public class MazeSolver
{
    private Maze theMaze;
    private List<(int y_scale, int x_scale)> scalar;

    public MazeSolver(Maze maze)
    { 
        this.theMaze = maze;
        scalar = new List<(int y_scale, int x_scale)>()
        {
            (0, -1),
            (0, 1),
            (-1, 0),
            (1, 0),
            (-1, -1),
            (-1, 1),
            (1, -1),
            (1, 1),
        };
    }

    public void DrawMaze(Node start_node, Node end_node, (int row, int col) start, (int row, int col) end)
    {
        // draw the maze
        for (int i = 0; i < theMaze.rows; i++)
        {
            for (int j = 0; j < theMaze.cols; j++)
            {
                if ((i == start.row && j == start.col) ||
                    (i == end.row && j == end.col))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (theMaze.maze[i, j] == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(theMaze.maze[i, j]);
                Console.ResetColor();
            }
            Console.Write("\n");
        }
    }

    public void Solve((int row, int col) start, (int row, int col) end)
    {
        Node start_node = new Node(null, start);
        Node end_node = new Node(null, end);
        List<Node> open_list = new List<Node>();
        List<Node> closed_list = new List<Node>();

        DrawMaze(start_node, end_node, start, end);
        
        open_list.Add(start_node);
        while (open_list.Count != 0)
        {
            Node current_node = open_list[0];
            int current_idx = 0;

            for (int i = 0; i < open_list.Count; i++)
            {
                if (open_list[i].f < current_node.f)
                { 
                    current_idx= i;
                    current_node = open_list[i];
                }
            }

            Node? nodeToRemove = null;
            for (int i = 0; i < open_list.Count; i++)
            {
                if (open_list[i].position.row == current_node.position.row &&
                    open_list[i].position.col == current_node.position.col)
                { 
                    nodeToRemove = open_list[i];
                }
            }

            if (nodeToRemove == null)
            {
                Console.WriteLine("error with removal (openlist popping)");
                break;
            }

            open_list.Remove(nodeToRemove);
            closed_list.Add(current_node);

            if (current_node.position.row == end.row &&
                current_node.position.col == end.col)
            {
                Console.WriteLine("made it!");
                break;
            }

            List<Node> children = new List<Node>();
            for (int i = 0; i < scalar.Count; i++)
            {
                (int y_scale, int x_scale) = scalar[i];
                (int row, int col) node_pos = (current_node.position.row + y_scale,
                                               current_node.position.col + x_scale);

                if (node_pos.row >= theMaze.rows ||
                    node_pos.col >= theMaze.cols)
                {
                    continue;
                }

                if (theMaze.maze[node_pos.row, node_pos.col] != 0)
                {
                    continue;
                }

                Node new_node = new Node(current_node, node_pos);
                children.Add(new_node);
            }

            for (int i = 0; i < children.Count; i++)
            { 
                Node child_node = children[i];
                child_node.g = current_node.g + 1;
                int h = (int)Math.Pow((child_node.position.row - end_node.position.row), 2);
                h += (int)Math.Pow((child_node.position.col - end_node.position.col), 2);
                child_node.h = h;
                child_node.f = child_node.g + child_node.h;
                open_list.Add(child_node);
            }
        }

        Console.WriteLine("\nPath is Below:");
        for (int i = 0; i < theMaze.rows; i++)
        {
            for (int j = 0; j < theMaze.cols; j++)
            {
                if (Check((i, j), closed_list))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (i == end.row && j == end.col)
                { 
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                if (theMaze.maze[i, j] == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(theMaze.maze[i, j]);
                Console.ResetColor();
            }
            Console.Write("\n");
        }
    }

    public bool Check((int row, int col) curr, List<Node> closed_list)
    {
        for (int i = 0; i < closed_list.Count; i++)
        {
            if (curr.row == closed_list[i].position.row &&
                curr.col == closed_list[i].position.col)
            {
                return true;
            }
        }
        return false;
    }

}

public class AStar { 

    public static void Main(String[] args) 
    {
        MazeSolver m = new MazeSolver(new Maze(10, 10));
        m.Solve((1, 1), (7, 9));
    }
}