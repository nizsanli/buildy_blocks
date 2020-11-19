using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//Builder builder = GameObject.Find("Builder").GetComponent<Builder>();
	}

	void OnCollisionEnter()
	{
		
	}

	public void Disconnect()
	{
		Destroy(GetComponent<ConfigurableJoint>());
	}
}
