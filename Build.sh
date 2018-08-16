git pull

rm -rf ./ServerBuild/*
cd ./ServerBuild

find ../Common -name '*.cs' -type f | xargs -i cp {} ./
csc /target:library /out:MyCardGameCommon.dll ./*.cs
rm -f ./*.cs

find ../Server -name '*.cs' -type f | xargs -i cp {} ./
csc /reference:"MyCardGameCommon.dll" ./*.cs 
rm -f ./*.cs

cp -r ../Client/Assets/StreamingAssets/Config ./ 
find . -name '*.meta' -type f | xargs rm


