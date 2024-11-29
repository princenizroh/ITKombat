using UnityEngine;

namespace ITKombat
{
    [System.Serializable]
    public struct MusicTrack
    {
        public string trackName;
        public AudioClip clip;
    }

    [CreateAssetMenu(fileName = "NewMusicLibrary", menuName = "Audio/MusicLibrary")]
    public class NewMusicLibrary : ScriptableObject
    {
        public MusicTrack[] tracks;
    
        public AudioClip GetClipFromName(string trackName)
        {
            foreach (var track in tracks)
            {
                if (track.trackName == trackName)
                {
                    return track.clip;
                }
            }
            return null;
        }
    }
}
