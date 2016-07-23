using UnityEngine;
using System.Collections;

public class SimpleColor : MonoBehaviour {
    public Color[] SelectColors = new Color[3] { Color.yellow, Color.red, Color.black };
    private Renderer rend;
	// Use this for initialization
	void Start () {
        rend = gameObject.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SelectColor() {
        rend.material.color = SelectColors[0];
    }
    public void Click() {
        rend.material.color = SelectColors[1];
    }
    public void Exit() {

        rend.material.color = SelectColors[2];
    }
}
