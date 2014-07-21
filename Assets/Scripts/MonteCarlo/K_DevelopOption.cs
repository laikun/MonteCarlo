using UnityEngine;
using System.Collections;
using System;

public class K_DevelopOption : SingletonInGame<K_DevelopOption>
{

    public bool OnScreen { get; set; }

    bool showUp;

    public void DevelopOption(bool sw)
    {
        showUp = sw;
    }

    int column = 0;
    int row = 0;
    bool autoDraw = false;
    float[] coin = {0f, 5f};
    float[] hint = {0f, 5f};
    float[] timelimit = {0f, 300f};
    float[] turnlimit = {0f, 10f};
    float[] separate = {0.05f, 0.5f};
    bool pokerRule = false;
    
    protected override void awake()
    {
        K_OptionData.Set<int>("Column", 5);
        K_OptionData.Set<int>("Row", 5);
        K_OptionData.Set<int>("AutoDraw", autoDraw ? 1 : 0);
        K_OptionData.Set<int>("Coin", 5);
        K_OptionData.Set<int>("Hint", Mathf.RoundToInt(hint [0]));
        K_OptionData.Set<int>("TimeLimit", Mathf.RoundToInt(timelimit [0]));
        K_OptionData.Set<int>("TurnLimit", 5);
        K_OptionData.Set<float>("Separate", separate [0]);
        K_OptionData.Set<int>("PokerRule", pokerRule ? 1 : 0);
    }

    void OnGUI()
    {

        if (!OnScreen)
            return;

        if (!showUp && GUI.Button(new Rect(Screen.width - 150, Screen.height - 50, 150, 50), "DEVELOPER MODE"))
        {
            showUp = true;

            K_CameraManager.Instance.AllEventActive(false);

            column = K_OptionData.Get<int>("Column");
            row = K_OptionData.Get<int>("Row");
            autoDraw = K_OptionData.Get<int>("AutoDraw") == 1;
            coin[0] = K_OptionData.Get<int>("Coin");
            turnlimit[0] = K_OptionData.Get<int>("TurnLimit");
            separate [0] = K_OptionData.Get<float>("Separate");
            pokerRule = K_OptionData.Get<int>("PokerRule") == 1;
        }

        if (!showUp)
            return;

        if (GUI.Button(new Rect(Screen.width / 2 - 100, 50, 200, 40), "Back To The Game"))
        {
            showUp = false;
            K_CameraManager.Instance.AllEventActive(true);

            if (Application.loadedLevelName.Equals("Play"))
            {
                K_Rule.Instance.SendMessage("NewGame");
            }
        }

        GUILayout.Space(100f);

        Rect optionArea = new Rect(Screen.width / 2 - 200, 120, 400, 400);
        GUI.Box(optionArea, "Options");
        GUILayout.BeginArea(optionArea);
        GUILayout.Space(30f);

        #region OptionSetting
        // Set Column
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Column", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        column = GUILayout.Toolbar(column, new string[]{"4", "5"}, GUILayout.MinWidth(200));
        GUILayout.Space(10f);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        // Set Column
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Row", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        row = GUILayout.Toolbar(row, new string[]{"4", "5"}, GUILayout.MinWidth(200));
        GUILayout.Space(10f);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        // Set AutoDraw
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("AutoDraw", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        autoDraw = GUILayout.Toggle(autoDraw, autoDraw ? "On" : "Off", GUILayout.Width(150));
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        // Set Coin
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Coin", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.Box(Mathf.RoundToInt(coin [0]) + "$", GUILayout.Width(50));
        GUILayout.BeginVertical();
        GUILayout.Space(9);
        coin [0] = GUILayout.HorizontalSlider(coin [0], 0.0f, coin [1], GUILayout.MinWidth(100));
        GUILayout.EndVertical();
        GUILayout.Space(3);
        if (GUILayout.Button("Max+"))
        {
            if (coin [0] < 99)
                coin [1]++;
        }
        GUILayout.Space(10f);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        //// Set Hint
        //GUILayout.BeginHorizontal();
        //GUILayout.Space(10f);
        //GUILayout.Label("Hint", GUILayout.Width(100));
        //GUILayout.FlexibleSpace();
        //GUILayout.Box(Mathf.RoundToInt(hint [0]) + "H", GUILayout.Width(50));
        //GUILayout.BeginVertical();
        //GUILayout.Space(9);
        //hint [0] = GUILayout.HorizontalSlider(hint [0], 0.0f, hint [1], GUILayout.MinWidth(100));
        //GUILayout.EndVertical();
        //GUILayout.Space(3);
        //if (GUILayout.Button("Max+"))
        //{
        //    if (hint [0] < 99)
        //        hint [1]++;
        //}
        //GUILayout.Space(10f);
        //GUILayout.EndHorizontal();
        //GUILayout.Space(10f);

        //// Set TimeLimit
        //GUILayout.BeginHorizontal();
        //GUILayout.Space(10f);
        //GUILayout.Label("TimeLimit", GUILayout.Width(100));
        //GUILayout.FlexibleSpace();
        //GUILayout.Box(Mathf.RoundToInt(timelimit [0]) + "s", GUILayout.Width(50));
        //GUILayout.BeginVertical();
        //GUILayout.Space(9);
        //timelimit [0] = GUILayout.HorizontalSlider(timelimit [0], 0.0f, timelimit [1], GUILayout.MinWidth(100));
        //GUILayout.EndVertical();
        //GUILayout.Space(3);
        //if (GUILayout.Button("Max+"))
        //{
        //    if (timelimit [0] < 999)
        //        timelimit [1]++;
        //}
        //GUILayout.Space(10f);
        //GUILayout.EndHorizontal();
        //GUILayout.Space(10f);

        // Set TurnLimit
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("TurnLimit", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.Box(Mathf.RoundToInt(turnlimit [0]) + "s", GUILayout.Width(50));
        GUILayout.BeginVertical();
        GUILayout.Space(9);
        turnlimit [0] = GUILayout.HorizontalSlider(turnlimit [0], 0.0f, turnlimit [1], GUILayout.MinWidth(100));
        GUILayout.EndVertical();
        GUILayout.Space(3);
        if (GUILayout.Button("Max+"))
        {
            if (turnlimit [0] < 99)
                turnlimit [1]++;
        }
        GUILayout.Space(10f);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        // Set Separate
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Separate", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.Box(separate [0].ToString("0.00") + "", GUILayout.Width(50));
        GUILayout.BeginVertical();
        GUILayout.Space(9);
        separate [0] = GUILayout.HorizontalSlider(separate [0], 0.0f, separate [1], GUILayout.MinWidth(100));
        GUILayout.EndVertical();
        GUILayout.Space(3);
        if (GUILayout.Button("Max+"))
        {
            if (separate [0] < 1f)
                separate [1]++;
        }
        GUILayout.Space(10f);
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);


        // Set Poker Rule
        GUILayout.Space(50f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Poker Rule", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        pokerRule = GUILayout.Toggle(pokerRule, pokerRule ? "On" : "Off", GUILayout.Width(150));
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);

        
        #endregion

        GUILayout.EndArea();

        if (GUI.changed)
        {
            K_OptionData.Set<int>("Column", column == 0 ? 4 : 5);
            K_OptionData.Set<int>("Row", row == 0 ? 4 : 5);
            K_OptionData.Set<int>("AutoDraw", autoDraw ? 1 : 0);
            K_OptionData.Set<int>("Coin", Mathf.RoundToInt(coin [0]));
            K_OptionData.Set<int>("Hint", Mathf.RoundToInt(hint [0]));
            K_OptionData.Set<int>("TimeLimit", Mathf.RoundToInt(timelimit [0]));
            K_OptionData.Set<int>("TurnLimit", Mathf.RoundToInt(turnlimit [0]));
            K_OptionData.Set<float>("Separate", separate [0]);
            K_OptionData.Set<int>("PokerRule", pokerRule ? 1 : 0);
        }
    }
}