rd build /s /q
mkdir build
dotnet publish FlightDeck.AddOn\FlightDeck.AddOn.csproj -c Debug -r win10-x64 --self-contained
XCopy FlightDeck.AddOn\bin\Debug\netcoreapp3.1\win10-x64\publish build\com.flightdeck.msfs2020.sdPlugin /e /h /c /i
cd build
cd ..