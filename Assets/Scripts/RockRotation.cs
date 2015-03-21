using UnityEngine;
using System.Collections;

public class RockRotation : MonoBehaviour {
    
    public float minRotationSpeed = -5.0f;
    public float maxRotationSpeed = 5.0f;
    float rotationSpeed;

	// Use this for initialization
	void Start () {
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 temp = this.transform.localRotation.eulerAngles;
        temp.z += rotationSpeed * Time.deltaTime;
        Quaternion tempQ = Quaternion.Euler(temp);
        this.transform.localRotation = tempQ;
	}
}
