git pull

rm -rf ./ServerBuild/*
find ./Server -name '*.cs' -type f | xargs -i cp {} ./ServerBuild
find ./Common -name '*.cs' -type f | xargs -i cp {} ./ServerBuild
cp -r ./Client/Assets/StreamingAssets/Config ./ServerBuild/ 
find ./ServerBuild -name '*.meta' -type f | xargs rm

cd ./ServerBuild
csc *.cs
rm -rf *.cs


