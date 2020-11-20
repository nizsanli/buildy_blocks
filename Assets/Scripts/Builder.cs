using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Builder : MonoBehaviour {

	float TRANSLATION_DAMPENER = 0.008f;
	float TRANSLATION_SPEED = 2f;

	public Transform activeBlock;

	ConfigurableJoint joint;

	float currentRopeHeight = 0f;
	float targetRopeHeight = 0f;

	[Range (0.01f, 10f)]
	public float xScale;
	[Range (0.01f, 10f)]
	public float yScale;
	[Range (0.01f, 10f)]
	public float zScale;

	public float currXScale;
	public float currYScale;
	public float currZScale;

	float tip = 3f;
	
	float anchorHeight;
	public float targetAnchorHeight;

	public Transform crane;
	Vector3 craneAnchor;

	float craneOffset = 0.35f;

	public Text heightText;

	float SCROLL_DAMPENER = 0.5f;

	public Transform blockPre;

	int countBlocks = 1;

	public Transform cranePivot;

	public LineRenderer rope;

	public Transform craneShaft;

	Vector3 ropeStart;
	Vector3 ropeEnd;

	// ----

	Vector3 lastPos; 
	Vector3 interpPos;
	Vector3 targetPos;

	// ----

	// Use this for initialization
	void Start () {
		currXScale = xScale;
		currYScale = yScale;
		currZScale = zScale;


		lastPos = Vector3.zero;
		interpPos = transform.position;
		targetPos = transform.position;

		joint = GetComponent<ConfigurableJoint>();

		//craneAnchor = transform.position + transform.forward*2.5f + transform.right*2.5f;
		anchorHeight = targetAnchorHeight;
	}

	void UpdateRopeLine()
	{
		ropeStart = activeBlock.position + activeBlock.up*activeBlock.localScale.y*0.5f;
		ropeEnd = cranePivot.position - cranePivot.right*cranePivot.localScale.x*0.25f;

		rope.SetPosition(0, ropeStart);
		rope.SetPosition(1, ropeEnd);
	}

	void UpdateCrane()
	{
		crane.rotation = Quaternion.Lerp(crane.rotation, Quaternion.LookRotation(craneAnchor - interpPos), Time.deltaTime * TRANSLATION_SPEED);
		crane.position = interpPos + Vector3.up*craneOffset + crane.rotation*(Vector3.forward*crane.localScale.z*0.49f);
	}

	void LockJoint()
	{
		joint.anchor = Vector3.zero;
		joint.connectedAnchor = Vector3.up * (activeBlock.localScale.y*0.5f + currentRopeHeight);
	}

	void LockCraneJoint()
	{
		anchorHeight = Mathf.Lerp(anchorHeight, targetAnchorHeight, Time.deltaTime * TRANSLATION_SPEED);
		joint.anchor = Vector3.up*anchorHeight;
	}
	
	// Update is called once per frame
	void Update () {
		targetAnchorHeight += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 40f;
		LockCraneJoint();

		if (Input.GetMouseButtonDown(0))
		{
			lastPos = Input.mousePosition;
		}

		if (Input.GetMouseButton(0))
		{
			Vector3 delta = Input.mousePosition - lastPos;
			lastPos = Input.mousePosition;

			Quaternion dragSpace = Quaternion.Euler(0f, Camera.main.transform.rotation.y, 0f);
			targetPos += ((dragSpace * Vector3.right)*delta.x + (dragSpace * Vector3.forward)*delta.y) * TRANSLATION_DAMPENER;
		}

		if (Input.GetMouseButtonUp(0))
		{

		}

		currXScale = Mathf.Lerp(currXScale, xScale, Time.deltaTime*TRANSLATION_SPEED);
		currYScale = Mathf.Lerp(currYScale, yScale, Time.deltaTime*TRANSLATION_SPEED);
		currZScale = Mathf.Lerp(currZScale, zScale, Time.deltaTime*TRANSLATION_SPEED);
		if (Vector3.Distance(activeBlock.localScale, new Vector3(currXScale, currYScale, currZScale)) <= 0.001f)
		{
			activeBlock.GetComponent<Rigidbody> ().detectCollisions = true;
        }

		if (Input.GetKeyDown(KeyCode.Space) && activeBlock.GetComponent<Rigidbody>().detectCollisions == true)
		{
			PlaceBlock();
		}

		activeBlock.localScale = new Vector3(currXScale, currYScale, currZScale);

		//ClampTargetPos();
		UpdateCamPos();
		UpdateRopeLine();
		UpdateCraneShaft();
	}

	public void PlaceBlock()
	{
		activeBlock.GetComponent<Block>().Disconnect();

		activeBlock = (Transform) Instantiate(blockPre, activeBlock.position, Quaternion.identity);
		activeBlock.GetComponent<ConfigurableJoint>().connectedBody = cranePivot.GetComponent<Rigidbody>();
		activeBlock.GetComponent<Rigidbody> ().detectCollisions = false;

		targetAnchorHeight += yScale;

		currXScale = 0f;
		currYScale = 0f;
		currZScale = 0f;
	}

	void UpdateCraneShaft()
	{
		craneShaft.localScale = new Vector3(craneShaft.localScale.x, anchorHeight + 0.5f, craneShaft.localScale.z);
		craneShaft.position = new Vector3(craneShaft.position.x, (anchorHeight+0.5f)*0.5f, craneShaft.position.z);
	}

	void UpdateCamPos()
	{
		Vector3 newCamPos = transform.position + Vector3.up*(anchorHeight) - Vector3.forward*10f;
		Vector3 doubleInterpPos = Vector3.Lerp(Camera.main.transform.position, newCamPos, Time.deltaTime*TRANSLATION_SPEED*0.95f);
		Camera.main.transform.position = doubleInterpPos;
	}

	void FixedUpdate()
	{
		interpPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * TRANSLATION_SPEED);
		GetComponent<Rigidbody>().MovePosition(interpPos);
	}
}
