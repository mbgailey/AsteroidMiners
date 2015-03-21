using UnityEngine;
using System.Collections;

public class WaterBehavior : MonoBehaviour {
    //Attach to an object with a rigidBody2D that you want to interact with water. Will limit velocity

    public float maximumSpeed = 5.0f;
    bool submerged = false;

	// Use this for initialization
	void FixedUpdate () {
        //Debug.Log("Speed " + Vector3.Magnitude(rigidbody2D.velocity));
        
        if (submerged)
        {

            float speed = Vector3.Magnitude(rigidbody2D.velocity);  // test current object speed
            
            //if (speed > maximumSpeed)
            //{
            //    float brakeSpeed = speed - maximumSpeed;  // calculate the speed decrease
            //    //Debug.Log("BrakeSpeed " + brakeSpeed);
            //    Vector3 normalizedVelocity = rigidbody2D.velocity.normalized;
            //    Vector3 brakeVelocity = normalizedVelocity * brakeSpeed * 2.0f;  // make the brake Vector3 value

            //    rigidbody2D.AddForce(-brakeVelocity);  // apply opposing brake force

            //    //Debug.Log("velocity " + rigidbody2D.velocity);
            //    //Debug.Log("brakeVelocity " + brakeVelocity);
                
            //}

            if (speed > maximumSpeed)
            {
                
                Vector3 normalizedVelocity = rigidbody2D.velocity.normalized;
                
                rigidbody2D.velocity = normalizedVelocity * maximumSpeed;  // limit velocity

                //Debug.Log("velocity " + rigidbody2D.velocity);
                //Debug.Log("brakeVelocity " + brakeVelocity);

            }

        }
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.CompareTag("Water"))
        {
            Debug.Log("SUBMERGED");
            submerged = true;
        }
	}

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            submerged = false;
        }
    }
}
