using UnityEngine;

public class StartButtonScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Debug.Log("StartGame method called in StartButtonScript.");
        GameObject mainObject = GameObject.Find("Main");
        if (mainObject != null)
        {
            Main main = mainObject.GetComponent<Main>();
            if (main != null)
            {
                main.StartGame(new TestTempoWithoutAudio());
            }
            else
            {
                Debug.LogError("Main component not found on Main GameObject.");
            }
        }
        else
        {
            Debug.LogError("Main GameObject not found in the scene.");
        }
    }
}
