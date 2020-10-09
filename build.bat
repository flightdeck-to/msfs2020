rd build /s /q
mkdir build
dotnet publish FlightDeck.AddOn\FlightDeck.AddOn.csproj -c Release -r win10-x64 --self-contained
XCopy FlightDeck.AddOn\bin\Release\netcoreapp3.1\win10-x64\publish build\com.flightdeck.msfs2020.sdPlugin /e /h /c /i
cd build
DistributionTool.exe -b -i com.flightdeck.msfs2020.sdPlugin -o .
cd ..