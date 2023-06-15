
function buttonClickGoDetail() {
    windowOpenSelf('./patternB1.html');
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

//JSON文字列
//string strWakeList = @"
//    {   
//        wake1:{
//            pos1:{ x: 120.307 , y: 35.846 },
//            pos2:{ x: 124.496 , y: 33.370 },
//            pos3:{ x: 121.259 , y: 31.974 },
//            pos4:{ x: 123.925 , y: 30.197 }
//        },
//        wake2:{
//            pos1:{ x: 134.080 , y: 41.495 },
//            pos2:{ x: 136.238 , y: 40.479 },
//            pos3:{ x: 133.572 , y: 39.781 },
//            pos4:{ x: 136.238 , y: 38.892 }
//        },
//        wake3:{
//            pos1:{ x: 143.855 , y: 34.703 },
//            pos2:{ x: 145.505 , y: 33.307 },
//            pos3:{ x: 143.030 , y: 32.545 },
//            pos4:{ x: 145.378 , y: 31.276 }
//        }
//    }";

//航跡初期化
function InitWake() {
    //var scene = "SceneA";
    //var scene = "SceneB";
    //var scene = "SceneC";
    var scene = "SceneC";

    //JSON文字列
    //AWake
    var strDictAWake = "\
    {\
        aWake1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 120.007 , y: 35.846 },\
            pos2:{ x: 124.496 , y: 33.370 },\
            pos3:{ x: 121.259 , y: 31.974 },\
            pos4:{ x: 123.925 , y: 30.197 }\
        },\
        aWake2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 136.238 , y: 38.892 },\
            pos2:{ x: 133.572 , y: 39.781 },\
            pos3:{ x: 136.238 , y: 40.479 },\
            pos4:{ x: 134.080 , y: 41.495 }\
        },\
        aWake3:{\
            info:{ row: 3, id: 3 },\
            pos1:{ x: 143.855 , y: 34.703 },\
            pos2:{ x: 145.505 , y: 33.307 },\
            pos3:{ x: 143.030 , y: 32.545 },\
            pos4:{ x: 145.378 , y: 31.276 }\
        }\
    }\
    ";

    //BWake
    var strDictBWake = "\
    {\
        bWake1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 135.735 , y: 36.043 },\
            pos2:{ x: 135.552 , y: 39.055 },\
            pos3:{ x: 135.220 , y: 38.740 },\
            pos4:{ x: 134.321 , y: 38.955 },\
            pos5:{ x: 135.486 , y: 36.593 }\
        },\
        bWake2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 139.310 , y: 40.000 },\
            pos2:{ x: 135.183 , y: 39.773 },\
            pos3:{ x: 135.533 , y: 40.572 },\
            pos4:{ x: 135.233 , y: 41.088 },\
            pos5:{ x: 139.393 , y: 40.588 }\
        },\
        bWake3:{\
            info:{ row: 3, id: 3 },\
            pos1:{ x: 139.426 , y: 39.623 },\
            pos2:{ x: 132.405 , y: 39.041 },\
            pos3:{ x: 133.669 , y: 41.487 },\
            pos4:{ x: 133.187 , y: 42.886 },\
            pos5:{ x: 139.476 , y: 41.936 }\
        }\
    }\
    ";
    var strDictDTrack = "\
    {\
        dTrack1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 138.995 , y: 39.962 },\
            pos2:{ x: 136.985 , y: 39.762 },\
            pos3:{ x: 134.110 , y: 38.458 },\
            pos4:{ x: 132.127 , y: 36.647 }\
        },\
        dTrack2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 139.062 , y: 41.319 },\
            pos2:{ x: 136.253 , y: 41.067 },\
            pos3:{ x: 131.954 , y: 40.894 },\
            pos4:{ x: 129.930 , y: 41.506 }\
        }\
    }\
    ";
    var strDictCPlace = "\
    {\
        cPlace1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 135.615 , y: 38.840 },\
            pos2:{ x: 135.432 , y: 39.439 },\
            pos3:{ x: 134.767 , y: 38.940 },\
            pos4:{ x: 134.534 , y: 39.656 }\
        },\
        cPlace2:{\
            info:{ row: 2, id: 2 },\
            pos1:{ x: 133.968 , y: 40.155 },\
            pos2:{ x: 134.867 , y: 39.939 },\
            pos3:{ x: 134.983 , y: 40.355 },\
            pos4:{ x: 135.715 , y: 40.205 }\
        },\
        cPlace3:{\
            info:{ row: 3, id: 3 },\
            pos1:{ x: 135.932 , y: 40.887 },\
            pos2:{ x: 135.266 , y: 40.704 },\
            pos3:{ x: 135.116 , y: 41.220 },\
            pos4:{ x: 134.484 , y: 41.070 }\
        }\
    }\
    ";
    var strDictCPlace2 = "\
    {\
        cPlace1:{\
            info:{ row: 1, id: 1 },\
            pos1:{ x: 135.615 , y: 38.840 }\
        },\
        cPlace2:{\
            info:{ row: 2, id: 2 },\
            pos2:{ x: 135.432 , y: 39.439 }\
        },\
        cPlace3:{\
            info:{ row: 3, id: 3 },\
            pos3:{ x: 134.767 , y: 38.940 }\
        },\
        cPlace4:{\
            info:{ row: 4, id: 4 },\
            pos4:{ x: 134.534 , y: 39.656 }\
        }\
    }\
    ";
    //var strDictEArrow = "\
    //{\
    //    arrow1:{\
    //        primaryKey:{ id: 1 },\
    //        pos1:{ x: 135.615 , y: 38.840 , direction: 120 , distance: 0.4 },\
    //        pos2:{ x: 135.432 , y: 39.439 , direction: 225 , distance: 0.3 },\
    //        pos3:{ x: 134.767 , y: 38.940 , direction:  80 , distance: 0.3 },\
    //        pos4:{ x: 134.534 , y: 39.656 , direction: 290 , distance: 0.2 }\
    //    },\
    //    arrow2:{\
    //        primaryKey:{ id: 2 },\
    //        pos1:{ x: 133.968 , y: 40.155 , direction: 350 , distance: 0.5 },\
    //        pos2:{ x: 134.867 , y: 39.939 , direction:  95 , distance: 0.2 },\
    //        pos3:{ x: 134.983 , y: 40.355 , direction: 315 , distance: 0.2 },\
    //        pos4:{ x: 135.715 , y: 40.205 , direction: 170 , distance: 0.3 }\
    //    },\
    //    arrow3:{\
    //        primaryKey:{ id: 3 },\
    //        pos1:{ x: 135.932 , y: 40.887 , direction: 230 , distance: 0.2 },\
    //        pos2:{ x: 135.266 , y: 40.704 , direction:  45 , distance: 0.2 },\
    //        pos3:{ x: 135.116 , y: 41.220 , direction: 230 , distance: 0.2 },\
    //        pos4:{ x: 134.484 , y: 41.070 , direction:  60 , distance: 0.2 }\
    //    }\
    //}\
    //";
    var strDictEArrow = "\
    {\
        arrow1:{\
            primaryKey:{ row: 1, id: 1 },\
            pos1:{ x: 135.615 , y: 38.840 , direction: 120 , distance: 0.4 }\
        },\
        arrow2:{\
            primaryKey:{ row: 2, id: 2 },\
            pos1:{ x: 135.432 , y: 39.439 , direction: 225 , distance: 0.3 }\
        },\
        arrow3:{\
            primaryKey:{ row: 3, id: 3 },\
            pos1:{ x: 134.767 , y: 38.940 , direction:  80 , distance: 0.3 }\
        },\
        arrow4:{\
            primaryKey:{ row: 4, id: 4 },\
            pos1:{ x: 134.534 , y: 39.656 , direction: 290 , distance: 0.2 }\
        }\
    }\
    ";

    //文字列内の空白を全て削除する
    strDictAWake = strDictAWake.replaceAll(/\s+/g, '');
    strDictBWake = strDictBWake.replaceAll(/\s+/g, '');
    strDictCPlace = strDictCPlace.replaceAll(/\s+/g, '');
    strDictCPlace2 = strDictCPlace2.replaceAll(/\s+/g, '');
    strDictDTrack = strDictDTrack.replaceAll(/\s+/g, '');
    strDictEArrow = strDictEArrow.replaceAll(/\s+/g, '');

    switch ( scene ){
        case "SceneA" :
            //C#の関数の実行
            chrome.webview.hostObjects.jsToCs.InitWake(
                scene,
                strDictAWake,
                strDictBWake,
                strDictCPlace,
                strDictDTrack,
                strDictEArrow
                );
            break;
        case "SceneB" :
            //C#の関数の実行
            chrome.webview.hostObjects.jsToCs.InitWake(
                scene,
                strDictAWake,
                strDictBWake,
                strDictCPlace,
                strDictDTrack,
                strDictEArrow
                );
            break;
        case "SceneC" :
            //C#の関数の実行
            chrome.webview.hostObjects.jsToCs.InitWake(
                scene,
                strDictAWake,
                strDictBWake,
                strDictCPlace2, //シーンB⇔Cでデータの持ち方が変わる
                strDictDTrack,
                strDictEArrow
                );
            break;
        default : 
            break;
    }




}

