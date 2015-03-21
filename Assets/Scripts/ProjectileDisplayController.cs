using UnityEngine;
using System.Collections;

public class ProjectileDisplayController : MonoBehaviour {

    //public ParticleSystem trail;
    public TrailRenderer trail;
    public ParticleSystem steerParticles;
    public ParticleSystem impactParticles;
    public ParticleSystem speedHalo;

    float speedScaleMin = 5.0f;
    float speedScaleMax = 100.0f;

    float minRed = 94f;
    float minGreen = 94f;
    float maxRed = 242f;
    float maxGreen = 242f;

	// Use this for initialization
	void Start () {
        trail.renderer.enabled = false;
        speedHalo.enableEmission = false;
	}
	
	// Update is called once per frame
	public void StartTrail () {
        trail.renderer.enabled = true;
        InvokeRepeating("SetHaloColor", 0.0f, 0.1f);
	}

    void SetHaloColor()
    {
    //As speed changes, change halo color
        float speed = this.rigidbody2D.velocity.magnitude;
        Color col = speedHalo.startColor;

        if (speed < speedScaleMin)
        {
            col.r = minRed;
            col.g = minGreen;
        }
        else if (speed > speedScaleMax)
        {
            col.r = maxRed;
            col.g = maxGreen;
        }
        else
        {
            float scaleFactor = (speed - speedScaleMin) / (speedScaleMax - speedScaleMin);
            col.r = minRed + (maxRed - minRed) * scaleFactor;
            col.g = minGreen + (maxGreen - minGreen) * scaleFactor;
        }

        speedHalo.startColor = col;
        speedHalo.Emit(1); 
    }

    public void EmitSteerParticles(Vector3 direction)
    {
        steerParticles.transform.rotation = Quaternion.LookRotation(direction);
       
        steerParticles.Emit(10);
    }

}
