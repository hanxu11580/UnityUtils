using UnityEditor;
using UnityEngine;

public class IcoSphereWizard : ScriptableWizard {
	
	[MenuItem("Assets/Create/Ico Sphere")]
	private static void CreateWizard () {
		ScriptableWizard.DisplayWizard<IcoSphereWizard>("Create Ico Sphere");
	}

    [MenuItem("Assets/Create/Base Sphere")]
    private static void CreateBaseSphere() {
        Mesh mesh = CreateBaseMesh();
        MeshUtility.Optimize(mesh);
        AssetDatabase.CreateAsset(mesh, $"Assets/base.asset");
        Selection.activeObject = mesh;
    }
	
	[Range(1, 8)]
	public int level = 1;
	public float radius = 1f;
	
	private void OnWizardCreate () {
        Mesh mesh = IcoSphereCreator.Create(level, radius);

        string path = EditorUtility.SaveFilePanelInProject("Save Ico Sphere", "IcoSphere", "asset", "Specify where to save the mesh.");
        if (path.Length > 0) {
            MeshUtility.Optimize(mesh);
            AssetDatabase.CreateAsset(mesh, path);
            Selection.activeObject = mesh;
        }
    }

    public static Mesh CreateBaseMesh() {
		Vector3[] vertices = new Vector3[24];
        int[] triangles = new int[24];

        vertices[0] = new Vector3(0, 1, 0);   //the triangle vertical to (1,1,1)
        vertices[1] = new Vector3(0, 0, 1);
        vertices[2] = new Vector3(1, 0, 0);
        // 1
        vertices[3] = new Vector3(0, -1, 0);  //to (1,-1,1)
        vertices[4] = new Vector3(1, 0, 0);
        vertices[5] = new Vector3(0, 0, 1);
        // 2
        vertices[6] = new Vector3(0, 1, 0);   //to (-1,1,1)
        vertices[7] = new Vector3(-1, 0, 0);
        vertices[8] = new Vector3(0, 0, 1);
        // 3
        vertices[9] = new Vector3(0, -1, 0);  //to (-1,-1,1)
        vertices[10] = new Vector3(0, 0, 1);
        vertices[11] = new Vector3(-1, 0, 0);

        // 前4个点尖锐凸向z轴

        // 4
        vertices[12] = new Vector3(0, 1, 0);  //to (1,1,-1)
        vertices[13] = new Vector3(1, 0, 0);
        vertices[14] = new Vector3(0, 0, -1);
        // 5
        vertices[15] = new Vector3(0, 1, 0); //to (-1,1,-1)
        vertices[16] = new Vector3(0, 0, -1);
        vertices[17] = new Vector3(-1, 0, 0);
        // 6
        vertices[18] = new Vector3(0, -1, 0); //to (-1,-1,-1)
        vertices[19] = new Vector3(-1, 0, 0);
        vertices[20] = new Vector3(0, 0, -1);
        // 7
        vertices[21] = new Vector3(0, -1, 0);  //to (1,-1,-1)
        vertices[22] = new Vector3(0, 0, -1);
        vertices[23] = new Vector3(1, 0, 0);

        // 后4个点尖锐凸向z轴

        for (int i = 0; i < 24; i++) {
            triangles[i] = i;
        }

        Mesh mesh = new Mesh();
        mesh.name = "BaseSphere";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uv;
        mesh.RecalculateNormals();
        //CreateTangents(mesh);
        return mesh;
    }

	public static void CreateBaseUV() {

    }

	public static void CreateBaseTangents() {

    }
}