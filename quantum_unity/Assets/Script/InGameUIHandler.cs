using Quantum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUIHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countDownText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Utils.TryGetQuantumFrame(out Frame frame))
        {
            if(frame.TryGetSingletonEntityRef<GameSession>(out var entity) == false)
            {
                countDownText.text = "GameSession singleton not found";
                return;
            }

            var gameSession = frame.GetSingleton<GameSession>();

            int countDown = (int)gameSession.TimeUntilStart;

            switch (gameSession.State)
            {
                case GameState.Countdown:
                    countDownText.text = $"{countDown}";
                    break;
                case GameState.Playing:
                    if (countDown == 0)
                        countDownText.text = "GO!";

                    if (countDown < 0)
                        countDownText.text = "";

                    break;
                case GameState.GameOver:
                    countDownText.text = "Game Over!";
                    break;
            }
        }
    }
}
