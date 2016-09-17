using UnityEngine;
using System.Collections;

public class VRPointer : MonoBehaviour {

    public enum State { IDLE,SELECT,CLICK}
    [SerializeField]
    private State currentState;


    public State CurrentState { get { return currentState; } set { currentState = value; } }


    IEnumerator Start() {

        while (true) {
            switch (currentState) {
                case State.IDLE:
                    Idle();
                    break;
                case State.SELECT:
                    Select();
                    break;
                case State.CLICK:
                    Click();
                    break;
            }

            yield return 0;
        }
    }

    public void SetPointerPosition(Vector3 position)
    {

        position.x = 0;
        position.y = 0;
  
        transform.localPosition = position;

        //.10 scales the pointer to a reasonable size
        transform.localScale = Vector3.one * Vector3.Distance(Camera.main.transform.position, transform.position) * .10f;
    }

    
    public void SetState(State newState)
    {
        currentState = newState;
    }

  public virtual void Idle() {

    }

  public virtual void Select() {

    }

    public virtual void Click() {

    }

}
