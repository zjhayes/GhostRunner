using UnityEngine;

public class AnimationGroup : MonoBehaviour
{
    [SerializeField] FlipbookAnimation south;
    [SerializeField] FlipbookAnimation east;
    [SerializeField] FlipbookAnimation north;
    [SerializeField] FlipbookAnimation west;

    public FlipbookAnimation GetAnimation(Cardinal direction)
    {
        if(direction == Cardinal.South)
        {
            return south;
        }
        else if(direction == Cardinal.East)
        {
            return east;
        }
        else if(direction == Cardinal.North)
        {
            return north;
        }
        else if(direction == Cardinal.West)
        {
            return west;
        }
        else
        {
            Debug.LogError("Animation group didn't receive a direction.");
            return south;
        }
    }



}
