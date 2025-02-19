using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class multiRoadTest : MonoBehaviour
{
    //test
    public GameObject road;

    

    


    // Start is called before the first frame update
    void Start()
    {
        Transform[][] combiner = new Transform[this.gameObject.transform.childCount][];

        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            Debug.Log(i);
            combiner[0][i] = this.gameObject.transform.GetChild(i);
        }

        Debug.Log(combiner);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
