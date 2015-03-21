using UnityEngine;
using System.Collections;

public class MiningBehavior : MonoBehaviour
{

    SpriteRenderer previewArea;
    PlayerGravity playerGravity;
    public GameObject player;
    Vector3 targetPosition;
    float moveSpeed = 6.0f;

    LayerMask terrainMask;
    public GameObject blastPrefab;
    public Texture2D StampTex;
    float angle = 0f;

    PhaseManager phaseManager;

    float blastSpacing = 1.5f;
    int numBlastRows = 15;
    int numBlastsPerRow = 10;
    float timeBetweenBlasts = 0.02f;
    //float totalBlastTime;

    // Use this for initialization
    void Start()
    {
        //totalBlastTime = timeBetweenBlasts * (float)numBlastRows * (float)numBlastsPerRow;
        terrainMask = 10;
        int terrainLayer = 10;
        terrainMask = 1 << terrainLayer;
        previewArea = this.GetComponent<SpriteRenderer>();
        phaseManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PhaseManager>();
        player = this.transform.root.gameObject;
        playerGravity = player.GetComponent<PlayerGravity> ();
		
    }

    // Update is called once per frame
    public IEnumerator StartMining(float ang)
    {
        angle = ang - 90f;
        previewArea.enabled = false;
        yield return StartCoroutine("MineBlast");

        
        //yield return new WaitForSeconds(totalBlastTime + 1.0f);

        targetPosition = this.transform.position; //Assume this object is co-located with muzzle point when mining starts
        StartCoroutine("MovePlayer");

        yield return new WaitForSeconds(2.0f);

        Destroy(this.gameObject);
        phaseManager.MiningComplete();
    }

    IEnumerator MineBlast()
    {
        float width = (float)numBlastsPerRow * blastSpacing;
        float height = (float)numBlastRows * blastSpacing;
        float x0 = -width / 2f;
        float y0 = height / 2f;

        int r = 0;
        while(r < numBlastRows)
        {
            int c = 0;
            while (c < numBlastsPerRow)
            {
                Vector3 localPos = new Vector3(x0 + c * blastSpacing, y0 - r * blastSpacing);
                Vector3 globalPos = this.transform.TransformPoint(localPos);

                Collider2D coll = Physics2D.OverlapCircle(globalPos, 0.5f, terrainMask);

                if (coll != null)
                {
                    GameObject.Instantiate(blastPrefab, globalPos, Quaternion.identity);
                    //c++;
                    //yield return new WaitForSeconds(timeBetweenBlasts);
                }

                //else
                //{
                //    c++;
                //    yield return null;
                //}
                c++;
                yield return null;
                
            }
            r++;
            yield return null;
        }
    }

    IEnumerator MovePlayer()
    {
        float distRemaining = 1000.0f;
        while (distRemaining > 0.01f) {
            Vector3 newPos = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            player.transform.position = newPos;
            distRemaining = (newPos - targetPosition).sqrMagnitude;
            yield return null;
        }
        playerGravity.EnableGravity();
        yield return null;
    }

    void StampShape()
    {
        
        Vector2 Size = new Vector2(3.0f, 7.5f);
        float Hardness = 1.0f;

        D2D_Destructible.StampAll(this.transform.position, Size, angle, StampTex, Hardness, terrainMask);
    }
}
