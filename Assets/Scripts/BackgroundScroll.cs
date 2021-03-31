using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    Transform[] children;
    Rigidbody2D player;
    Vector3 earlierpos;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] copy = GetComponentsInChildren<Transform>();
        children = new Transform[copy.Length - 1];
        int ind = 0;
        foreach(Transform trans in copy)
        {
            if(trans != this.transform)
            {
                children[ind] = trans;
                ind++;
            }
        }
        player = FindObjectOfType<Player>().gameObject.GetComponent<Rigidbody2D>();
        earlierpos = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Transform trans in children)
        {
            if(Mathf.Abs((player.transform.position - earlierpos).magnitude) > 0.02f)
            {
                Vector3 vel = player.velocity;
                vel.x = 0;
                vel.y *= 0.3f;
                trans.position -= vel * Time.deltaTime;

                if (trans.localPosition.y < -40)
                {
                    trans.localPosition = new Vector3(0, 20);
                }
                else if (trans.localPosition.y > 20)
                {
                    trans.localPosition = new Vector3(0, -40);
                }
            }
        }
        earlierpos = player.transform.position;
    }
}
