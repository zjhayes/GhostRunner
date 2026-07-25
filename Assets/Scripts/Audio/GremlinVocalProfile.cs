using UnityEngine;

[CreateAssetMenu(
    fileName = "GremlinVocalProfile",
    menuName = "Audio/Gremlin Vocal Profile")]
public class GremlinVocalProfile : ScriptableObject
{
    [Tooltip("Played when an unalerted gremlin enters chase or scatter.")]
    [SerializeField] private RandomAudioCue regular;

    [Tooltip("Played when the gremlin becomes alert and starts running.")]
    [SerializeField] private RandomAudioCue aggressive;

    [Tooltip("Played when the gremlin becomes frightened.")]
    [SerializeField] private RandomAudioCue frightened;

    public RandomAudioCue Regular => regular;
    public RandomAudioCue Aggressive => aggressive;
    public RandomAudioCue Frightened => frightened;
}
