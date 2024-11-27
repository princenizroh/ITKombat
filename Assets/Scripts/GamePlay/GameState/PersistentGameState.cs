using UnityEngine;

namespace ITKombat
{
    public enum WinState
    {
        Invalid,
        Draw,
        Player1Win,
        Player2Win,
        Victory,
        Defeat
    }

    public enum RoundState
    {
        Round1,
        Round2,
        Round3,
        Round4,
        FinalRound
    }
    
    public enum State {
        TimeOut,
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }
    public class PersistentGameState
    {
        public WinState WinState { get; set; }
        public RoundState RoundState { get; set; }

        public void SetWinState(WinState winState) // Belum dipakai
        {
            WinState = winState;
        }

        public void SetRoundState(RoundState roundState) // Belum dipakai
        {
            RoundState = roundState;
        }  

        public void Reset() // Belum dipakai
        {
            WinState = WinState.Invalid;
            RoundState = RoundState.Round1;
        }

    }
}
