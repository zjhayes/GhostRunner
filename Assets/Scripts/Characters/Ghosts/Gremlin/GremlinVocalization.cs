using UnityEngine;

[RequireComponent(typeof(GremlinContext))]
[RequireComponent(typeof(RandomAudioCuePlayer))]
public class GremlinVocalization : MonoBehaviour
{
    [SerializeField] private GremlinContext context;
    [SerializeField] private RandomAudioCuePlayer cuePlayer;
    [SerializeField] private GremlinVocalProfile profile;

    private GhostChase chase;
    private GhostScatter scatter;

    private void Awake()
    {
        if (context == null)
            context = GetComponent<GremlinContext>();

        if (cuePlayer == null)
            cuePlayer = GetComponent<RandomAudioCuePlayer>();

        chase = GetComponent<GhostChase>();
        scatter = GetComponent<GhostScatter>();
    }

    private void OnEnable()
    {
        if (context != null)
            context.OnStateChanged += HandleStateChanged;

        if (chase != null)
            chase.Entered += HandleNormalBehaviourEntered;

        if (scatter != null)
            scatter.Entered += HandleNormalBehaviourEntered;
    }

    private void OnDisable()
    {
        if (context != null)
            context.OnStateChanged -= HandleStateChanged;

        if (chase != null)
            chase.Entered -= HandleNormalBehaviourEntered;

        if (scatter != null)
            scatter.Entered -= HandleNormalBehaviourEntered;
    }

    private void HandleNormalBehaviourEntered(GhostBehaviour behaviour)
    {
        if (context.State == GremlinContext.GremlinState.Walking)
            Play(profile != null ? profile.Regular : null);
    }

    private void HandleStateChanged(
        GremlinContext.GremlinState previousState,
        GremlinContext.GremlinState nextState)
    {
        switch (nextState)
        {
            case GremlinContext.GremlinState.Running:
                Play(profile != null ? profile.Aggressive : null);
                break;

            case GremlinContext.GremlinState.Frightened:
                Play(profile != null ? profile.Frightened : null);
                break;
        }
    }

    private void Play(RandomAudioCue cue)
    {
        if (cuePlayer != null)
            cuePlayer.Play(cue);
    }
}
