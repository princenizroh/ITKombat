
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreElementManager : MonoBehaviour
{

    public TMP_Text usernameText;
    public TMP_Text levelText;
    public TMP_Text ktmText;
    public TMP_Text danusText;

    public void SetScoreElement(string _username, int _level, int _ktm, int _danus)
    {
        usernameText.text = _username;
        levelText.text = _level.ToString();
        ktmText.text = _ktm.ToString();
        danusText.text = _danus.ToString();
    }
}
