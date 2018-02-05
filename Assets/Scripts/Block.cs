using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Block : MonoBehaviour {
    public float time = 10f;
	// Use this for initialization
	void Start () {

        Birth();
        Destroy(this.gameObject, time - 2);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Birth()
    {
        Sequence seq = DOTween.Sequence();
        float adjustment = 10f;
        // Appearance
        seq.Append(this.transform.DOScale(1.5f / adjustment, 0.3f));
        seq.Append(this.transform.DOScale(0.5f / adjustment, 0.4f));
        seq.Append(this.transform.DOScale(1.2f / adjustment, 0.4f));
        seq.Append(this.transform.DOScale(0.7f / adjustment, 0.5f));
       // seq.Append(this.transform.DOScale(1.1f / adjustment, 0.3f));
       // seq.Append(this.transform.DOScale(0.6f / adjustment, 0.5f));
        seq.Append(this.transform.DOScale(1.2f / adjustment, 1f));

        // Disappearance
        seq.Append(this.transform.DOScale(0.4f / adjustment, time));

    }
}
