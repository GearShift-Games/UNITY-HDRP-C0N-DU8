using Barmetler.RoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class multiRoadTest : MonoBehaviour
{
    //test
    public GameObject road;

    public Transform[] MainPath;
    public Transform[] InPath;
    public Transform[] OutPath;

    public Transform[] CombinedPath;

    public int ChangeTrack = 0;

    // Start is called before the first frame update
    void Start()
    {
        /*Transform[][] combiner = new Transform[this.gameObject.transform.childCount][];

        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            Debug.Log(i);
            combiner[0][i] = this.gameObject.transform.GetChild(i);
        }

        Debug.Log(combiner);*/
        ChangeTrack = Random.Range(0, 2);

        if (ChangeTrack == 0)
        {
            CombinedPath = MainPath.Concat(InPath).ToArray();
        }
        else if (ChangeTrack == 1)
        {
            CombinedPath = MainPath.Concat(OutPath).ToArray();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
