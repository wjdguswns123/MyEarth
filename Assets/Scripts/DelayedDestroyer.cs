using UnityEngine;
using System.Collections;

public class DelayedDestroyer : MonoBehaviour
{
    public float delay = 0f;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(DelayedDestroy());
	}

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(delay);

        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
