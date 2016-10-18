using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[ExecuteInEditMode]
public class CreateWorld : MonoBehaviour {

	public static int[,] level;

	public int worldWidth = 100;
	public int worldHeight = 100;
	public int numberOfRooms = 10;
	//public int tileWidthPixels = 32;
	//public int tileHeightPixels = 32;
	//public float tileWidthUnits = 1.0f;
	//public float tileHeightUnits = 1.0f;
	public int tileSheetWidth = 5;
	public int tileSheetHeight = 2;
	public float edgeNormalsZ = -1f;

	private List<KeyValuePair<Vector3, Vector3>> gizmos = new List<KeyValuePair<Vector3, Vector3>>();
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		foreach(KeyValuePair<Vector3, Vector3> pair in gizmos) {
			Gizmos.DrawLine (pair.Key, pair.Value);
		}
	}

	void Start () {

        int seed = 0;//DateTime.Now.GetHashCode();
        UnityEngine.Random.InitState(seed);
        Debug.Log("random seed is " + seed);

		var rooms = new List<Room> ();
		level = DungeonBuilder.BuildDungeon (worldWidth, worldHeight, numberOfRooms, rooms);

		BuildMesh(level);
		AStar.InitGraph(level);

		float spawnX = rooms [0].x + (rooms [0].width / 2f);
		float spawnY = rooms [0].y + (rooms [0].height / 2f);

		GameObject.Find ("player").transform.position = new Vector3 (spawnX, spawnY, 0);
		GameObject.Find ("CameraContainer").transform.position = new Vector3 (spawnX, spawnY, 0);

	}

	public void BuildMesh(int[,] level) {
		var meshFilter 	 = GetComponent<MeshFilter> ();
		//var meshRenderer = GetComponent<MeshRenderer> ();
		var collider = GetComponent<PolygonCollider2D> ();

		int numTiles = worldWidth * worldHeight;
		int numTriangles = numTiles * 6;
		int numVerts = numTiles * 4;

		var vertices = new Vector3[numVerts];
		var triangles = new int[numTriangles];
		var UVArray = new Vector2[numVerts];
		var normals = new Vector3[numVerts];
		var vertIndecies = new List<int>[2, worldHeight + 1, worldWidth + 1];
		int x, y;
		int iVertCount = 0;
		for (int z = 0; z < vertIndecies.GetLength (0); z++) {
			for (y = 0; y < vertIndecies.GetLength (1); y++) {
				for (x = 0; x < vertIndecies.GetLength (2); x++) {
					vertIndecies [z, y, x] = new List<int> ();
				}
			}
		}
		 
		for (x = 0; x < worldWidth; x++) {
			for (y = 0; y < worldHeight; y++) {
				int z = level [y, x] <= 1 ? 0 : 1;
				vertices[iVertCount + 0] = new Vector3(x, y, z);
				vertices[iVertCount + 1] = new Vector3(x + 1, y, z);
				vertices[iVertCount + 2] = new Vector3(x + 1, y + 1, z);
				vertices[iVertCount + 3] = new Vector3(x, y + 1, z);
				vertIndecies[z, y, x].Add(iVertCount + 0);
				vertIndecies[z, y, x + 1].Add(iVertCount + 1);
				vertIndecies[z, y + 1, x + 1].Add(iVertCount + 2);
				vertIndecies[z, y + 1, x].Add(iVertCount + 3);
				iVertCount += 4;
			}
		}

		int iIndexCount = 0; iVertCount = 0;
		for (int i = 0; i < numTiles; i++) {

			triangles[iIndexCount + 0] += (iVertCount + 0);
			triangles[iIndexCount + 1] += (iVertCount + 2);
			triangles[iIndexCount + 2] += (iVertCount + 1);
			triangles[iIndexCount + 3] += (iVertCount + 0);
			triangles[iIndexCount + 4] += (iVertCount + 3);
			triangles[iIndexCount + 5] += (iVertCount + 2);
			
			iVertCount += 4; iIndexCount += 6;
		}

		var triangles2Builder = new List<int> ();
		for(y = 1; y < worldHeight; y++) {
			for(x = 1; x < worldWidth; x++) {
				if (level [y, x] <= 1 && level [y - 1, x] > 1) {
					triangles2Builder.Add (vertIndecies [0, y, x][0]);
					triangles2Builder.Add (vertIndecies [0, y, x + 1][0]);
					triangles2Builder.Add (vertIndecies [1, y, x][0]);

					triangles2Builder.Add (vertIndecies [0, y, x + 1][0]);
					triangles2Builder.Add (vertIndecies [1, y, x + 1][0]);
					triangles2Builder.Add (vertIndecies [1, y, x][0]);

				} else if ((level [y, x] > 1 && level [y - 1, x] <= 1)) {
					triangles2Builder.Add (vertIndecies [0, y, x][0]);
					triangles2Builder.Add (vertIndecies [1, y, x][0]);
					triangles2Builder.Add (vertIndecies [1, y, x + 1][0]);

					triangles2Builder.Add (vertIndecies [0, y, x + 1][0]);
					triangles2Builder.Add (vertIndecies [0, y, x][0]);
					triangles2Builder.Add (vertIndecies [1, y, x + 1][0]);
				}

				if (level [y, x] <= 1 && level [y, x - 1] > 1) {
					triangles2Builder.Add (vertIndecies [0, y + 1, x][0]);
					triangles2Builder.Add (vertIndecies [0, y, x][0]);
					triangles2Builder.Add (vertIndecies [1, y, x][0]);

					triangles2Builder.Add (vertIndecies [0, y + 1, x][0]);
					triangles2Builder.Add (vertIndecies [1, y, x][0]);
					triangles2Builder.Add (vertIndecies [1, y + 1, x][0]);

				} else if (level [y, x] > 1 && level [y, x - 1] <= 1) {
					triangles2Builder.Add (vertIndecies [0, y, x][0]);
					triangles2Builder.Add (vertIndecies [0, y + 1, x][0]);
					triangles2Builder.Add (vertIndecies [1, y, x][0]);

					triangles2Builder.Add (vertIndecies [0, y + 1, x][0]);
					triangles2Builder.Add (vertIndecies [1, y + 1, x][0]);
					triangles2Builder.Add (vertIndecies [1, y, x][0]);
				}
			}
		}

		for (int i = 0; i < numVerts; i++) {
			normals [i] = new Vector3 (0, 0, -1);
		}

		for (y = 1; y < worldHeight - 1; y++) {
			for (x = 1; x < worldWidth - 1; x++) {
				var upperLeft = level [y, x - 1] <= 1;
				var upperRight = level [y, x] <= 1;
				var lowerLeft = level [y - 1, x - 1] <= 1;
				var lowerRight = level [y - 1, x] <= 1;

				if (!(True (upperLeft, upperRight, lowerLeft, lowerRight) || False (upperLeft, upperRight, lowerLeft, lowerRight))) {
					
					Vector3 normal = new Vector3 (0, 0, 0);
					//print (upperLeft + " " + upperRight + "\n" + lowerLeft + " " + lowerRight);
					float z = edgeNormalsZ;

					if (True (upperRight, lowerRight) && False (upperLeft, lowerLeft)) {
						//W
						normal = new Vector3 (-1, 0, z);
					} else if (False (upperRight, lowerRight) && True (upperLeft, lowerLeft)) {
						//E
						normal = new Vector3 (1, 0, z);
					} else if (True (upperLeft, upperRight) && False (lowerLeft, lowerRight)) {
						//S
						normal = new Vector3 (0, -1, z);
					} else if (False (upperLeft, upperRight) && True (lowerLeft, lowerRight)) {
						//N
						normal = new Vector3 (0, 1, z);
					} else if (lowerRight && False (lowerLeft, upperLeft, upperRight) || True (upperRight, lowerLeft) && !upperLeft) {
						//NW
						normal = new Vector3 (-1, 1, z);
					} else if (lowerLeft && False (upperLeft, upperRight, lowerRight) || True (upperLeft, lowerRight) && !upperRight) {
						//NE
						normal = new Vector3 (1, 1, z);
					} else if (upperLeft && False (lowerLeft, upperRight, lowerRight) || True (upperRight, lowerLeft) && !lowerRight) {
						//SE
						normal = new Vector3 (1, -1, z);
					} else if (upperRight && False (lowerLeft, upperLeft, lowerRight) || True (upperLeft, lowerRight) && !lowerLeft) {
						//SW
						normal = new Vector3 (-1, -1, z);
					}

					if (normal.z == 0) {
						print ("something went wrong");
					}
					//print (vertIndecies [0, y, x].Count);
					foreach (var index in vertIndecies[0, y, x]) {
						normals [index] = normal;
						gizmos.Add (new KeyValuePair<Vector3, Vector3> (new Vector3 (x, y, 0), new Vector3 (x, y, 0) + normal));
					}
				}
			}
		}

		var triangles2 = triangles2Builder.ToArray();

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		meshFilter.sharedMesh = mesh;
		mesh.subMeshCount = 2;
		mesh.SetTriangles (triangles2, 1);
		mesh.normals = normals;

		collider.pathCount = 0;
		iVertCount = 0;
		for (x = 0; x < worldWidth; x++) {
			for (y = 0; y < worldHeight; y++) {
				var tile = level[y,x];
				int tileX, tileY;
				if (tile == 0) {
					tileX = 0;
					tileY = 0;
				} else if (tile == 1) {
					if (UnityEngine.Random.value < 0.5f) {
						tileX = 4;
						tileY = 0;
					} else {
						tileX = 0;
						tileY = 1;
					}
				} else {
					if (UnityEngine.Random.value < 0.5f) {
						tileX = 2;
						tileY = 1;
					} else {
						tileX = 2;
						tileY = 0;
					}
				}

				UVArray [iVertCount + 0] = new Vector2 ((float)tileX / tileSheetWidth, (float)tileY / tileSheetHeight); //Top left of tile in atlas
				UVArray [iVertCount + 1] = new Vector2 ((float)(tileX + 1) / tileSheetWidth, (float)tileY / tileSheetHeight); //Top right of tile in atlas
				UVArray [iVertCount + 2] = new Vector2 ((float)(tileX + 1) / tileSheetWidth, (float)(tileY + 1) / tileSheetHeight); //Bottom right of tile in atlas
				UVArray [iVertCount + 3] = new Vector2 ((float)tileX / tileSheetWidth, (float)(tileY + 1) / tileSheetHeight); //Bottom left of tile in atlas
				iVertCount += 4;


				if (tile == 1) {
					var path = new Vector2[4];
					path [0] = new Vector2 (x, y);
					path [1] = new Vector2 (x + 1, y);
					path [2] = new Vector2 (x + 1, y + 1);
					path [3] = new Vector2 (x, y + 1);
					collider.pathCount += 1;
					collider.SetPath (collider.pathCount - 1, path);
				}
			}
		}

		meshFilter.sharedMesh.uv = UVArray;
	}

	private static bool True(params bool[] args) {
		foreach (var arg in args) {
			if (!arg)
				return false;
		}
		return true;
	}

	private static bool False(params bool[] args) {
		foreach (var arg in args) {
			if (arg)
				return false;
		}
		return true;
	}

}
