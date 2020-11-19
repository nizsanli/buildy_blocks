using UnityEngine;
using System.Collections;

public class DrawGrid : MonoBehaviour {

	public LineRenderer linePre;
	LineRenderer[] hLines;
	LineRenderer[] vLines;

	public float resolution;

	// Use this for initialization
	void Start () {
		hLines = new LineRenderer[(int)resolution];
		vLines = new LineRenderer[(int)resolution];

		for (int z = 1; z < resolution; z++)
		{
			Vector3 pos = new Vector3(-transform.localScale.x*0.5f, 0.001f, -transform.localScale.y*0.5f + (z/resolution)*transform.localScale.y);
			LineRenderer line = (LineRenderer) Instantiate(linePre, pos, Quaternion.identity);
			line.material = (Material) Resources.Load("Materials/Line");
			line.SetPosition(0, Vector3.zero);
			line.SetPosition(1, Vector3.right*transform.localScale.y);
			hLines[z] = line;
			line.transform.parent = transform;
		}

		for (int x = 1; x < resolution; x++)
		{
			Vector3 pos = new Vector3(-transform.localScale.x*0.5f + (x/resolution)*transform.localScale.x, 0.001f, -transform.localScale.y*0.5f);
			LineRenderer line = (LineRenderer) Instantiate(linePre, pos, Quaternion.identity);
			line.material = (Material) Resources.Load("Materials/Line");
			line.SetPosition(0, Vector3.zero);
			line.SetPosition(1, Vector3.forward*transform.localScale.y);
			vLines[x] = line;
			line.transform.parent = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
