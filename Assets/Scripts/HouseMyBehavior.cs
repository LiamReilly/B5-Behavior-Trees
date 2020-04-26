using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;
using UnityEngine.UI;

public class HouseMyBehavior : MonoBehaviour
{
    [Header("Destinations")]
    public Transform Couch;
    public Transform Bear2ByeBye;
    public Transform insideDoor;
    public Transform outsideDoor;
    public Transform Bear1CatchPosition;
    public Transform Bear2CatchPosition;
    public Transform tv;
    [Space]
    [Header("Actors")]
    public GameObject Bear1;
    public GameObject Bear2;
    [Space]
    [Header("Sound")]
    public AudioSource knock;
    public AudioSource lecture;
    [Space]
    [Header("Other")]
    public int UserInput;
    public GameObject Text;
    


    private BehaviorAgent behaviorAgent;
    //private BehaviorAgent behaviorAgent2;
    // Start is called before the first frame update
    void Start()
    {
        behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
        Text.SetActive(false);
        knock = GetComponent<AudioSource>();
        lecture = transform.GetChild(0).GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    protected Node BuildTreeRoot()
    {
        return new DecoratorLoop(
                        new Sequence(
                        WatchingTvArc(),
                        PlayingCatchArc()
                        ));
       
    }
    #region Arcs
    #region TVArc
    protected Node WatchingTvArc()
    {
        Val<Vector3> position = Val.V(() => outsideDoor.position);
        Val<Vector3> position2 = Val.V(() => insideDoor.position);
        Val<Vector3> position3 = Val.V(() => Bear1.transform.position);
        Val<Vector3> position4 = Val.V(() => Bear2.transform.position);
        //Val<Vector3> position5 = Val.V(() => tv.position);
        return new Sequence(
            
            Bear2.GetComponent<BehaviorMecanim>().Node_GoTo(position),
            new LeafInvoke(() => knock.Play()),
            new LeafWait(1000),
            //new LeafInvoke(() => Bear1.GetComponent<CharacterMecanim>().StandUp()),
            Bear1.GetComponent<BehaviorMecanim>().Node_GoTo(position2),
            Bear1.GetComponent<BehaviorMecanim>().Node_OrientTowards(position4),
            Bear2.GetComponent<BehaviorMecanim>().Node_OrientTowards(position3),
            OfferChoice(),
            new LeafInvoke(() => { Text.SetActive(false); }),
            new LeafInvoke(() => print("got to before selector")),
            new Selector(
                Friendship(),
                Loneliness()
                )
            );
    }
    protected Node OfferChoice()
    {

        //Debug.Log(UserInput);
        return new Sequence(
            new LeafInvoke(() => Text.GetComponent<Text>().text = "Do you want to (1)play catch or (2)go back to watching Kappadia's Lecture"),
            new LeafInvoke(() => Text.SetActive(true)),
            GetUserInput()
            );
            
        
       // return new Sequence(new LeafWait(1000));
    }
    protected Node TvSelector()
    {
        return new Selector(
            Friendship(),
            Loneliness()
            );
    }
    protected Node Loneliness()
    {
        
        Val<Vector3> position3 = Val.V(() => tv.position);
        return new Sequence(
            LonelinessCheck(),
            new LeafInvoke(() => print("got to loneliness")),
            LonelinessReturn(),
            Bear1.GetComponent<BehaviorMecanim>().Node_OrientTowards(position3),
            new LeafInvoke(()=> Bear1.GetComponent<CharacterMecanim>().SitDown()),
            new LeafInvoke(() => lecture.Play()),
            Bear2.GetComponent<BehaviorMecanim>().Node_BodyAnimation("H_CowBoy", true),
            new LeafWait(1000000)
            );
        //waiting a 1000 seconds is an ending 
    }
    protected Node Friendship()
    {
        
        // Debug.Log("made it to friendship");
        return new Sequence(
            FriendshipCheck(),
            new LeafInvoke(() => print("got to friendship")),
            WalkToPositions(),
            new LeafWait(1000));
    }
    protected Node WalkToPositions()
    {
        Val<Vector3> position = Val.V(() => Bear1CatchPosition.position);
        Val<Vector3> position2 = Val.V(() => Bear2CatchPosition.position);
        // return new SequenceParallel(
        return new Sequence(
          Bear1.GetComponent<BehaviorMecanim>().Node_GoTo(position),
            Bear2.GetComponent<BehaviorMecanim>().Node_GoTo(position2)
            );
    }
    protected Node LonelinessReturn()
    {
        Val<Vector3> position = Val.V(() => Bear2ByeBye.position);
        Val<Vector3> position2 = Val.V(() => Couch.position);
        // return new SequenceParallel(
        return new Sequence(
             Bear2.GetComponent<BehaviorMecanim>().Node_GoTo(position),
            Bear1.GetComponent<BehaviorMecanim>().Node_GoTo(position2)
            );
    }
    protected Node LonelinessCheck()
    {
        return new LeafAssert(() => UserInput == 2
        );
    }
    protected Node FriendshipCheck()
    {
        return new LeafAssert(() => UserInput == 1
        );
    }
    #endregion
    #region PlayingCatchArc
    protected Node PlayingCatchArc()
    {
        return new Sequence(
            new LeafInvoke(()=> print("made it to Catch Arc")),
            new LeafWait(1000)
            );
    }
    #endregion
    #endregion

    #region UserInput
    protected Node GetUserInput()
    {
        // return new SequenceParallel(
        return new Sequence(
            new DecoratorInvert(
                new DecoratorLoop(
                    new LeafInvoke(() =>
                    {
                        var uinput = -1;
                        if (Input.GetKey("1"))
                        {
                            uinput = 1;
                        }
                        if (Input.GetKey("2"))
                        {
                            uinput = 2;
                        }
                        if (uinput == 1 || uinput == 2)
                        {
                            UserInput = uinput;
                            print(UserInput);
                            return RunStatus.Failure;
                        }
                        else
                        {
                            return RunStatus.Running;
                        }
                    }

                    )
                )
            
           )
         );
    }

    #endregion

    #region Affordances
    //add this shit later boys
    protected Node OpenDoor(GameObject knob){
        return null;
    }

    #endregion
}
