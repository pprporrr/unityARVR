using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heartSpin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotate
        this.transform.Rotate(new Vector3(0f, 0.15f, 0f), Space.World);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            collision.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }
}
