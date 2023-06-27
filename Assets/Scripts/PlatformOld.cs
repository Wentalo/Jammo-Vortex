using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformOld : MonoBehaviour
{
    public Rigidbody platform; //Para que se pueda mover la persona encima de ella
    public Transform[] platformPositions;
    public float platformSpeed;

    private int actualPosition = 0;
    private int nextPosition = 1;

    public bool moveToTheNext = true;
    public float waitTime;

    public GameObject player;

    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.transform.parent = this.transform;
        }
    }
    

    private void LateUpdate()
    {
        if (moveToTheNext)
        {
            StopCoroutine(WaitForMove(0)); //Ahorro de recursos, no hace falta que se ejecute si no estamos ni en el if
            //platform.MovePosition(Vector3.MoveTowards(platform.position, platformPositions[nextPosition].position, platformSpeed * Time.deltaTime));
            platform.AddForce(new Vector3(0, 1, 0), ForceMode.VelocityChange);

        }

        if (Vector3.Distance(platform.position, platformPositions[nextPosition].position) <= 0)
        {
            StartCoroutine(WaitForMove(waitTime));  //Para durante 5 segundos
            actualPosition = nextPosition;
            nextPosition++;

            if (nextPosition > platformPositions.Length - 1)
            {
                nextPosition = 0;
            }
        }
    }

    //Arreglar, hace stuttering
    private void MovePlatform()
    {
        
    }

    IEnumerator WaitForMove(float time)
    {
        moveToTheNext = false;
        yield return new WaitForSeconds(time);
        moveToTheNext = true;
    }


}
