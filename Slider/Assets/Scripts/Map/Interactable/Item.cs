using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour 
{

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D myCollider;
    public bool canKeep = false;

    // animation
    
    [SerializeField] private float pickUpDuration;
    [SerializeField] private AnimationCurve xPickUpMotion;
    [SerializeField] private AnimationCurve yPickUpMotion;



    public void Awake() 
    {
       
    }

    public virtual void PickUpItem(Transform pickLocation, System.Action callback=null) // pickLocation may be moving
    {
        StartCoroutine(AnimatePickUp(pickLocation, callback));
    }

    public virtual STile DropItem(Vector3 dropLocation, System.Action callback=null) 
    {
        StartCoroutine(AnimateDrop(dropLocation, callback));
        Collider2D hit = Physics2D.OverlapPoint(dropLocation, LayerMask.GetMask("Slider"));
        if (hit == null || hit.GetComponent<STile>() == null)
        {
            gameObject.transform.parent = null;
            //Debug.LogWarning("Player isn't on top of a slider!");
            return null;
        }

        STile hitTile = hit.GetComponent<STile>();
        gameObject.transform.parent = hitTile.transform.Find("Objects").transform;
        return hitTile;
 
    }

    public virtual void OnEquip()
    {
        Player.setMoveSpeedMultiplier(1f);
    }


    protected IEnumerator AnimatePickUp(Transform target, System.Action callback=null)
    {
        float t = 0;

        Vector3 start = new Vector3(transform.position.x, transform.position.y);
        // Debug.Log(myCollider);
        // Debug.Log(spriteRenderer);
        while (t < pickUpDuration)
        {
            float x = xPickUpMotion.Evaluate(t / pickUpDuration);
            float y = yPickUpMotion.Evaluate(t / pickUpDuration);
            Vector3 pos = new Vector3(Mathf.Lerp(start.x, target.transform.position.x, x),
                                      Mathf.Lerp(start.y, target.transform.position.y, y));
            
            spriteRenderer.transform.position = pos;

            yield return null;
            t += Time.deltaTime;
        }

        transform.position = target.position;
        spriteRenderer.transform.position = target.position;
        myCollider.enabled = false;
        callback();
    }

    protected IEnumerator AnimateDrop(Vector3 target, System.Action callback = null)
    {
        float t = pickUpDuration;

        Vector3 start = new Vector3(transform.position.x, transform.position.y);
        transform.position = target;
        myCollider.enabled = true;

        while (t >= 0)
        {
            float x = xPickUpMotion.Evaluate(t / pickUpDuration);
            float y = yPickUpMotion.Evaluate(t / pickUpDuration);
            Vector3 pos = new Vector3(Mathf.Lerp(target.x, start.x, x),
                                      Mathf.Lerp(target.y, start.y, y));
            
            spriteRenderer.transform.position = pos;

            yield return null;
            t -= Time.deltaTime;
        }

        transform.position = target;
        spriteRenderer.transform.position = target;
        callback();

    }
    public virtual void dropCallback()
    {

    }
}
