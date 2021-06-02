using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToLoad : MonoBehaviour {

	void Awake()
    {
        if (GameController.Instance == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
