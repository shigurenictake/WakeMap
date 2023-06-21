
function buttonClickGoDetail() {
    windowOpenSelf('./subPatternB.html');
}

/** メモ
●group1 : 福岡県の西
[ 0 ] : POINT (120.30740526824003 35.846238257443026)
[ 1 ] : POINT (124.49650372695156 33.370861895477127)
[ 2 ] : LINESTRING (120.30740526824003 35.846238257443026, 124.49650372695156 33.370861895477127)
[ 3 ] : POINT (121.25947309976539 31.974495742573282)
[ 4 ] : LINESTRING (124.49650372695156 33.370861895477127, 121.25947309976539 31.974495742573282)
[ 5 ] : POINT (123.92526302803636 30.197302457059305)
[ 6 ] : LINESTRING (121.25947309976539 31.974495742573282, 123.92526302803636 30.197302457059305)

●group2 : 石川県の北
[ 0 ] : POINT (134.08066205432851 41.495174555158819)
[ 1 ] : POINT (136.23868247245261 40.479635534865118)
[ 2 ] : LINESTRING (134.08066205432851 41.495174555158819, 136.23868247245261 40.479635534865118)
[ 3 ] : POINT (133.57289254418166 39.7814524584132)
[ 4 ] : LINESTRING (136.23868247245261 40.479635534865118, 133.57289254418166 39.7814524584132)
[ 5 ] : POINT (136.23868247245261 38.892855815656205)
[ 6 ] : LINESTRING (133.57289254418166 39.7814524584132, 136.23868247245261 38.892855815656205)

●group3 : 千葉県の南東
[ 0 ] : POINT (143.85522512465539 34.703757356944685)
[ 1 ] : POINT (145.50547603263266 33.30739120404084)
[ 2 ] : LINESTRING (143.85522512465539 34.703757356944685, 145.50547603263266 33.30739120404084)
[ 3 ] : POINT (143.03009967066674 32.545736938820568)
[ 4 ] : LINESTRING (145.50547603263266 33.30739120404084, 143.03009967066674 32.545736938820568)
[ 5 ] : POINT (145.37853365509594 31.276313163453437)
[ 6 ] : LINESTRING (143.03009967066674 32.545736938820568, 145.37853365509594 31.276313163453437)
**/

//航跡初期化
function InitWake() {
    var strHtmlName = window.location.href.split('/').pop();
    console.log(strHtmlName);
    //var scene = "SceneA";
    //var scene = "SceneB";
    //var scene = "SceneC";
    var scene;
    switch (strHtmlName) {
        case "patternB1.html" :
            scene = "SceneA";
            break;
        case "patternB2.html" :
            scene = "SceneB";
            break;
        case "patternB3.html" :
            scene = "SceneC";
            break;
    }

    //JSON文字列
    //SceneA ============================================================================
    var SceneA_strDictAWake = "\
    {\
        aWake1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 120.007 , y: 35.846 , 'time': '20230101001111' },\
            pos2:{ x: 124.496 , y: 33.370 , 'time': '20230101001122' },\
            pos3:{ x: 121.259 , y: 31.974 , 'time': '20230101001133' },\
            pos4:{ x: 123.925 , y: 30.197 , 'time': '20230101001144' }\
        },\
        aWake2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 136.700 , y: 39.000 , 'time': '20230101001100' },\
            pos2:{ x: 136.200 , y: 39.000 , 'time': '20230101001110' },\
            pos3:{ x: 135.500 , y: 38.600 , 'time': '20230101001120' },\
            pos4:{ x: 134.800 , y: 38.500 , 'time': '20230101001130' },\
            pos5:{ x: 134.200 , y: 38.800 , 'time': '20230101001140' },\
            pos6:{ x: 133.800 , y: 38.800 , 'time': '20230101001150' },\
            pos7:{ x: 133.572 , y: 39.781 , 'time': '20230101001200' },\
            pos8:{ x: 136.238 , y: 40.479 , 'time': '20230101001300' },\
            pos9:{ x: 134.080 , y: 41.495 , 'time': '20230101001400' }\
        },\
        aWake3:{\
            info:{ row: 3, id: 3 },\
            pos1:{ x: 143.855 , y: 34.703 , 'time': '20230102001111' },\
            pos2:{ x: 145.505 , y: 33.307 , 'time': '20230102001122' },\
            pos3:{ x: 143.030 , y: 32.545 , 'time': '20230102001133' },\
            pos4:{ x: 145.378 , y: 31.276 , 'time': '20230102001144' }\
        }\
    }\
    ";

    //SceneB ===============================================================
    var SceneB_strDictAWake = "\
    {\
        aWake2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 136.700 , y: 39.000 , 'time': '20230101001100' },\
            pos2:{ x: 136.200 , y: 39.000 , 'time': '20230101001110' },\
            pos3:{ x: 135.500 , y: 38.600 , 'time': '20230101001120' },\
            pos4:{ x: 134.800 , y: 38.500 , 'time': '20230101001130' },\
            pos5:{ x: 134.200 , y: 38.800 , 'time': '20230101001140' },\
            pos6:{ x: 133.800 , y: 38.800 , 'time': '20230101001150' },\
            pos7:{ x: 133.572 , y: 39.781 , 'time': '20230101001200' },\
            pos8:{ x: 136.238 , y: 40.479 , 'time': '20230101001300' },\
            pos9:{ x: 134.080 , y: 41.495 , 'time': '20230101001400' }\
        }\
    }\
    ";
    var SceneB_strDictDTrack = "\
    {\
        dTrack1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 138.995 , y: 39.962 , 'time': '20230101001100' },\
            pos2:{ x: 136.985 , y: 39.762 , 'time': '20230101001110' },\
            pos3:{ x: 134.110 , y: 38.458 , 'time': '20230101001150' },\
            pos4:{ x: 132.127 , y: 36.647 , 'time': '20230101001200' }\
        },\
        dTrack2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 139.062 , y: 41.319 , 'time': '20230101001100' },\
            pos2:{ x: 136.253 , y: 41.067 , 'time': '20230101001110' },\
            pos3:{ x: 131.954 , y: 40.894 , 'time': '20230101001150' },\
            pos4:{ x: 129.930 , y: 41.506 , 'time': '20230101001200' }\
        }\
    }\
    ";
    var SceneB_strDictBWake = "\
    {\
        bWake1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 135.735 , y: 36.043 , 'time': '20230101001110' },\
            pos2:{ x: 135.552 , y: 39.055 , 'time': '20230101001120' },\
            pos3:{ x: 135.220 , y: 38.740 , 'time': '20230101001130' },\
            pos4:{ x: 134.321 , y: 38.955 , 'time': '20230101001140' },\
            pos5:{ x: 135.486 , y: 36.593 , 'time': '20230101001150' }\
        },\
        bWake2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 139.310 , y: 40.000 , 'time': '20230101001111' },\
            pos2:{ x: 135.183 , y: 39.773 , 'time': '20230101001122' },\
            pos3:{ x: 135.533 , y: 40.572 , 'time': '20230101001133' },\
            pos4:{ x: 135.233 , y: 41.088 , 'time': '20230101001144' },\
            pos5:{ x: 139.393 , y: 40.588 , 'time': '20230101001155' }\
        },\
        bWake3:{\
            info:{ row: 3, id: 3 },\
            pos1:{ x: 139.426 , y: 39.623 , 'time': '20230102001111' },\
            pos2:{ x: 132.405 , y: 39.041 , 'time': '20230102001122' },\
            pos3:{ x: 133.669 , y: 41.487 , 'time': '20230102001133' },\
            pos4:{ x: 133.187 , y: 42.886 , 'time': '20230102001144' },\
            pos5:{ x: 139.476 , y: 41.936 , 'time': '20230102001155' }\
        }\
    }\
    ";
    var SceneB_strDictCPlace = "\
    {\
        cPlace1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 135.615 , y: 38.200 , 'time': '20230101001110' },\
            pos2:{ x: 135.432 , y: 38.800 , 'time': '20230101001120' },\
            pos3:{ x: 134.767 , y: 38.300 , 'time': '20230101001130' },\
            pos4:{ x: 134.534 , y: 39.100 , 'time': '20230101001140' },\
            pos5:{ x: 134.000 , y: 38.400 , 'time': '20230101001150' }\
        },\
        cPlace2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 133.968 , y: 40.155 , 'time': '20230101001111' },\
            pos2:{ x: 134.867 , y: 39.939 , 'time': '20230101001122' },\
            pos3:{ x: 134.983 , y: 40.355 , 'time': '20230102001133' },\
            pos4:{ x: 135.715 , y: 40.205 , 'time': '20230102001144' }\
        },\
        cPlace3:{\
            info:{ row: 3, id: 3 },\
            pos1:{ x: 135.932 , y: 40.887 , 'time': '20230101001111' },\
            pos2:{ x: 135.266 , y: 40.704 , 'time': '20230101001122' },\
            pos3:{ x: 135.116 , y: 41.220 , 'time': '20230102001133' },\
            pos4:{ x: 134.484 , y: 41.070 , 'time': '20230102001144' }\
        }\
    }\
    ";

    //SceneC ===============================================================
    var SceneC_strDictAWake = "\
    {\
        aWake2:{\
            info:{ row: 2, id: 2 },\
            pos2:{ x: 136.200 , y: 39.000 , 'time': '20230101001110' },\
            pos3:{ x: 135.500 , y: 38.600 , 'time': '20230101001120' },\
            pos4:{ x: 134.800 , y: 38.500 , 'time': '20230101001130' },\
            pos5:{ x: 134.200 , y: 38.800 , 'time': '20230101001140' },\
            pos6:{ x: 133.800 , y: 38.800 , 'time': '20230101001150' }\
        }\
    }\
    ";
    var SceneC_strDictDTrack = "\
    {\
        dTrack1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 138.995 , y: 39.962 , 'time': '20230101001100' },\
            pos2:{ x: 136.985 , y: 39.762 , 'time': '20230101001110' },\
            pos3:{ x: 134.110 , y: 38.458 , 'time': '20230102001150' },\
            pos4:{ x: 132.127 , y: 36.647 , 'time': '20230102001200' }\
        },\
        dTrack2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 139.062 , y: 41.319 , 'time': '20230101001100' },\
            pos2:{ x: 136.253 , y: 41.067 , 'time': '20230101001110' },\
            pos3:{ x: 131.954 , y: 40.894 , 'time': '20230102001150' },\
            pos4:{ x: 129.930 , y: 41.506 , 'time': '20230102001200' }\
        }\
    }\
    ";
    var SceneC_strDictBWake = "\
    {\
        bWake1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 135.735 , y: 36.043 , 'time': '20230101001110' },\
            pos2:{ x: 135.552 , y: 39.055 , 'time': '20230101001120' },\
            pos3:{ x: 135.220 , y: 38.740 , 'time': '20230101001130' },\
            pos4:{ x: 134.321 , y: 38.955 , 'time': '20230101001140' },\
            pos5:{ x: 135.486 , y: 36.593 , 'time': '20230101001150' }\
        }\
    }\
    ";
    var SceneC_strDictCPlace = "\
    {\
        arrow1:{\
            primaryKey:{ row: 1, id: 1 },\
            pos:{ x: 135.615 , y: 38.200 , direction: 120 , distance: 0.4 , 'time': '20230101001110' }\
        },\
        arrow2:{\
            primaryKey:{ row: 2, id: 2 },\
            pos:{ x: 135.432 , y: 38.800 , direction: 225 , distance: 0.3 , 'time': '20230101001120' }\
        },\
        arrow3:{\
            primaryKey:{ row: 3, id: 3 },\
            pos:{ x: 134.767 , y: 38.300 , direction:  80 , distance: 0.3 , 'time': '20230101001130' }\
        },\
        arrow4:{\
            primaryKey:{ row: 4, id: 4 },\
            pos:{ x: 134.534 , y: 39.100 , direction: 290 , distance: 0.2 , 'time': '20230101001140' }\
        },\
        arrow5:{\
            primaryKey:{ row: 5, id: 5 },\
            pos:{ x: 134.000 , y: 38.400 , direction: 290 , distance: 0.2 , 'time': '20230101001150' }\
        }\
    }\
    ";

    //文字列内の空白を全て削除する
    SceneA_strDictAWake  = SceneA_strDictAWake.replaceAll(/\s+/g, '');

    SceneB_strDictAWake  = SceneB_strDictAWake.replaceAll(/\s+/g, '');
    SceneB_strDictBWake  = SceneB_strDictBWake.replaceAll(/\s+/g, '');
    SceneB_strDictCPlace = SceneB_strDictCPlace.replaceAll(/\s+/g, '');
    SceneB_strDictDTrack = SceneB_strDictDTrack.replaceAll(/\s+/g, '');

    SceneC_strDictAWake  = SceneC_strDictAWake.replaceAll(/\s+/g, '');
    SceneC_strDictBWake  = SceneC_strDictBWake.replaceAll(/\s+/g, '');
    SceneC_strDictCPlace = SceneC_strDictCPlace.replaceAll(/\s+/g, '');
    SceneC_strDictDTrack = SceneC_strDictDTrack.replaceAll(/\s+/g, '');
    //SceneC_strDictEArrow = SceneC_strDictEArrow.replaceAll(/\s+/g, '');

    switch ( scene ){
        case "SceneA" :
            //C#の関数の実行
            chrome.webview.hostObjects.jsToCs.InitWake(
                scene,
                SceneA_strDictAWake,
                null,
                null,
                null
                );
            break;
        case "SceneB" :
            //C#の関数の実行
            chrome.webview.hostObjects.jsToCs.InitWake(
                scene,
                SceneB_strDictAWake,
                SceneB_strDictDTrack,
                SceneB_strDictBWake,
                SceneB_strDictCPlace
                );
            break;
        case "SceneC" :
            //C#の関数の実行
            chrome.webview.hostObjects.jsToCs.InitWake(
                scene,
                SceneC_strDictAWake,
                SceneC_strDictDTrack,
                SceneC_strDictBWake,
                SceneC_strDictCPlace //シーンB⇔Cでデータの持ち方が変わる
                );
            break;
        default : 
            break;
    }
}

