@ECHO OFF

dotnet publish -c Release
ECHO Compiled

scp -r  ./bin/Release/net7.0/publish/* admin@raspberrypi:/var/www/alphablade01/
ECHO Transferred

ssh admin@raspberrypi "sudo service alphablade01 restart"
ECHO Restarted server