using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;
public class ChangeValue : MonoBehaviour
{
    public TextMeshProUGUI textVersion;
    public double increment;
    public ChangeableGameValues value;
    public void ChangeValueInGameManager()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.ChangeValues(value, increment, textVersion);
    }
}
