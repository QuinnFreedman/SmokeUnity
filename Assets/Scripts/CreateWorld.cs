using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class CreateWorld : MonoBehaviour {

	public int worldWidth = 100;
	public int worldHeight = 100;
	public int numberOfRooms = 10;
	//public int tileWidthPixels = 32;
	//public int tileHeightPixels = 32;
	//public float tileWidthUnits = 1.0f;
	//public float tileHeightUnits = 1.0f;
	public int tileSheetWidth = 5;
	public int tileSheetHeight = 2;

	private List<KeyValuePair<Vector3, Vector3>> gizmos = new List<KeyValuePair<Vector3, Vector3>>();
	void OnDrawGizmos() {
		foreach(KeyValuePair<Vector3, Vector3> pair in gizmos) {
			Gizmos.DrawLine (pair.Key, pair.Value);
		}
	}


	void Start () {

		Application.targetFrameRate = 60;

		var rooms = new List<Room> ();
		int[,] dungeon = DungeonBuilder.BuildDungeon (worldWidth, worldHeight, numberOfRooms, rooms);
		/*var debug = "";
		for (int y = 0; y < dungeon.GetLength (0); y++) {
			for (int x = 0; x < dungeon.GetLength (1); x++) {
				debug += dungeon [y, x] + " ";
			}
			debug += "\n";
		}
		print(debug);*/

		BuildMesh(dungeon);

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
		var vertIndecies = new int[2, worldHeight + 1, worldWidth + 1];

		int x, y, iVertCount = 0;
		for (x = 0; x < worldWidth; x++) {
			for (y = 0; y < worldHeight; y++) {
				int z = level [y, x] <= 1 ? 0 : 1;
				vertices[iVertCount + 0] = new Vector3(x, y, z);
				vertices[iVertCount + 1] = new Vector3(x + 1, y, z);
				vertices[iVertCount + 2] = new Vector3(x + 1, y + 1, z);
				vertices[iVertCount + 3] = new Vector3(x, y + 1, z);
				vertIndecies[z, y, x] 	= iVertCount + 0;
				vertIndecies[z, y, x + 1] = iVertCount + 1;
				vertIndecies[z, y + 1, x + 1] = iVertCount + 2;
				vertIndecies[z, y + 1, x] = iVertCount + 3;
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
					triangles2Builder.Add (vertIndecies [0, y, x]);
					triangles2Builder.Add (vertIndecies [0, y, x + 1]);
					triangles2Builder.Add (vertIndecies [1, y, x]);

					triangles2Builder.Add (vertIndecies [0, y, x + 1]);
					triangles2Builder.Add (vertIndecies [1, y, x + 1]);
					triangles2Builder.Add (vertIndecies [1, y, x]);

				} else if ((level [y, x] > 1 && level [y - 1, x] <= 1)) {
					triangles2Builder.Add (vertIndecies [0, y, x]);
					triangles2Builder.Add (vertIndecies [1, y, x]);
					triangles2Builder.Add (vertIndecies [1, y, x + 1]);

					triangles2Builder.Add (vertIndecies [0, y, x + 1]);
					triangles2Builder.Add (vertIndecies [0, y, x]);
					triangles2Builder.Add (vertIndecies [1, y, x + 1]);
				}

				if (level [y, x] <= 1 && level [y, x - 1] > 1) {
					triangles2Builder.Add (vertIndecies [0, y + 1, x]);
					triangles2Builder.Add (vertIndecies [0, y, x]);
					triangles2Builder.Add (vertIndecies [1, y, x]);

					triangles2Builder.Add (vertIndecies [0, y + 1, x]);
					triangles2Builder.Add (vertIndecies [1, y, x]);
					triangles2Builder.Add (vertIndecies [1, y + 1, x]);

				} else if (level [y, x] > 1 && level [y, x - 1] <= 1) {
					triangles2Builder.Add (vertIndecies [0, y, x]);
					triangles2Builder.Add (vertIndecies [0, y + 1, x]);
					triangles2Builder.Add (vertIndecies [1, y, x]);

					triangles2Builder.Add (vertIndecies [0, y + 1, x]);
					triangles2Builder.Add (vertIndecies [1, y + 1, x]);
					triangles2Builder.Add (vertIndecies [1, y, x]);
				}
			}
		}

		for (int i = 0; i < numVerts; i++) {
			normals [i] = new Vector3 (0, 0, -1);
		}

		for (y = 1; y < worldHeight - 1; y++) {
			for (x = 1; x < worldWidth - 1; x++) {
				if (level [y, x] > 1) {
					if (level [y - 1, x] <= 1) {
						if (level [y, x - 1] <= 1) {
							normals [vertIndecies [0, y, x]] = new Vector3 (1, 1, -1);
							gizmos.Add (new KeyValuePair<Vector3, Vector3> (new Vector3 (x, y, 0), new Vector3 (x + 1, y + 1, -1)));
						} else {
							normals [vertIndecies [0, y, x]] = new Vector3 (0, 1, -1);
							gizmos.Add (new KeyValuePair<Vector3, Vector3> (new Vector3 (x, y, 0), new Vector3 (x, y + 1, -1)));
						}
					}
				}
			}
		}

		var triangles2 = triangles2Builder.ToArray ();

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		meshFilter.mesh = mesh;
		mesh.subMeshCount = 2;
		mesh.SetTriangles (triangles2, 1);
		mesh.normals = normals;

		iVertCount = 0;
		for (x = 0; x < worldWidth; x++) {
			for (y = 0; y < worldHeight; y++) {
				var tile = level[y,x];
				int tileX, tileY;
				if (tile == 0) {
					tileX = 0;
					tileY = 0;
				} else if (tile == 1) {
					if (Random.value < 0.5f) {
						tileX = 4;
						tileY = 0;
					} else {
						tileX = 0;
						tileY = 1;
					}
				} else {
					if (Random.value < 0.5f) {
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

		meshFilter.mesh.uv = UVArray;
	}



}
