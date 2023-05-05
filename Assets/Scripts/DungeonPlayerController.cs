using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DungeonPlayerController : MonoBehaviour
{
    private const int LEFT_TOP = 0;
    private const int RIGHT_TOP = 1;
    private const int LEFT_BOTTOM = 2;
    private const int RIGHT_BOTTOM = 3;
    private const float STEP_DISTANCE = 262.8f;
    private const float NORMAL_RELATIVE_DISTANCE = 350;
    private const float ABNORMAL_RELATIVE_DISTANCE = STEP_DISTANCE - NORMAL_RELATIVE_DISTANCE;
    [SerializeField]
    private DungeonManager dungeonManager;
    [SerializeField]
    private Image leftAvailableAreaIndicator;
    [SerializeField]
    private Image rightAvailableAreaIndicator;
    [SerializeField]
    private Image[] cornerFrames;
    [SerializeField]
    private Button[] availableMovements;

    private int currentPos = 2;
    // Start is called before the first frame update
    private void Awake()
    {
        dungeonManager.newAreaButtonPressed += HorizontalTransformation;
        DungeonManager.sceneOver += unreg;
    }
    private void unreg(object sender, int pos) 
    {
        dungeonManager.newAreaButtonPressed -= HorizontalTransformation;
        DungeonManager.sceneOver -= unreg;
    }

    private void HorizontalTransformation(object sender, int pos)
    {
        //for each button, only active the reachable routes
        for (int i = 0; i < 5; i++)
        {
            if (Mathf.Abs(i - pos) < 2)
                availableMovements[i].enabled = true;
            else
                availableMovements[i].enabled = false;
        }
        if (pos == currentPos)
            return;
        else 
        {
            transform.position = new Vector2(transform.position.x + (STEP_DISTANCE * (pos - currentPos)),
                transform.position.y);
            //350 - 262.8 = 87.2
            if (pos == 0)
            {
                leftAvailableAreaIndicator.enabled = false;
                changeRelativePlace(LEFT_TOP, ABNORMAL_RELATIVE_DISTANCE);
                changeRelativePlace(LEFT_BOTTOM, ABNORMAL_RELATIVE_DISTANCE);
                availableMovements[2].enabled = false;
            }
            else if (pos == 4) 
            {
                rightAvailableAreaIndicator.enabled = false;
                changeRelativePlace(RIGHT_TOP, -ABNORMAL_RELATIVE_DISTANCE);
                changeRelativePlace(RIGHT_BOTTOM, -ABNORMAL_RELATIVE_DISTANCE);
                //only way is from 3 to 4, so also disable 2
                availableMovements[2].enabled = false;
            }
            else
            {
                rightAvailableAreaIndicator.enabled = true;
                leftAvailableAreaIndicator.enabled = true;
                changeRelativePlace(LEFT_TOP, -NORMAL_RELATIVE_DISTANCE);
                changeRelativePlace(LEFT_BOTTOM, -NORMAL_RELATIVE_DISTANCE);
                changeRelativePlace(RIGHT_TOP, NORMAL_RELATIVE_DISTANCE);
                changeRelativePlace(RIGHT_BOTTOM, NORMAL_RELATIVE_DISTANCE);
            }
        }
        currentPos = pos;
    }

    private void changeRelativePlace(int cornorLoc, float relativePos) 
    {
        Vector2 cornerPos = cornerFrames[cornorLoc].transform.position;
        cornerPos.x = transform.position.x + relativePos;
        cornerFrames[cornorLoc].transform.position = cornerPos;
    }
}
