using UnityEngine;

public class SpawnCoin : MonoBehaviour
{
    [SerializeField] private Transform targetObjTransform;
    [SerializeField] private float speed;

    void Update()
    {
        var step =  speed * Time.deltaTime; 
        MoveTowardsTarget(transform.position,targetObjTransform.position,step);

        
        if (Vector3.Distance(transform.position, targetObjTransform.position) < 0.001f)
        {
           Destroy(this.gameObject);
        }
    }

    private void MoveTowardsTarget(Vector3 curPos,Vector3 targetPos,float maxDistanceDelta)
    {
        transform.position = Vector3.MoveTowards(curPos, targetPos, maxDistanceDelta);
    }
}
