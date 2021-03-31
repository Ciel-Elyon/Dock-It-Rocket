using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    private Vector3 scrollSpeed = new Vector3(0f, 1f, 0f);

    private void FixedUpdate()
    {
        transform.position += scrollSpeed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
        else if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
