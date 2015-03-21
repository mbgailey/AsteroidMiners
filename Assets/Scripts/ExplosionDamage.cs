using UnityEngine;
using System.Collections;

public class ExplosionDamage : MonoBehaviour {

	public float explosionRadius = 40.0f;
	public float baseDamage = 40.0f;
	public float maxForce;
	public bool allowFriendlyFire = true;
	float distanceBuff;

	int playerLayer;
	int oreLayer;
	LayerMask layerMask;

	// Use this for initialization
	void Start () {

		playerLayer = 9;
		oreLayer = 12;
		layerMask = ((1 << playerLayer) | (1 << oreLayer));
		maxForce = 100.0f;
	}
	
	// Update is called once per frame
	public void IncurExplosionDamage (float blastRadius, float velocityBonus) 
	{
        explosionRadius = blastRadius;
        //Collider2D[] hitColliders;
		//hitColliders = new Collider2D[10];
		Collider2D[] hitColliders = Physics2D.OverlapCircleAll(this.transform.position, explosionRadius, layerMask);

		//Debug.Log (hitColliders);

		if (hitColliders.Length > 0) 
		{
			for (int i = 0; i < hitColliders.Length; i++) 
			{
					if (hitColliders [i].CompareTag("Player"))
				    {
						Vector3 offset = this.transform.position - hitColliders [i].transform.position;
						float distance = offset.magnitude;
                        float damage = (baseDamage + velocityBonus) * ((explosionRadius - distance) / explosionRadius);
						damage = Mathf.Max (damage, 0.0f);		//Don't allow damage to be negative
	
						PlayerHealth playerHealth = hitColliders [i].gameObject.GetComponent<PlayerHealth> ();
						playerHealth.TakeDamage (damage);

						PlayerGravity playerGravity = hitColliders [i].gameObject.GetComponent<PlayerGravity> ();
						playerGravity.StartCoroutine("TemporarilyDisableGravity");
						
						Vector2 force = maxForce * ((explosionRadius - distance) / explosionRadius) * offset.normalized;
						force.x = Mathf.Max (force.x, 0.0f);		//Don't allow force to be negative
						force.y = Mathf.Max (force.y, 0.0f);		//Don't allow force to be negative
						
						hitColliders [i].rigidbody2D.AddForce(force,ForceMode2D.Impulse);
					}
					else if (hitColliders [i].CompareTag("Ore")) {
						hitColliders [i].GetComponent <OreBehavior>().CollectOre();
					}
			}
		}
	}
}
