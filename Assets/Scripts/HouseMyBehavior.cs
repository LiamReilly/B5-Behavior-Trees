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
    public Transform Bear3Approach;
    public Transform RomanticRunaway;
    public Transform RomanticRunaway2;
    [Space]
    [Header("Actors")]
    public GameObject Bear1;
    public GameObject Bear2;
    public GameObject Bear3;
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
                        PlayingCatchArc(),
                        Bear3EntranceArc()
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
   /* protected Node TvSelector()
    {
        return new Selector(
            Friendship(),
            Loneliness()
            );
    }*/
    protected Node Loneliness()
    {
        
        Val<Vector3> position3 = Val.V(() => tv.position);
        Val<bool> act = true;
        return new Sequence(
            Check2(),
            new LeafInvoke(() => print("got to loneliness")),
            LonelinessReturn(),
            Bear1.GetComponent<BehaviorMecanim>().Node_OrientTowards(position3),
            new LeafInvoke(()=> Bear1.GetComponent<CharacterMecanim>().SitDown()),
            new LeafInvoke(() => lecture.Play()),
            Bear2.GetComponent<BehaviorMecanim>().Node_HandAnimation("SATNIGHTFEVER", act),
            new LeafWait(1000000)
            );
        //waiting a 1000 seconds is an ending 
    }
    protected Node Friendship()
    {
        Val<Vector3> position = Val.V(() => Bear1CatchPosition.position);
        Val<Vector3> position2 = Val.V(() => Bear2CatchPosition.position);
        Val<Vector3> position3 = Val.V(() => outsideDoor.position);
        // Debug.Log("made it to friendship");
        return new Sequence(
            Check1(),
            new LeafInvoke(() => print("got to friendship")),
            //new SequenceParallel(
            Bear2.GetComponent<BehaviorMecanim>().Node_GoTo(position2),
            Bear1.GetComponent<BehaviorMecanim>().Node_GoTo(position3),
            Bear1.GetComponent<BehaviorMecanim>().Node_GoTo(position),
            //),
            //WalkToPositions(),
            new LeafWait(1000));
    }
    /*protected Node WalkToPositions()
    {
       
         return new Sequence(
        //return new Sequence(
            new SequenceParallel(
            Bear1.GetComponent<BehaviorMecanim>().Node_GoTo(position),
            Bear2.GetComponent<BehaviorMecanim>().Node_GoTo(position2)
            )
            );
    }*/
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
    #endregion
    #region PlayingCatchArc
    protected Node PlayingCatchArc()
    {
        Val<Vector3> position = Val.V(() => Bear1.transform.position);
        Val<Vector3> position2 = Val.V(() => Bear2.transform.position);
        Val<bool> act = true;
        return new Sequence(
            new LeafInvoke(() => print("made it to Catch Arc")),
            Bear1.GetComponent<BehaviorMecanim>().Node_OrientTowards(position2),
            Bear1.GetComponent<BehaviorMecanim>().Node_OrientTowards(position),
            new SequenceParallel(
                Bear2.GetComponent<BehaviorMecanim>().Node_FaceAnimation("FIREBREATH", act),
                Bear1.GetComponent<BehaviorMecanim>().Node_FaceAnimation("LOOKAWAY", act)
                ),
            new LeafWait(1000)
            );
    }
    #endregion
    #region Bear3EntranceArc
    protected Node Bear3EntranceArc()
    {
        Val<Vector3> position = Val.V(() => Bear3Approach.position);
        Val<bool> act = true;
       // Func<bool> shouldact = (true);
        return new Sequence(
            Bear3.GetComponent<BehaviorMecanim>().Node_GoTo(position),
            Bear3.GetComponent<BehaviorMecanim>().Node_HandAnimation("WAVE", act),
            GiveOptions()

            );
    }
    protected Node GiveOptions()
    {
        return new Sequence(
            new LeafInvoke(() => Text.GetComponent<Text>().text = "Do you want to (1)continue hanging out with your homie or(2)run off with bear3"),
            new LeafInvoke(() => Text.SetActive(true)),
            GetUserInput(),
            new Selector(
                ChooseHomie(),
                ChooseRomanticInterest()
                )
            );
    }
    protected Node ChooseHomie()
    {
        Val<Vector3> position = Val.V(() => RomanticRunaway.position);
        Val<bool> act = true;
        return new Sequence(
            Check1(),
            new LeafInvoke(()=> print("made it to choose homie")),
            Bear3.GetComponent<BehaviorMecanim>().Node_GoTo(position),
            Bear3.GetComponent<BehaviorMecanim>().Node_HandAnimation("CRY", act),
            new LeafWait(100000)
            );
    }
    protected Node ChooseRomanticInterest()
    {
        Val<Vector3> position = Val.V(() => RomanticRunaway.position);
        Val<Vector3> position2 = Val.V(() => RomanticRunaway2.position);
        Val<bool> act = true;
        return new Sequence(
            Check2(),
            new LeafInvoke(() => print("made it to choose Romantic Interest")),
            new SequenceParallel(
                Bear3.GetComponent<BehaviorMecanim>().Node_GoTo(position),
                Bear1.GetComponent<BehaviorMecanim>().Node_GoTo(position2),
                Bear2.GetComponent<BehaviorMecanim>().Node_HandAnimation("CRY", act)
                ),
            new LeafWait(100000)
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
    protected Node Check2()
    {
        return new LeafAssert(() => UserInput == 2
        );
    }
    protected Node Check1()
    {
        return new LeafAssert(() => UserInput == 1
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
