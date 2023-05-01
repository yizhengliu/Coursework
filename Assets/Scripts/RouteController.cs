using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
public class RouteController : MonoBehaviour
{
    [SerializeField]
    private int pos;
    [SerializeField]
    private DungeonManager dungeonManager;
    [SerializeField]
    private Image cardFrame;
    [SerializeField]
    private Image cardContent;
    private Image routeBackground;
    private Button goToNext = null;
    private string info = null;
    private void Awake()
    {
        dungeonManager.mapReaded += show;
        dungeonManager.newAreaMoved += GoToNextRoute;
        dungeonManager.randomRouteEntered += flipCard;
        //bind show with mapreaded function
        routeBackground = GetComponent<Image>();
        goToNext = GetComponent<Button>();
    }

    private void show(object sender, NextAreaInfoArgs args)
    {
        int area = pos / 10;
        int route = pos % 10;
        string[] mapInfos = args.MapInfos;
        int currentArea = args.Counter;
        if (currentArea < 0)
            currentArea += mapInfos.Length;
        if (area + currentArea >= mapInfos.Length)
            currentArea -= mapInfos.Length;
        info = mapInfos[area + currentArea].Split(' ')[route];
        LoadInfo();
    }

    private void GoToNextRoute(object sender, NextAreaInfoArgs args) {
        int area = pos / 10;
        if (args.Counter + area + 1 >= args.MapInfos.Length)
            area -= args.MapInfos.Length;
        info = args.MapInfos[args.Counter + area + 1].Split(' ')[pos % 10];
        LoadInfo();
    }

    private void LoadInfo()
    {
        int cardType = -1;
        cardContent.enabled = true;
        if (info.Contains('r'))
        {
            cardType = SpriteHolder.RANDOM;
            cardContent.enabled = false;
        }
        else if (info.Contains('x'))
        {
            cardType = SpriteHolder.BLOCKED;
            if (goToNext != null) goToNext.enabled = false;
        }
        else if (info.Contains('e'))
            cardType = SpriteHolder.EMPTY;
        else if (info.Contains('m'))
            cardType = SpriteHolder.MONSTER;
        else if (info.Contains('c'))
            cardType = SpriteHolder.CHEST;
        else if (info.Contains('d'))
            cardType = SpriteHolder.DEBUFF;
        else if (info.Contains('b'))
            cardType = SpriteHolder.BUFF;
        else if (info.Contains('M'))
            cardType = SpriteHolder.MAINCITY;
        else if (info.Contains('f'))
            cardType = SpriteHolder.EVENT;

        routeBackground.sprite = SpriteHolder.Instance.getCardSprite(cardType);
        cardFrame.color = SpriteHolder.Instance.GetTheme(cardType);
        string tempNum = string.Join("", info.ToCharArray().Where(Char.IsDigit));
        int num = -1;
        if (tempNum != "")
            num = int.Parse(tempNum);
        cardContent.sprite = SpriteHolder.Instance.getCardContent(cardType, num);
    }

    private void flipCard(object sender, string[] args) {
        if (goToNext != null) goToNext.enabled = false;
        int area = pos / 10;
        int route = pos % 10;
        int currentRoute = int.Parse(args[0]);
        if (area != 0 || route % 10 != currentRoute)
            return;
        StartCoroutine(cardFlipAnimation(args[1]));
        Debug.Log(info + " " + pos);
        
    }

    IEnumerator cardFlipAnimation(string info) {
        while (transform.localScale.x > 0) 
        {
            Vector3 scale = transform.localScale;
            scale.x -= 0.1f;
            transform.localScale = scale;
            yield return null;
        }
        this.info = info;
        LoadInfo();
        transform.localScale = new Vector3(1, 1, 1);
        yield return new WaitForSeconds(0.3f);
        dungeonManager.animationFinished();
    }
}
