using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactTileButton : MonoBehaviour
{
    // public static bool canComplete = false;
    public bool isComplete = false;
    public bool isForcedDown = false;

    public bool isTileActive = false;
    public int islandId = -1;
    public int x;
    public int y;

    public bool flickerNext = false;
    private bool startsActive;

    private const int UI_OFFSET = 37;

    public STile myStile { get; private set; }
    public ArtifactTileButton linkButton;

    private Sprite islandSprite;
    public Sprite completedSprite;
    public Sprite emptySprite;
    public Sprite hoverSprite;
    public Sprite blankSprite;
    public ArtifactTileButtonAnimator buttonAnimator;
    public UIArtifact buttonManager;

    private void Awake() 
    {
        islandSprite = buttonAnimator.sliderImage.sprite;
    }

    private void Start()
    {
        myStile = SGrid.current.GetStile(islandId); // happens in SGrid.Awake()
        
        startsActive = myStile.isTileActive;
        SetTileActive(myStile.isTileActive);
        SetPosition(myStile.x, myStile.y);

        linkButton = null;
        foreach (ArtifactTileButton b in buttonManager.buttons) {
            if (myStile.linkTile != null && myStile.linkTile == b.myStile)
            {
                linkButton = b;
                b.linkButton = this;
            }
        }

        //if (!isTileActive)
        //{
        //    buttonAnimator.sliderImage.sprite = emptySprite;
        //}
        // update artifact button
    }

    public void OnDisable()
    {
        if (myStile.isTileActive)
        {
            if (buttonAnimator.sliderImage.sprite == emptySprite || buttonAnimator.sliderImage.sprite == blankSprite)
            {
                ResetToIslandSprite();
            }
        }
    }

    public void SetPosition(int x, int y)
    {
        //Debug.Log("Current position: " + this.x + "," + this.y);
        this.x = x;
        this.y = y;
        //Debug.Log("New position: " + this.x + "," + this.y);

        Vector3 pos = new Vector3(x - 1, y - 1) * UI_OFFSET;
        GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void SelectButton()
    {
        buttonManager.SelectButton(this);
    }

    //public void UpdatePushedDown()
    //{
    //    buttonAnimator.UpdatePushedDown();
    //}

    public void SetHighlighted(bool v)
    {
        buttonAnimator.SetHighlighted(v);
    }

    public void SetPushedDown(bool v)
    {
        buttonAnimator.SetPushedDown(v);
    }

    public void SetSelected(bool v)
    {
        buttonAnimator.SetSelected(v);
    }

    public void SetForcedPushedDown(bool v)
    {
        buttonAnimator.SetForcedPushedDown(v);
        isForcedDown = v;
    }

    public void SetTileActive(bool v)
    {
        if (islandSprite == null)
        {
            islandSprite = buttonAnimator.sliderImage.sprite;
        }

        isTileActive = v;
        if (v)
        {
            if (!startsActive)
            {
                flickerNext = true;
            }
            buttonAnimator.sliderImage.sprite = islandSprite;
        }
        else
        {
            buttonAnimator.sliderImage.sprite = emptySprite;
        }
    }

    public void SetComplete(bool value)
    {
        if (!isTileActive)
            return;

        isComplete = value;
        ResetToIslandSprite();
    }

    public void ResetToIslandSprite()
    {
        if (!isTileActive)
        {
            buttonAnimator.sliderImage.sprite = emptySprite;
        }
        else if (isComplete)
        {
            buttonAnimator.sliderImage.sprite = completedSprite;
        }
        else
        {
            buttonAnimator.sliderImage.sprite = islandSprite;
        }
    }

    public void Flicker() {
        flickerNext = false;
        StartCoroutine(NewButtonFlicker());
    }

    private IEnumerator NewButtonFlicker() {
        ResetToIslandSprite();
        yield return new WaitForSeconds(.25f);
        for (int i = 0; i < 3; i++) 
        {
            yield return new WaitForSeconds(.25f);
            buttonAnimator.sliderImage.sprite = blankSprite;
            yield return new WaitForSeconds(.25f);
            ResetToIslandSprite();
        }
    }

    public void AfterStileMoveDragged(object sender, SGridAnimator.OnTileMoveArgs e) 
    {
        if (e.stile.islandId == islandId)
            SetPushedDown(false);
    }
}
