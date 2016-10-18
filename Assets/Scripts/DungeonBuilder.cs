using System.Collections.Generic;
using UnityEngine;

class DungeonBuilder {

	public static int[,] BuildDungeon(int dungeonWidth, int dungeonHeight, int numRooms) {
		return BuildDungeon (dungeonWidth, dungeonHeight, numRooms, new List<Room> ());
	}

	public static int[,] BuildDungeon(int dungeonWidth, int dungeonHeight, int numRooms, List<Room> rooms) {
		
		int[,] dungeon = new int[dungeonHeight,dungeonWidth];
		for(int i = 0; i < numRooms; i++) {
			rooms.Add(new Room(dungeonWidth, dungeonHeight));
		}
		
		DungeonBuilder.collideRooms(rooms, dungeonWidth, dungeonHeight);
		
		for (int r = 0; r < rooms.Count; r++) {
			for (int i = 0; i < rooms [r].roomWalls.Count; i++) {
				if (rooms [r].roomWalls [i].x >= 0) {
					dungeon [rooms [r].roomWalls [i].y, rooms [r].roomWalls [i].x] = 1;
				}
			}
			for (int y1 = rooms [r].y; y1 < rooms [r].y + rooms [r].height; y1++) {	
				for (int x1 = rooms [r].x; x1 < rooms [r].x + rooms [r].width; x1++) {
					if (dungeon [y1, x1] == 0) {
						dungeon [y1, x1] = 2;
					}
				}
			}
		}
		
		SetPaths(dungeon, rooms, true);
		return dungeon;
	}
	
	private static void collideRooms(List<Room> rooms, int width, int height){
		collideRooms(rooms, width, height, 1);
	}

	private static void collideRooms(List<Room> rooms, int width, int height, int padding){
		int[,] overlapWeights = new int[height, width];
		List<int[]> vectors = new List<int[]> ();
		
		int itt = 0;
		do {
			setWeights (overlapWeights, rooms, padding);
			for (int r = 0; r < rooms.Count; r++) {
				int[,] overlap = new int[2, 2];
				//[[NW,NE],
				// [SW,SE]]
				int vectorNE;
				int vectorSE;
				int vectorE;
				int vectorS;
				float halfWidth = rooms [r].width >> 1;
				float halfHeight = rooms [r].height >> 1;
				for (int x = 0; x < rooms [r].width; x++) {
					for (int y = 0; y < rooms [r].height; y++) {
						if ((float)x != halfWidth && (float)y != halfHeight
						   && y + rooms [r].y > 0 && x + rooms [r].x > 0
						   && y + rooms [r].y < height && x + rooms [r].x < width)
							overlap [((y < halfHeight) ? 0 : 1), ((x < halfWidth) ? 0 : 1)] += overlapWeights [y + rooms [r].y, x + rooms [r].x] - 1;
					}
				}
				vectorNE = (overlap [1, 0] == overlap [0, 1]) ? 0 : (int)(Mathf.Max (Mathf.Round (Mathf.Sqrt ((float)overlap [1, 0]) - Mathf.Sqrt ((float)overlap [0, 1])), ((overlap [1, 0] - overlap [0, 1] > 0) ? 1 : -1)));
				vectorSE = (overlap [0, 0] == overlap [1, 1]) ? 0 : (int)(Mathf.Max (Mathf.Round (Mathf.Sqrt ((float)overlap [0, 0]) - Mathf.Sqrt ((float)overlap [1, 1])), ((overlap [0, 0] - overlap [1, 1] > 0) ? 1 : -1)));
				vectorE = (int)Mathf.Round (0.7f * (vectorSE + vectorNE));
				vectorS = (int)Mathf.Round (0.7f * (vectorSE - vectorNE));
				if (r >= vectors.Count) {
					vectors.Add (new int[]{ vectorE, vectorS });
				} else {
					vectors [r] = new int[]{ vectorE, vectorS };
				}

			}

			for (int r = 0; r < rooms.Count; r++) {
				rooms [r].x += vectors [r] [0];
				if (rooms [r].x < 0)
					rooms [r].x = 0;
				else if (rooms [r].x + rooms [r].width > width)
					rooms [r].x = width - rooms [r].width;
				
				rooms [r].y += vectors [r] [1];
				if (rooms [r].y < 0)
					rooms [r].y = 0;
				else if (rooms [r].y + rooms [r].height > height)
					rooms [r].y = height - rooms [r].height;
			}
			itt++;
		} while(itt < 10);
		
		for (int r = 0; r < rooms.Count; r++) {
			for (int x = 0; x < rooms [r].width; x++) {
				for (int y = 0; y < rooms [r].height; y++) {
					if (overlapWeights [y + rooms [r].y, x + rooms [r].x] > 1) {
						rooms.RemoveAt (r);
						r--;
						setWeights (overlapWeights, rooms, padding);
						goto Escape;
					}
				}
			}
			Escape:
			rooms [r].construct ();
		}
	}
	
	private static void setWeights(int[,] overlapWeight, List<Room> rooms, int padding){
		for (int y = 0; y < overlapWeight.GetLength (0); y++) {
			for (int x = 0; x < overlapWeight.GetLength (1); x++) {
				overlapWeight [y, x] = 0;
			}
		}
		for (int r = 0; r < rooms.Count; r++) {
			for (int x = 0 - padding; x < rooms [r].width + padding; x++) {
				if (x + rooms [r].x >= 0 && x + rooms [r].x < overlapWeight.GetLength (1)) {
					for (int y = 0 - padding; y < rooms [r].height + padding; y++) {
						if (y + rooms [r].y >= 0 && y + rooms [r].y < overlapWeight.GetLength (0)) {
							overlapWeight [y + rooms [r].y, x + rooms [r].x] += 1;
						}
					}
				}
			}
		}
	}


	//****************************************************************************************
	//Pathing
	//****************************************************************************************

	private static bool contains(List<Point> roomWalls, Point p2) {
		foreach (Point p1 in roomWalls) {
			if(p1.x == p2.x && p1.y == p2.y){
				return true;
			}
		}
		return false;
	}

	private static int distanceBetween(Point a, Point b) {
		int d = Mathf.Abs(a.x-b.x)+Mathf.Abs(a.y-b.y);
		return d;
	}

	private static void SetPaths(int[,] walls, List<Room> rooms, bool outlinePaths) {
		
		var openList = new List<AStar.Node> ();
		var closedList = new List<AStar.Node> ();
		var paths = new List<List<AStar.Node>> ();
		
		AStar.Node[,] nodes = new AStar.Node[walls.GetLength (0), walls.GetLength (1)];
		for (int y = 0; y < walls.GetLength (0); y++) {
			for (int x = 0; x < walls.GetLength (1); x++) {
				if (walls [y, x] == 0) {
					nodes [y, x] = new AStar.Node (x, y, true);
				} else {
					nodes [y, x] = new AStar.Node (x, y, false);
				}
			}
		}
		List<int> connected = new List<int> ();//list of connected rooms
		connected.Add (0);
		List<Point> connectedDoors = new List<Point> ();
		
		//for each door, find closest partner
		for (int i = 1; i < rooms.Count; i++) {
			for (int j = 0; j < rooms [i].roomDoors.Count; j++) {
				if (!contains (connectedDoors, rooms [i].roomDoors [j])) {
					connectedDoors.Add (rooms [i].roomDoors [j]);
					Point startPoint = new Point (rooms [i].roomDoors [j].x, rooms [i].roomDoors [j].y);
					Point endpoint = new Point ();
					int distanceToDoor = -1;

					int saveE = 0;
					for (int e = 0; e < rooms.Count; e++) {
						if (e != i) {//check if not in same room, if is connected, if is not self
							if (connected.Contains (e) || connected.Contains (i)) {//if room is not connected, must connect to a connected room. else, does not matter
								for (int f = 0; f < rooms [e].roomDoors.Count; f++) {
									if (distanceBetween (rooms [i].roomDoors [j], rooms [e].roomDoors [f]) < distanceToDoor
									   || distanceToDoor == -1) {
										endpoint = rooms [e].roomDoors [f];
										saveE = e;
										distanceToDoor = distanceBetween (startPoint, endpoint);
									}
								}
							}
						}
					}
					if (!connected.Contains (i)) {
						connected.Add (i);
					} else if (!connected.Contains (saveE)) {
						connected.Add (saveE);
					}
					connectedDoors.Add (endpoint);
					
					// A* door to closest door
					
					openList.Clear ();
					closedList.Clear ();
					nodes [startPoint.y, startPoint.x].h = distanceBetween (nodes [startPoint.y, startPoint.x], nodes [endpoint.y, endpoint.x]);
					closedList.Add (nodes [startPoint.y, startPoint.x]);
					nodes [startPoint.y, startPoint.x].g = 0;
				
					int g = 0;
					bool done = false;
					int itt = 0;
					do {
						for (int h = 0; h < 4; h++) {
							int localX = closedList [g].x;
							int localY = closedList [g].y;
						
							if (h == 0) {
								localY++;
							} else if (h == 1) {
								localX++;
							} else if (h == 2) {
								localY--;
							} else if (h == 3) {
								localX--;
							}
							//if h exists
							if (localX >= 0 && localY >= 0 && localY < nodes.GetLength (0) && localX < nodes.GetLength (1)) {
								//Console.log(nodes[localY,localX]);
								//if h is not on open or closed list or is impassable
								if (!openList.Contains (nodes [localY, localX]) && !closedList.Contains (nodes [localY, localX]) && nodes [localY, localX].isPassable == true) {
									
									nodes [localY, localX].parent = closedList [g]; // parent current node to new node
									nodes [localY, localX].g = closedList [g].g + 1; // set move cost
									nodes [localY, localX].h = distanceBetween (nodes [localY, localX], nodes [endpoint.y, endpoint.x]);
									nodes [localY, localX].f = nodes [localY, localX].g + nodes [localY, localX].h;
									//level.debugBoard[localY,localX] = (nodes[localY,localX].h+"").charAt((nodes[localY,localX].h+"").GetLength(0)()-1;
									//(nodes[localY,localX].h+"").charAt((nodes[localY,localX].h+"").GetLength(0)()-1
									
									openList.Add (nodes [localY, localX]); // add new node to open list
									
									
									
								} else if (closedList.Contains (nodes [localY, localX])) {
									if (closedList [g].g + 1 < nodes [localY, localX].g) {
										nodes [localY, localX].parent = closedList [g];
										nodes [localY, localX].g = closedList [g].g + 1;
										nodes [localY, localX].f = nodes [localY, localX].g + nodes [localY, localX].h;
									}
								} else if (localY == endpoint.y && localX == endpoint.x) {
									nodes [endpoint.y, endpoint.x].parent = closedList [g];
									
									done = true;
								}
							}
						}
						
						//loop through openList - find lowest f
						AStar.Node lowestF = null;
						
						if (openList.Count > 0) {
							foreach (AStar.Node test in openList) {
								if (lowestF == null || test.f < lowestF.f || (test.f == lowestF.f && walls [test.y, test.x] == 3)) {
									//prefer nodes that have already been used, if two are equal, to reduce parallel paths
									//TODO /don't/ change to random chance with preference toward used nodes, to allow some wide pathing
									// to eliminate all unnecessary pathing, you could loop through all paths, check if borders 3 empty squares, or if borders > 2 other path tiles, soem of which each border > 2 other tiles
									// check diagonally 
									lowestF = test;
								}
							}
						} else {
							done = true;
						}
						
						
						if (!closedList.Contains (lowestF)) {
							closedList.Add (lowestF); //move next node to be checked to closed list
						}
						if (openList.Contains (lowestF)) {
							openList.Remove (lowestF); //and remove from open list
						}

						//loop do again
						g++;
						itt++;
						
					} while(done == false && itt < 400);
					
					//Trace back
					//TODO save paths to arraylist, and use last pos in list instead of currentTile
					if (done == true) {
						paths.Add (new List<AStar.Node> ());
						AStar.Node currentTile = nodes [endpoint.y, endpoint.x].parent;
						if (currentTile != null) {
							while (currentTile.parent != null) {
								paths [paths.Count - 1].Add (currentTile);
								walls [currentTile.y, currentTile.x] = 3;
								currentTile = currentTile.parent;
							}
						}
					}
				}
			}
		}
		if (outlinePaths) {
			//fill in edges of painted area with walls
			foreach (List<AStar.Node> path in paths) {
				foreach (AStar.Node node in path) {
					for (int h = 0; h < 8; h++) {
						int localX = node.x;
						int localY = node.y;
						if (h == 0) {
							localY++;
						} else if (h == 1) {
							localX++;
						} else if (h == 2) {
							localY--;
						} else if (h == 3) {
							localX--;
						} else if (h == 4) {
							localY++;
							localX--;
						} else if (h == 5) {
							localY++;
							localX++;
						} else if (h == 6) {
							localY--;
							localX--;
						} else if (h == 7) {
							localY--;
							localX++;
						}
						if (localX >= 0 && localY >= 0 && localY < nodes.GetLength (0) && localX < nodes.GetLength (1)) {
							if (walls [localY, localX] == 0) {
								walls [localY, localX] = 1;
							}
						}
					}
				}
			}
		}
		foreach (Point door in rooms[0].roomDoors) {
			if (walls [door.y, door.x + 1] == 0) {
				//TODO remove door
				walls [door.y, door.x] = 1;
				continue;
			} else if (walls [door.y, door.x - 1] == 0) {
				//TODO remove door
				walls [door.y, door.x] = 1;
				continue;
			} else if (walls [door.y + 1, door.x] == 0) {
				//TODO remove door
				walls [door.y, door.x] = 1;
				continue;
			} else if (walls [door.y - 1, door.x] == 0) {
				//TODO remove door
				walls [door.y, door.x] = 1;
				continue;
			}
		}
	}
}