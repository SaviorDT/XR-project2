using UnityEngine;

public class CameraMotionManager : MonoBehaviour
{
    [SerializeField] private SimpleController _playerMotionController;
    // [SerializeField] private Transform _playerCenter, _playerHead;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GameCore.OnGameStart += HandleGameStart;
        GameCore.OnFinalSceneStart += HandleGameStart;
        GameCore.OnReturnMainScene += HandleGameStart;
    }
    void OnDisable()
    {
        GameCore.OnGameStart -= HandleGameStart;
        GameCore.OnFinalSceneStart -= HandleGameStart;
        GameCore.OnReturnMainScene -= HandleGameStart;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleGameStart()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }
}
