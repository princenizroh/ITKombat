using UnityEngine;

public class RoundTracker : MonoBehaviour
{
    private const int MAX_ROUNDS = 4;      
    private const int ADVANTAGE_REQUIRED_TO_WIN = 2;     

    public enum Side
    {
        Left = 0,   
        Right = 1   
    }

    static public RoundTracker Instance { get; private set; }

    int _leftWins, _rightWins;

    void Awake() => Reset();
    void OnEnable() => Instance = this;
    void OnDisable() => Instance = null;
    void OnDestroy() => OnDisable();

    public void Reset() { _leftWins = _rightWins = 0; }
    public void RecordRoundWinner(Side winner)
      => setWins(winner, getWins(winner) + 1);
    public int CurrentWinsOf(Side side) => getWins(side);

    public int TotalRounds => _leftWins + _rightWins; 
    public int CurrentAdvantage => Mathf.Abs(_leftWins - _rightWins); 

    public bool FinalRound()
      => TotalRounds == MAX_ROUNDS ||
         CurrentAdvantage == ADVANTAGE_REQUIRED_TO_WIN;

    public Side? CurrentWinner
    {
        get
        {
            if (_leftWins > _rightWins) return Side.Left;
            else if (_rightWins > _leftWins) return Side.Right;
            return null;  
        }
    }

    private int getWins(Side side)
      => side == Side.Right ? _rightWins : _leftWins;

    private void setWins(Side side, int value)
    {
        if (side == Side.Right) _rightWins = value;
        else _leftWins = value;
    }
}
