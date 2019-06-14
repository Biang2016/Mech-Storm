git pull

rm -rf ./ServerBuild/*
cd ./ServerBuild

cp ../Client/Build/osx/MechStorm.app/Contents/Resources/Data/Managed/*.dll ./
chmod 777  ./*
find . -name "Server.dll" -exec rm -f {} \;
find . -name "Client.dll" -exec rm -f {} \;
find . -name "OutGameLogic.dll" -exec rm -f {} \;
find . -name "Assembly-CSharp.dll" -exec rm -f {} \;
find . -name "Unity*.dll" -exec rm -f {} \;
find . -name "DOTween*.dll" -exec rm -f {} \;
find . -name "TextMeshPro*.dll" -exec rm -f {} \;

cmd='csc '
for dllName in $(ls)
  do
    cmd=$cmd'/reference:"'$dllName'" '
  done

cmd=$cmd" *.cs"
echo $cmd
find ../Client/Assets/Scripts/Server -name '*.cs' -type f | xargs -i cp {} ./;
result=`$cmd`

rm -f ./*.cs

cp -r ../Client/Assets/StreamingAssets/Config ./ 
find . -name '*.meta' -type f | xargs rm;
