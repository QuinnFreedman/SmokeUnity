using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

class Room {
	public int x, y, width, height;
	public int numDoors;
	public List<Point> roomWalls;
	public List<Point> roomDoors;
	private int levelWidth = -1;
	private int levelHeight = -1;

	private static int randomBtwn(int min, int max) {
		//inclusive
		return (int) Mathf.Floor(Random.value * (max - min + 1) + min);
	}

	private static int gaussian(int min, int max) {
		float randNormal;
		do {
			var u1 = Random.value;
			var u2 = Random.value;
			var randStdNormal = Mathf.Sqrt (-2f * Mathf.Log (u1)) * Mathf.Sin (2f * Mathf.PI * u2);
			var mean = (max - min) / 2f + min;
			var stdDev = (max - min) / 6f;
			randNormal = mean + stdDev * randStdNormal;
		} while (randNormal > max || randNormal < min);

		return (int)Mathf.Round (randNormal);
	}
	
	
	private void setWalls(){
		roomWalls = new List<Point>();
		roomDoors = new List<Point>();
		
		for(int i = 0; i < this.width; i++) {
			roomWalls.Add(new Point(this.x+i, this.y));
			roomWalls.Add(new Point(this.x+i, this.y+this.height-1));
		}
		for(int i = 2; i < this.height; i++) {
			roomWalls.Add(new Point(this.x, this.y+i-1));
			roomWalls.Add(new Point(this.x+this.width - 1, this.y+i-1));
		}
	}

	private void setDoors(){
		bool corner;
		for (int e = 1; e <= numDoors; e++) {
			do {	
				corner = false;			
				int k = randomBtwn (0, roomWalls.Count - 1);	
				
				//check for corners
				if ((roomWalls [k].y == this.height + this.y - 1 && roomWalls [k].x == this.width + this.x - 1) ||
				    (roomWalls [k].y == this.y && roomWalls [k].x == this.width + this.x - 1) ||
				    (roomWalls [k].y == this.y && roomWalls [k].x == this.x) ||
				    (roomWalls [k].y == this.height + this.y - 1 && roomWalls [k].x == this.x)) {
					corner = true;
				} else if (roomWalls [k].y == levelHeight - 1 ||
				           roomWalls [k].x == levelWidth - 1 ||
				           roomWalls [k].y == 0 ||
				           roomWalls [k].x == 0) {
					corner = true;
				} else {
					roomDoors.Add (roomWalls [k]);
					roomWalls.RemoveAt (k);
				}
				
			} while(corner == true);
			
		}
		
	}
	public Room(int maxWidth, int maxHeight){
		levelHeight = maxHeight;
		levelWidth = maxWidth;
		this.height = gaussian (4, 7);
		this.width = gaussian (4, 7);
		this.x = gaussian (1, maxWidth - this.width - 1);
		this.y = gaussian (1, maxHeight - this.height - 1);
	}
	
	public Room(int x, int y, int width, int height, int levelWidth, int levelHeight) {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		this.levelWidth = levelWidth;
		this.levelHeight = levelHeight;
	}
	public void construct(){
		Assert.IsTrue(levelWidth != -1 && levelHeight != -1);
		numDoors = gaussian(1,3);
		setWalls();
		setDoors();
	}

	/*
	@Override
	public String ToString () {
		return super.toString()+"[x = "+x+" y = "+y+" w = "+width+" h = "+height+"]";
	}*/
}