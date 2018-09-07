#!/bin/bash
cd "${0%/*}"
if [ ! -f /Library/Frameworks/Mono.framework/Versions/Current/Commands/mono32 ]; then
    osascript -e 'tell app "System Events" to display dialog "Mono is required to run this application.\n\nYou can download it from\nhttps://www.mono-project.com/download/stable"'
else
    /Library/Frameworks/Mono.framework/Versions/Current/Commands/mono32 YoctoVisualization.exe &
fi
