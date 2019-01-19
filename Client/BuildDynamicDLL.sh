rm -rf ./DynamicScriptsBuild
mkdir ./DynamicScriptsBuild
cd ./DynamicScriptsBuild

find ../DynamicUsedReferences -name '*.dll' -type f -exec cp {} ./ \;
find ../DynamicScripts -name '*.cs' -type f -exec cp {} ./ \;
find ../Assets/Plugins -name 'iTween.cs' -type f -exec cp {} ./ \;
find ../Assets/Plugins -name '*.dll' -type f -exec cp {} ./ \;
csc /r:UnityEngine.UI.dll /r:UnityEditor.dll /r:UnityEngine.dll /r:Newtonsoft.Json.dll /r:MyCardGameCommon.dll /target:library /out:InGameDynamic.dll ./*.cs
rm -f ./*.cs
