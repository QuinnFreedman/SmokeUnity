using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class CreateWorld : MonoBehaviour {

	public int worldWidth = 100;
	public int worldHeight = 100;
	public int tileWidthPixels = 32;
	public int tileHeightPixels = 32;
	public float tileWidthUnits = 1.0f;
	public float tileHeightUnits = 1.0f;
	public int tileSheetWidth = 5;
	public int tileSheetHeight = 2;

	void Start () {

		Application.targetFrameRate = 60;

		BuildMesh ();
	}

	public void BuildMesh() {
		var meshFilter 	 = GetComponent<MeshFilter> ();
		var meshRenderer = GetComponent<MeshRenderer> ();
		var collider = GetComponent<PolygonCollider2D> ();

		int numTiles = worldWidth * worldHeight;
		int numTriangles = numTiles * 6;
		int numVerts = numTiles * 4;

		Vector3[] vertices = new Vector3[numVerts];
		var UVArray = new Vector2[numVerts];

		int x, y, iVertCount = 0;
		for (x = 0; x < worldWidth; x++) {
			for (y = 0; y < worldHeight; y++) {
				vertices[iVertCount + 0] = new Vector3(x, y, 0);
				vertices[iVertCount + 1] = new Vector3(x + 1, y, 0);
				vertices[iVertCount + 2] = new Vector3(x + 1, y + 1, 0);
				vertices[iVertCount + 3] = new Vector3(x, y + 1, 0);
				iVertCount += 4;
			}
		}

		int[] triangles = new int[numTriangles];

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

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		meshFilter.mesh = mesh;

		iVertCount = 0;
		for (x = 0; x < worldWidth; x++) {
			for (y = 0; y < worldHeight; y++) {
				var tile = Random.Range (0, 2);
				int tileX, tileY;
				if (tile == 0) {
					tileX = 0;
					tileY = 1;
				} else {
					tileX = 3;
					tileY = 1;
				}

				UVArray [iVertCount + 0] = new Vector2 ((float)tileX / tileSheetWidth, (float)tileY / tileSheetHeight); //Top left of tile in atlas
				UVArray [iVertCount + 1] = new Vector2 ((float)(tileX + 1) / tileSheetWidth, (float)tileY / tileSheetHeight); //Top right of tile in atlas
				UVArray [iVertCount + 2] = new Vector2 ((float)(tileX + 1) / tileSheetWidth, (float)(tileY + 1) / tileSheetHeight); //Bottom right of tile in atlas
				UVArray [iVertCount + 3] = new Vector2 ((float)tileX / tileSheetWidth, (float)(tileY + 1) / tileSheetHeight); //Bottom left of tile in atlas
				iVertCount += 4;


				if (tile == 0) {
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
