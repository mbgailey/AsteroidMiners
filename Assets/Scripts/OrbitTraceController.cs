using UnityEngine;
using System.Collections;

public class OrbitTraceController : MonoBehaviour {

    LineRenderer stepTrace;
    LineRenderer orbitTrace;
    GameObject endMarker;
    Vector3 origin = new Vector2(0.0f, 0.0f);

    float orbitRadius = 50.0f;   //Orbit radius in world units
    float orbitStepDegrees = 30.0f; //Degrees to show that planet will move each turn
    float currentAngle = 60.0f;

    int numPointsPerDegree = 5;

    //Vector3[] points;



	// Use this for initialization
	void Start () {
        orbitTrace = this.transform.FindChild("OrbitTrace").GetComponent<LineRenderer>();
        stepTrace = this.transform.FindChild("StepTrace").GetComponent<LineRenderer>();
        endMarker = this.transform.FindChild("EndMarker").gameObject;

        Vector3 vect = this.transform.position - origin;
        orbitRadius = vect.magnitude;
        currentAngle = Mathf.Atan2(vect.y,vect.x) * Mathf.Rad2Deg;

        CreateOrbitTrace();
        CreateStepTrace();
	}
	
	void CreateStepTrace () {
	    int size = numPointsPerDegree * (int) orbitStepDegrees;
        stepTrace.SetVertexCount(size);
        //points = new Vector3[size];
        float angle = currentAngle;
        float degreesPerPt = orbitStepDegrees / (float)size;
        Vector3 pt = new Vector3();
        for (int i = 0; i < size; i++)
        {
            angle += degreesPerPt;
            pt = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0.0f) * orbitRadius;
            //points[i] = pt;
            stepTrace.SetPosition(i, pt);
        }
        endMarker.transform.position = pt;
    }

    void CreateOrbitTrace()
    {
        int size = numPointsPerDegree * (int)360;
        orbitTrace.SetVertexCount(size);
        //points = new Vector3[size];
        float angle = 0.0f;
        float degreesPerPt = 360.0f / (float)size;
        for (int i = 0; i < size; i++)
        {
            angle += degreesPerPt;
            Vector3 pt = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0.0f) * orbitRadius;
            pt.z = 1.0f;     //Set full orbit trace back farther from camera than the step trace;
            //points[i] = pt;
            orbitTrace.SetPosition(i, pt);
        }
    }
}
