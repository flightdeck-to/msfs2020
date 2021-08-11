rd build /s /q
mkdir build
dotnet publish FlightDeck.AddOn\FlightDeck.AddOn.csproj -c Release -r win10-x64 --self-contained
XCopy FlightDeck.AddOn\bin\Release\net5.0-windows\win10-x64\publish build\to.flightdeck.msfs2020.sdPlugin /e /h /c /i
cd build
DistributionTool.exe -b -i to.flightdeck.msfs2020.sdPlugin -o .
cd ..