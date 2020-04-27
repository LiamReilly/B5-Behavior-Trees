using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class IKTestScript : MonoBehaviour
{
    public GameObject Bear1;
    public GameObject Door;
    public GameObject DoorKnob;
    public GameObject ball;
    [Header("IK Stuff")]
    public FullBodyBipedEffector hand;
    public InteractionObject doorIO;
    public InteractionObject ikBall;
    public Animator doorAnim;
    public GameObject DialogueText;



    private BehaviorAgent behaviorAgent;
    //private BehaviorAgent behaviorAgent2;
    // Start is called before the first frame update
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
        // doorIO = Door.GetComponent<InteractionObject>();
        // print(doorIO.GetTarget(hand, "").ToString());
        // doorAnim = Door.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    protected Node BuildTreeRoot()
    {
        return new DecoratorLoop(
                        new Sequence(
                        TestArc()
                        ));

    }
    #region TestArc
    protected Node TestArc()
    {
        return new Sequence(
            
            PickUp(Bear1),
            new LeafWait(1000),
            PutDown(Bear1),
            new LeafWait(1000)
        );
    }
    #endregion

    #region Affordances
    //add this shit later boys
    protected Node OpenDoor(Animator anim, GameObject p)
    {
        return new Sequence(
            p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, doorIO),
            new LeafWait(100),
            new LeafInvoke(() =>
            {
                anim.SetTrigger("OpenDoor");
            }),
            new LeafWait(800),
            p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand)
        );
    }

    protected Node CloseDoor(Animator anim, GameObject p)
    {
        return new Sequence(
            p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, doorIO),
            new LeafWait(100),
            new LeafInvoke(() =>
            {
                anim.SetTrigger("CloseDoor");
            }),
            new LeafWait(800),
            p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand)
        );
    }
    protected Node Dialogue(string speech)
    {
        return new Sequence(
            new LeafInvoke(() => DialogueText.GetComponent<Text>().text = speech),
            new LeafInvoke(() => DialogueText.SetActive(true)),
            new LeafWait(5000),
            new LeafInvoke(() => DialogueText.SetActive(false))
            );
    }

    protected Node seduce(GameObject newdude, GameObject jipped)
    {

        return new Sequence(
            jipped.GetComponent<BehaviorMecanim>().Node_HandAnimation("SURPRISED", true),
            newdude.GetComponent<BehaviorMecanim>().Node_HandAnimation("WAVE", true),
            new LeafWait(10000),
            jipped.GetComponent<BehaviorMecanim>().Node_HandAnimation("SURPRISED", false)

        );
    }

    #endregion


    #region IK related function
    protected Node PickUp(GameObject p)
    {
        return new Sequence(this.Node_BallStop(),
                            p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                            new LeafWait(1000),
                            p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
    }

    public Node Node_BallStop()
    {
        return new LeafInvoke(() => this.BallStop());
    }
    public virtual RunStatus BallStop()
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;

        return RunStatus.Success;
    }

    protected Node PutDown(GameObject p)
    {
        return new Sequence(p.GetComponent<BehaviorMecanim>().Node_StartInteraction(hand, ikBall),
                            new LeafWait(300),
                            this.Node_BallMotion(),
                            new LeafWait(500), p.GetComponent<BehaviorMecanim>().Node_StopInteraction(hand));
    }

    public Node Node_BallMotion()
    {
        return new LeafInvoke(() => this.BallMotion());
    }

    public virtual RunStatus BallMotion()
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        ball.transform.parent = null;
        return RunStatus.Success;
    }
    #endregion

}
