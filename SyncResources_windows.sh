git pull
cd ~/webserver/BiangStudio/MechStormResources/windows
mv ./MechStorm_Data/GetAllFileNames.sh ./
mv ./MechStorm_Data/.downloadIgnore ./
mv ./MechStorm_Data/ServerVersion ./
rm -rf ./MechStorm_Data/
cp -r ~/GameServer/MyFirstCardGame/Client/Build/windows/MechStorm_Data/ ./ 
rm -rf ./MechStorm_Data/Resources/
rm -rf ./MechStorm_Data/StreamingAssets/AssetBundle/osx/
mv ./GetAllFileNames.sh ./MechStorm_Data/
mv ./.downloadIgnore ./MechStorm_Data/
mv ./ServerVersion ./MechStorm_Data/
cd ./MechStorm_Data/
sh ~/webserver/BiangStudio/MechStormResources/windows/MechStorm_Data/GetAllFileNames.sh
