git pull
cd ~/webserver/BiangStudio/MechStormResources/osx
mv ./MechStorm_Data/GetAllFileNames.sh ./
mv ./MechStorm_Data/.downloadIgnore ./
mv ./MechStorm_Data/ServerVersion ./
rm -rf ./MechStorm_Data/
cp -r ~/GameServer/MechStorm/Client/Build/osx/MechStorm.app/Contents/Resources/Data/ ./ 
mv ./Data ./MechStorm_Data
rm -rf ./MechStorm_Data/StreamingAssets/AssetBundle/windows/
mv ./GetAllFileNames.sh ./MechStorm_Data/
mv ./.downloadIgnore ./MechStorm_Data/
mv ./ServerVersion ./MechStorm_Data/
cd ./MechStorm_Data/
sh ~/webserver/BiangStudio/MechStormResources/osx/MechStorm_Data/GetAllFileNames.sh
