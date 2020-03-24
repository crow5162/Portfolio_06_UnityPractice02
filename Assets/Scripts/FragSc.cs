using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragSc : MonoBehaviour
{
    public GameObject expEffect;

    public int playerId = -1;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.ExplosionFrag(0.0f));
    }

    IEnumerator ExplosionFrag(float tm)
    {
        yield return new WaitForSeconds(tm);

        GameObject effectObj = (GameObject)Instantiate(expEffect,
            transform.position,
            Quaternion.identity);

        Destroy(effectObj, 2.0f);
        Destroy(this.gameObject, 1.0f);
    }
}
