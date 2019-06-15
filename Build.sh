git pull

rm -rf ./ServerBuild/*
cd ./ServerBuild

find ../Client/Assets/Scripts/Core -name '*.cs' -type f | xargs -i cp {} ./
cp ../Client/Assets/Plugins/NewtonsoftJson/Newtonsoft.Json.dll ./
csc /r:Newtonsoft.Json.dll /target:library /out:Core.dll ./*.cs
rm -f ./*.cs

find ../Client/Assets/Scripts/Network -name '*.cs' -type f | xargs -i cp {} ./
cp ../Client/Assets/Plugins/NewtonsoftJson/Newtonsoft.Json.dll ./
csc /r:Core.dll /r:Newtonsoft.Json.dll /target:library /out:Network.dll ./*.cs
rm -f ./*.cs

find ../Client/Assets/Scripts/BattleProxy -name '*.cs' -type f | xargs -i cp {} ./
cp ../Client/Assets/Plugins/NewtonsoftJson/Newtonsoft.Json.dll ./
csc /r:Core.dll /r:Network.dll /r:Newtonsoft.Json.dll /target:library /out:BattleProxy.dll ./*.cs
rm -f ./*.cs

find ../Client/Assets/Scripts/GameProxy -name '*.cs' -type f | xargs -i cp {} ./
cp ../Client/Assets/Plugins/NewtonsoftJson/Newtonsoft.Json.dll ./
csc /r:Core.dll /r:Network.dll /r:BattleProxy.dll /r:Newtonsoft.Json.dll /target:library /out:GameProxy.dll ./*.cs
rm -f ./*.cs

find ../Client/Assets/Scripts/Server -name '*.cs' -type f | xargs -i cp {} ./
cp ../Client/Assets/Plugins/NewtonsoftJson/Newtonsoft.Json.dll ./
csc /r:Core.dll /r:Network.dll /r:BattleProxy.dll /r:GameProxy.dll /r:Newtonsoft.Json.dll /target:library /out:Server.dll ./*.cs
rm -f ./*.cs

cp ../Client/Assets/Scripts/ServerConsole/ServerConsole.cs ./

chmod 777  ./*

csc /reference:Core.dll /reference:BattleProxy.dll /reference:GameProxy.dll /reference:Network.dll /reference:Server.dll *.cs

rm -f ./*.cs

cp -r ../Client/Assets/StreamingAssets/Config ./ 
find . -name '*.meta' -type f | xargs rm;
