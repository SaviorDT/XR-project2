using Unity.XR.CoreUtils;
using UnityEngine;

public class PlayerMotionManager : MonoBehaviour
{
    [SerializeField] private SimpleController _playerMotionController;
    [SerializeField] private Transform _playerDelta;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GameCore.OnGameStart += HandleGameStart;
        GameCore.OnFinalSceneStart += HandleFinalSceneStart;
        GameCore.OnReturnMainScene += HandleReturnMainScene;
    }
    void OnDisable()
    {
        GameCore.OnGameStart -= HandleGameStart;
        GameCore.OnFinalSceneStart -= HandleFinalSceneStart;
        GameCore.OnReturnMainScene -= HandleReturnMainScene;
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
        _playerMotionController.MoveTo(new Vector3(-30.4039993f, 1.329f, 25.5470009f), 0.5f);
        _playerMotionController.RotateTo(new Vector3(0, 90, 0), 0.1f);
    }
    private void HandleFinalSceneStart()
    {
        _playerMotionController.MoveTo(new Vector3(-28.4389992f, 1.329f, 25.5470009f), 0.5f);
        _playerMotionController.RotateTo(new Vector3(0, -90, 0), 0.1f);
    }
    private void HandleReturnMainScene()
    {
        _playerMotionController.MoveTo(new Vector3(-35.0099983f, 1.329f, 25.5470009f), 0.5f);
        _playerMotionController.RotateTo(new Vector3(0, 90, 0), 0.1f);
    }
}
