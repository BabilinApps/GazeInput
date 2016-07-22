using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GazePointer : VRPointer {

    public UnityEvent onIdle;
    public UnityEvent onSelect;
    public UnityEvent onClick;

    private PointerState[] StateArray;
    void Awake() {
        StateArray = transform.GetComponentsInChildren<PointerState>(true); 

    }

    public void SetState(PointerState state) {

        for (int i = 0; i < StateArray.Length; i++) {
            StateArray[i].gameObject.SetActive(false);
        }

        state.gameObject.SetActive(true);
    }
    public override void Idle()
    {
        base.Idle();
        onIdle.Invoke();
    }
    public override void Select()
    {
        base.Select();
        onSelect.Invoke();
    }
    public override void Click()
    {
        base.Click();
        onClick.Invoke();
    } 
}
