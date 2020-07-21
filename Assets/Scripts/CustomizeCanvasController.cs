using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class CustomizeCanvasController : MonoBehaviour
{
    //SKIN_COUNT can be changed, as in, the functionality
    //of the code does not depend on it. However, there should
    //be SKIN_COUNT number of Skin transforms under the canvas
    //and SKIN_COUNT number of skin images in the Resources/_Sprites
    //folder.
    const int SKIN_COUNT = 9;
    Color highlightOnColor, highlightOffColor;
    Sprite[] skinImages;
    Transform[] skins;
    bool[] skinUnlocked;
    List<int> shuffleBag;

    private static System.Random rng = new System.Random();  
    void Awake()
    {
        highlightOnColor = new Color(0.1619794f, 0.5283019f, 0.177457f);
        highlightOffColor = new Color(0.5019608f, 0.5019608f, 0.5019608f);
        
        skins = new Transform[SKIN_COUNT];
        skinImages = new Sprite[SKIN_COUNT];
        skinUnlocked = new bool[SKIN_COUNT];
        
        for(int i = 1; i <= SKIN_COUNT; ++i){
            int index = i - 1;

            skins[index] = transform.Find("Skin" + i);

            skinImages[index] = Resources.Load<Sprite>("_Sprites/" + i);

            skinUnlocked[index] = false;
        }

        
    }

    public void PickAndUnlock(){
        //shuffleBag is constructed from selecting each skin that
        //is not unlocked.
        shuffleBag = skinUnlocked.Select((r,i)=> new {value = r, index = i})
                              .Where(r => r.value == false)
                              .Select(r => r.index)
                              .ToList();

        //Selecting a skin to unlock before padding the shuffleBag with copies
        int nextSelection = shuffleBag.ElementAt(Random.Range(0,shuffleBag.Count()));

        //Randomizing the order
        shuffleBag = shuffleBag.OrderBy(x => Random.value).ToList();

        //Preventing the shuffle animation from getting shorter for each
        //unlocked skin, by expanding the bag with copies of the same objects
        if(shuffleBag.Count() < SKIN_COUNT){
            shuffleBag.AddRange(shuffleBag.Take(SKIN_COUNT - shuffleBag.Count()));
        }
        
        StartCoroutine(ShuffleEffect(nextSelection)); 
    }

    //Unlocks a skin, setting its image as the designated
    //skin image in Resources/_Sprites folder.
    public void UnlockSkin(int id){
        if(!skinUnlocked[id]){
            skins[id].Find("SkinImage")
                     .GetComponent<Image>()
                     .sprite = skinImages[id];
            skinUnlocked[id] = true;    
        }
    }

    //This is separated from UnlockSkin() since it is also
    //used in creating the shuffle effect.
    void HighlightOn(int id){
        if(!skinUnlocked[id]){
            skins[id].GetComponent<Image>()
                     .color = highlightOnColor;
            skins[id].GetComponent<Button>()
                     .interactable = true;
        }
    }

    //We only have to highlight each unlockable skin
    //a limited time, so they need to be turned off right after.
    void HighlightOff(int id){
        if(!skinUnlocked[id]){
            skins[id].GetComponent<Image>()
                     .color = highlightOffColor;
            skins[id].GetComponent<Button>()
                     .interactable = false;
        }
    }

    IEnumerator ShuffleEffect(int nextSelection){
        while(shuffleBag.Any()){
            int next = shuffleBag.First();
            HighlightOn(next);
            yield return new WaitForSeconds(0.2f); //This can be a variable
            HighlightOff(next);
            shuffleBag.Remove(next);
        }
        HighlightOn(nextSelection);
        UnlockSkin(nextSelection);
    }
}
