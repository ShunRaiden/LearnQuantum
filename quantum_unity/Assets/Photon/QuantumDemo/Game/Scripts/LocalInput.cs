using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class LocalInput : MonoBehaviour {
    
  private void OnEnable() {
    QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
  }

  public void PollInput(CallbackPollInput callback) {
    Quantum.Input i = new Quantum.Input();
        i.Jump = UnityEngine.Input.GetButton("Jump");

        Vector2 inputDirection = Vector2.zero;

        inputDirection.x = UnityEngine.Input.GetAxis("Horizontal");
        inputDirection.y = UnityEngine.Input.GetAxis("Vertical");

        i.DirectionX = (short)(inputDirection.x * 10);
        i.DirectionY = (short)(inputDirection.y * 10);
    callback.SetInput(i, DeterministicInputFlags.Repeatable);
  }
}
