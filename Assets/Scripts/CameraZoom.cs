using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour {

	Camera cam;
	public float defaultSize;
	public Vector3 defaultLocation;
	public float moveSpeed;
	public float zoomStep;
	public float zoomSpeed;
	public float maxZoom;
	public float minZoom;
	public bool allowZoom = true;
    public bool allowPan = true;
	public float targetSize;
	public bool touchZoom = true;
    public bool touchPan = true;
	public float touchZoomSpeed;        // The rate of change of the orthographic size in orthographic mode.
    public float touchPanSpeed;

	public bool objectZoomOn;
	public GameObject zoomTarget;
	public float autoZoomSpeed;
	public float autoMaxZoom;
	public float autoMinZoom;

    Vector2 panStartPos;
    Vector2 panVect;

	public bool objectFollowOn;
	public GameObject followTarget;

	// Use this for initialization
	void Start () {
		objectZoomOn = false;
		objectFollowOn = false;

		cam = this.GetComponent<Camera> ();
		defaultSize = 250.0f;
		cam.orthographicSize = defaultSize;
		defaultLocation = cam.transform.position;	//Make default location the starting position of the camera
		moveSpeed = 40.0f;
		zoomStep = 0.1f;	//Percent to increase zoom with each mouse step
		zoomSpeed = 1.1f;
		maxZoom = 800.0f;
		minZoom = 5.0f;
		targetSize = cam.orthographicSize;
		touchZoomSpeed = 0.05f;
        touchPanSpeed = 0.05f;
		autoZoomSpeed = 1.1f;
		autoMaxZoom = 800.0f;
		autoMinZoom = 5.0f;

        

        StartCoroutine("ResetZoom");
	}
	
	// Update is called once per frame
	void Update () {
		//If target zoom is on
		if (objectZoomOn && zoomTarget != null) {
			ZoomToObject();
		}
		else if (objectZoomOn && zoomTarget == null){
			objectZoomOn = false;
			StartCoroutine("ResetZoom");
		}
		//If target follow is on
		if (objectFollowOn && followTarget != null) {
			FollowObject();
		}
		else if (objectFollowOn && followTarget == null){

			objectFollowOn = false;
			//StartCoroutine("ResetCameraPosition");
		}

		if (allowZoom && Input.GetAxis ("Mouse ScrollWheel") != 0.0f) {
			float scroll = Input.GetAxis ("Mouse ScrollWheel");
			StartCoroutine("ScrollZoom",scroll);
		}

		if (allowZoom && touchZoom)
		{
			// If there are two touches on the device...
			if (Input.touchCount == 2)
			{
				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);
				
				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
				
				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
				
				// Find the difference in the distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

				// ... change the orthographic size based on the change in distance between the touches.
				camera.orthographicSize += deltaMagnitudeDiff * touchZoomSpeed;
				
				// Make sure the orthographic size never goes below min or above max.
				camera.orthographicSize = Mathf.Max(minZoom, Mathf.Min (maxZoom, camera.orthographicSize));
				
			}
		}

        if (allowPan && touchPan)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        panStartPos = touch.position;
                        break;

                    case TouchPhase.Moved:
                        panVect = -(touch.position - panStartPos).normalized;
                        Vector3 newPos = cam.transform.position + (Vector3)panVect * touchPanSpeed * cam.orthographicSize;  //Scale movement by speed and current size. Need to move more when zoomed out farther
                        cam.transform.position = newPos;
                        panStartPos = touch.position;
                        break;
                    case TouchPhase.Ended:
                        
                        break;
                }
            }
        }
	}

	IEnumerator ScrollZoom (float dir) 
	{

		if (dir < 0.0f) 		//Zoom in (target is larger size)
		{
			targetSize = targetSize * (1.0f + zoomStep);
			while (cam.orthographicSize < targetSize) 
			{
				float size = Mathf.Lerp (cam.orthographicSize, targetSize, Time.smoothDeltaTime * zoomSpeed);
				size = Mathf.Max (minZoom, Mathf.Min (maxZoom, size));
				cam.orthographicSize = size;
				yield return null;
			}
		}
		else 		//Zoom out (target is smaller size)
		{
			targetSize = targetSize * (1.0f - zoomStep);
			while (cam.orthographicSize > targetSize) 
			{
				float size = Mathf.Lerp (cam.orthographicSize, targetSize, Time.smoothDeltaTime * zoomSpeed);
				size = Mathf.Max (minZoom, Mathf.Min (maxZoom, size));
				cam.orthographicSize = size;
				yield return null;
			}
		}
		cam.orthographicSize = targetSize;
		yield return null;
	}

	void ZoomToObject () {
		Vector2 objScreenPos = cam.WorldToViewportPoint (zoomTarget.transform.position);

		//float vertExtent = cam.orthographicSize * Screen.height /2f;    
		//float horzExtent = vertExtent * Screen.width / Screen.height/2f;

		//Zoom out if screen position is greater than 95% of either x or y bound
		if (Mathf.Abs(objScreenPos.y) > 0.90f || Mathf.Abs(objScreenPos.y) < 0.10f || Mathf.Abs(objScreenPos.x) > 0.90f || Mathf.Abs(objScreenPos.x) < 0.10f) {
			targetSize = cam.orthographicSize * (1.0f + 0.05f);
			float size = Mathf.Lerp (cam.orthographicSize, targetSize, Time.smoothDeltaTime * zoomSpeed * 8.0f);
			size = Mathf.Max (autoMinZoom, Mathf.Min (autoMaxZoom, size));
			cam.orthographicSize = size;
			//cam.orthographicSize += cam.orthographicSize* autoZoomSpeed * Time.smoothDeltaTime;	
		}

		//Otherwise zoom in if screen position is less than 90% of either x or y bound
		else if ((Mathf.Abs(objScreenPos.y) < 0.80f && Mathf.Abs(objScreenPos.y) > 0.15f) || (Mathf.Abs(objScreenPos.x) < 0.85f && Mathf.Abs(objScreenPos.x) > 0.15f)) {
			targetSize = cam.orthographicSize * (1.0f - 0.05f);
			float size = Mathf.Lerp (cam.orthographicSize, targetSize, Time.smoothDeltaTime * zoomSpeed * 5.0f);
			size = Mathf.Max (autoMinZoom, Mathf.Min (autoMaxZoom, size));
			cam.orthographicSize = size;
			//cam.orthographicSize += cam.orthographicSize* autoZoomSpeed * Time.smoothDeltaTime;	
		}

	}

	IEnumerator ResetZoom () 
	{
		//Debug.Log ("reset zoom");

		while (Mathf.Abs(cam.orthographicSize - defaultSize) > 0.01) 
		{
			float size = Mathf.Lerp (cam.orthographicSize, defaultSize, Time.smoothDeltaTime * zoomSpeed * 2.0f);
			size = Mathf.Max (minZoom, Mathf.Min (maxZoom, size));
			cam.orthographicSize = size;
			yield return null;
		}

		cam.orthographicSize = defaultSize;
		yield return null;
	}

	void FollowObject () 
	{

		Vector2 targetPos = (Vector2)followTarget.transform.position;
		float distanceRemaining = ((Vector2)cam.transform.position - targetPos).sqrMagnitude;	//Use square magnitude instead of magnitude because its cheaper
		//Debug.Log ("distanceRemaining " + distanceRemaining);
		if (distanceRemaining > 0.01) 
		{
			Vector2 newPos = Vector2.MoveTowards(cam.transform.position, targetPos, moveSpeed * Time.smoothDeltaTime);
			cam.transform.position = new Vector3(newPos.x, newPos.y, defaultLocation.z);
	
		}
		else {
			cam.transform.position = new Vector3(targetPos.x, targetPos.y, defaultLocation.z);
		}

	}

	IEnumerator ResetCameraPosition () 
	{
		//Debug.Log ("reset zoom");
		float distanceRemaining = ((Vector2)cam.transform.position - (Vector2)defaultLocation).sqrMagnitude;

		while (distanceRemaining > 0.01) 
		{
			Vector2 newPos = Vector2.MoveTowards(cam.transform.position, defaultLocation, moveSpeed * Time.smoothDeltaTime);
			cam.transform.position = new Vector3(newPos.x, newPos.y, defaultLocation.z);
			yield return null;
		}
		
		cam.transform.position = defaultLocation;
		yield return null;
	}

	IEnumerator MaxZoom () 
	{

		while (Mathf.Abs(cam.orthographicSize - maxZoom) > 0.2) 
		{
			float size = Mathf.Lerp (cam.orthographicSize, maxZoom, Time.smoothDeltaTime * zoomSpeed * 3.0f);
			size = Mathf.Max (minZoom, Mathf.Min (maxZoom, size));
			cam.orthographicSize = size;
			yield return null;
		}
		
		cam.orthographicSize = maxZoom;
		yield return null;
	}

}
