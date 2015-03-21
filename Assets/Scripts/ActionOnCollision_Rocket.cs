using UnityEngine;
using System.Collections;
using System;

//[AddComponentMenu("Destructible 2D/D2D Action On Collision")]
public class ActionOnCollision_Rocket : MonoBehaviour
{
/// <summary>
/// This class defines how the projectile will behave upon colliding with something /// </summary>
	public Boolean bombProjectile = true;
	public int bombCount = 3;
	public float bombForce = 50.0f;
	public float bombSpreadAngle = 120.0f;
	public GameObject bombPrefab;
	[HideInInspector] public Vector3 gravityCenter;
	
	PhaseManager phaseManager;
	//[HideInInspector] public StampFunction stampFunction;
	[HideInInspector] public ExplosionDamage explosionDamage;
	ProjectileRegistry projectileRegistry;
	public Boolean masterProjectile = true;

	public GameObject[] explosionPrefabs = new GameObject[4] ;
    GameObject explosion;
    
	public GameObject impactPrefab;
	public AudioClip impactSound;

	public Texture2D StampTex;
	public LayerMask Layers = -1;
	public Vector2 Size = Vector2.one;
	public float Angle;
	public float Hardness = 1.0f;

	Vector3 lastPos;
	Vector3 currentPos;
	Vector3 velVect;
    float velMagnitude;

    //Variable damage scales
    bool variableMagnitude = true;
    float blastRadius;
    float minRadius = 4.0f;
    float maxRadius = 10.0f;
    float velocityDamage;
    float minDamage = 0.0f;
    float maxDamage = 50.0f;

    //Velocity scale. Anything below min gets min damage and radius. Anything above max gets max damage and radius. Linear scale in between.
    float minVelocityScale = 5.0f;
    float maxVelocityScale = 100.0f;
    float velocityScale;

	Vector3 gravityDir;

    public bool isCollidable = false;
	float timeAlive = 0.0f;
	public float maxTime = 20.0f;

	void Awake()
	{
		projectileRegistry = GameObject.Find ("GameManager").GetComponent<ProjectileRegistry>();

        if (!masterProjectile) {
            projectileRegistry.AddToRegistry (this.gameObject);
        }
	}

	void Start()
	{
		//projectileRegistry.AddToRegistry (this.gameObject);
		//phaseManager = GameObject.Find ("GameManager").GetComponent<PhaseManager>();
		////phaseManager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PhaseManager>();
		//stampFunction = this.gameObject.GetComponent<StampFunction> ();
		explosionDamage = this.gameObject.GetComponent<ExplosionDamage> ();
		//gravityCenter = GameObject.Find ("Terrain").transform.position;
		////lastPos = this.transform.position;

        explosion = explosionPrefabs[1]; //Set default explosion
        velocityScale = maxVelocityScale - minVelocityScale;
	}

	protected virtual void Update()
	{
		timeAlive += Time.deltaTime;

		if (timeAlive >= maxTime) {
			//SelfDestruct ();  //Disable timed destruct for now
		}

//		currentPos = this.transform.position;
//		velVect = currentPos - lastPos;
//		velVect = velVect.normalized;
//		lastPos = currentPos;

		velVect = this.rigidbody2D.velocity;
        //velocity = velVect.magnitude;
		velVect = velVect.normalized;

	}

	protected virtual void OnCollisionEnter2D(Collision2D coll)
	{

        Debug.Log("Collided with " + coll.gameObject.name);

        velMagnitude = coll.relativeVelocity.magnitude;
        SetBlastMagnitude(velMagnitude);

        if (bombProjectile) {
			ReleaseBombs();
			Detonate();
		}
		else {
			Detonate();
		}

//		if (masterProjectile) {
//			phaseManager.SendMessage ("ProjectileHit",bombCount); //Specify how many extra projectile hit messages will be sent to phase manager
//			Debug.Log ("Sent Hit Message as master Projectile");
//		}
//		else {
//			phaseManager.SendMessage ("ProjectileHit");
//			Debug.Log ("Sent Hit Message only");
//		}
		

	}

    void SetBlastMagnitude(float vel)
    { //Size the blast based on a velocity magnitude
        if (vel < minVelocityScale)
        {
            blastRadius = minRadius;
            velocityDamage = minDamage;

        }
        else if (vel > maxVelocityScale)
        {
            blastRadius = maxRadius;
            velocityDamage = maxDamage;
        }
        else
        {
            float scaleFactor = (vel - minVelocityScale) / velocityScale;
            Debug.Log("scaleFactor " + scaleFactor);
            blastRadius = minRadius + (maxRadius - minRadius) * scaleFactor;
            velocityDamage = minDamage + (maxDamage - minDamage) * scaleFactor;
        }

        Size = new Vector2(blastRadius, blastRadius);

        //Set explosion prefab based on blast radius
        if (blastRadius >= maxRadius)
        {
            explosion = explosionPrefabs[3];
        }
        else if (blastRadius > maxRadius * 0.66f)
        {
            explosion = explosionPrefabs[2];
        }
        else if (blastRadius > maxRadius * 0.33f)
        {
            explosion = explosionPrefabs[1];
        }
        else
        {
            explosion = explosionPrefabs[0];
        }
    }

	void OnTriggerExit2D (Collider2D col) {
		if( col.gameObject.CompareTag("GameBounds")) {
			SelfDestruct();
		}
	}

	public void Detonate()
	{
		PlayImpactSound ();
        explosionDamage.IncurExplosionDamage(blastRadius, velocityDamage);

        Debug.Log("velMagnitude " + velMagnitude);
        Debug.Log("blastRadius " + blastRadius);
        Debug.Log("velocityDamage " + velocityDamage);

        Stamp();

		if (masterProjectile) {
			////phaseManager.ProjectileHit ();
		}
		projectileRegistry.RemoveFromRegistry (this.gameObject);
		Destroy (this.gameObject);
	}

	public void ReleaseBombs()
	{
		for (int i = 1; i <= bombCount; i++) {
			GameObject bomb = GameObject.Instantiate (bombPrefab, this.transform.position, Quaternion.identity) as GameObject;
			Vector3 traj = Quaternion.Euler(0,0, -bombSpreadAngle/2 + bombSpreadAngle/(bombCount-1) * (i-1)) * -velVect;
//			Debug.Log ("traj1 " + traj);
			//Vector3 traj = -velVect;
			traj.z = 0.0f;
			traj = traj.normalized;
			//Vector2 traj2D = Vector2(traj.x,traj.y);
			bomb.rigidbody2D.AddForce(traj * bombForce,ForceMode2D.Impulse);
//			Debug.Log ("BOMB " + i);
//			Debug.Log ("velVect " + velVect);
//			Debug.Log ("traj normalized " + traj);
		}
	}

	protected virtual void SelfDestruct()
	{
		//Projectile self desctructs if it reaches its time limit. Usually happens if it goes into an endless orbit.
		//phaseManager.SendMessage ("ProjectileSelfDestruct");
		if (masterProjectile) {
			////phaseManager.ProjectileHit ();
		}
		projectileRegistry.RemoveFromRegistry (this.gameObject);
		Destroy (this.gameObject);	
		
	}
	
	public virtual void Stamp()
	{
		
		D2D_Helper.CloneGameObject(explosion, null).transform.position = this.transform.position;
		GameObject.Instantiate(impactPrefab, this.transform.position, Quaternion.LookRotation(-velVect));

		D2D_Destructible.StampAll(this.transform.position, Size, Angle, StampTex, Hardness, Layers);

	}

	public void PlayImpactSound()
	{
		AudioSource.PlayClipAtPoint (impactSound, this.transform.position);
	}

}