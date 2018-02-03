using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bojoing : MonoBehaviour {
    public Transform target;

    // Use this for initialization
    void Start () {
  
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Bojoing1()
    {
        // Create new Sequence object
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(target.DOScale(0.5f, 0.4f));
        mySequence.Append(target.DOScale(0.3f, 0.4f));
        mySequence.Append(target.DOScale(0.8f, 0.45f));
        mySequence.Append(target.DOScale(0.6f, 0.35f));
        mySequence.Append(target.DOScale(1.2f, 0.5f));
        mySequence.Append(target.DOScale(0.75f, 0.4f));
        mySequence.Append(target.DOScale(1f, 0.7f)).onComplete();
    }
}
