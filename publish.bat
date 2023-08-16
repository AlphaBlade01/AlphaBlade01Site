@ECHO OFF

dotnet publish -c Release
ECHO "Compiled."

ssh admin@raspberrypi "sudo rm -rf /var/www/alphablade01/*"
scp -r  ./bin/Release/net7.0/* admin@raspberrypi:/var/www/alphablade01/
ECHO "Transferred."

ssh admin@raspberrypi "sudo service alphablade01 restart"
ECHO "Restarted server."