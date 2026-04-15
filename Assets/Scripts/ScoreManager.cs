using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI nowScoreUI;
    public TextMeshProUGUI bestScoreUI;
    public int nowScore;
    public int bestScore;

    private void Start()
    {
       bestScore = PlayerPrefs.GetInt("bestScore");
       bestScoreUI.text = "Best Score : " + bestScore;
    }
}
