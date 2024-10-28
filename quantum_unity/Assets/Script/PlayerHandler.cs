using Cinemachine;
using Quantum;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] EntityView entityView;

    public void OnEntityInstantiated()
    {
        QuantumGame game = QuantumRunner.Default.Game;

        Frame frame = game.Frames.Verified;

        if (frame.TryGet(entityView.EntityRef, out PlayerLink playerLink))
        {
            if (game.PlayerIsLocal(playerLink.Player))
            {
                CinemachineVirtualCamera virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
                virtualCamera.m_Follow = transform;
            }
        }
    }
}