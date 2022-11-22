using UnityEngine;
using System.Collections;

public class DelayedDestroyer : MonoBehaviour
{
    #region Inspector

    public float delay = 0f;

    #endregion

    // Use this for initialization
    private void Start ()
    {
        StartCoroutine(DelayedDestroy());
	}

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(delay);

        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
