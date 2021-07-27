taskkill -f -t -im StreamDeck.exe -fi "status eq running"
XCOPY "." "%appdata%\Elgato\StreamDeck\Plugins\to.flightdeck.msfs2020.sdPlugin\" /S /E /Y
START /d "%ProgramW6432%\Elgato\StreamDeck" StreamDeck.exe