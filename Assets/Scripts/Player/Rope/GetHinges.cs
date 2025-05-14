using UnityEngine;

public class GetHinges : MonoBehaviour
{
    [SerializeField] private HingeJoint2D firstHingeJoint2D;
    [SerializeField] private HingeJoint2D secondHingeJoint2D;

    public HingeJoint2D GetFirstHingeJoint2D => firstHingeJoint2D;
    public HingeJoint2D GetSecondHingeJoint2D => secondHingeJoint2D;
}
