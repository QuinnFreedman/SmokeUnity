using System;

public class GridPathNode : AStar.IPathNode<System.Object>
{
	public Int32 X { get; set; }
	public Int32 Y { get; set; }
	public Boolean IsWall {get; set;}
	
	public bool IsWalkable(System.Object unused)
	{
		return !IsWall;
	}

    public GridPathNode(int x, int y, bool isWall) 
    {
        this.X = x;
        this.Y = y;
        this.IsWall = isWall;
    }
}
