using UnityEngine;

namespace Scripts.Player.Platformer
{
    public interface IPlayerEventsListener
    {
        void OnJump();
        void OnFall();
        void OnGround();
        void OnVelocity(Vector3 currentVelocity);
    }
}