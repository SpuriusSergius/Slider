using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGrid : SGrid
{
    public static VillageGrid instance;

    public GameObject caveDoor;
    public GameObject particleSpawner;

    private bool fishOn;

    private static bool checkCompletion = false;

    private new void Awake() {
        myArea = Area.Village;

        foreach (Collectible c in collectibles) 
        {
            c.SetArea(myArea);
        }

        base.Awake();

        fishOn = WorldData.GetState("fishOn");
        if (fishOn)
        {
            particleSpawner.GetComponent<ParticleSpawner>().SetFishOn();
        }
        instance = this;
    }
    
    private void OnEnable() {
        if (checkCompletion) {
            SGrid.OnGridMove += SGrid.CheckCompletions;
            SGridAnimator.OnSTileMoveEnd += CheckFinalPlacementsOnMove;
        }
    }

    private void OnDisable() {
        if (checkCompletion) {
            SGrid.OnGridMove -= SGrid.CheckCompletions;
            SGridAnimator.OnSTileMoveEnd -= CheckFinalPlacementsOnMove;
        }
    }

    void Start()
    {
        foreach (Collectible c in collectibles) 
        {
            if (PlayerInventory.Contains(c)) 
            {
                c.gameObject.SetActive(false);
            }
        }

        AudioManager.PlayMusic("Connection");
        UIEffects.FadeFromBlack();
    }

    public override void SaveGrid() 
    {
        base.SaveGrid();

        // GameManager.saveSystem.SaveSGridData(Area.Village, this);
        // GameManager.saveSystem.SaveMissions(new Dictionary<string, bool>());
    }

    public override void LoadGrid()
    {
        base.LoadGrid();
    }


    // === Village puzzle specific ===
    public void CheckFishOn(Conditionals.Condition c)
    {
        c.SetSpec(fishOn);
    }

    public void TurnFishOn()
    {
        if (!fishOn)
        {
            WorldData.SetState("fishOn", true);
            fishOn = true;
            particleSpawner.GetComponent<ParticleSpawner>().SetFishOn();
        }
    }

    // Puzzle 8 - 8puzzle
    public void ShufflePuzzle() {
        int[,] shuffledPuzzle = new int[3, 3] { { 7, 0, 1 },
                                                { 6, 4, 8 },
                                                { 5, 3, 2 } };
        SetGrid(shuffledPuzzle);

        // fading stuff
        UIEffects.FlashWhite();
        CameraShake.Shake(1.5f, 1.0f);

        checkCompletion = true;
        OnGridMove += CheckCompletions;
        SGridAnimator.OnSTileMoveEnd += CheckFinalPlacementsOnMove;// SGrid.OnGridMove += SGrid.CheckCompletions
    }


    private void CheckFinalPlacementsOnMove(object sender, SGridAnimator.OnTileMoveArgs e)
    {
        if (!PlayerInventory.Contains("Slider 9", Area.Village) && (GetGridString() == "624_8#7_153"))
        {
            // ActivateSliderCollectible(9);
            GivePlayerTheCollectible("Slider 9");

            // we don't have access to the Collectible.StartCutscene() pick up, so were doing this dumb thing instead
            StartCoroutine(CheckCompletionsAfterDelay(1.1f));

            AudioManager.Play("Puzzle Complete");
        }
    }

    private IEnumerator CheckCompletionsAfterDelay(float t)
    {
        yield return new WaitForSeconds(t);

        CheckCompletions(this, null); // sets the final one to be complete
    }

    public void Explode()
    {
        caveDoor.SetActive(true);
        CameraShake.Shake(4f, 3.5f);
    }
}
