using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Player.Platformer;


public class PlayerInitializer : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private PlayerProperties _playerProperties;

    [SerializeField]
    private int _id;

    [SerializeField]
    private Vector3 _startPosition;

    public void InitPlayerDefaultInScene()
    {
        _player.Inject(_playerProperties, _id, _startPosition);
        _player.ConnectController();
    }
    
    public void InitPlayerInSpecificPlace(Vector3 startPosition)
    {
        _player.Inject(_playerProperties, _id, startPosition);
        _player.ConnectController();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
