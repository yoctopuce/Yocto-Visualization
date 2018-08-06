## builds linux binary for local platform
## requires installation of
## mono-devel   (sudo apt-get install mono-devel)
## monodevelop  (sudo apt-get install monodevelop)

MACHINE_TYPE=`uname -m`
LIB_SUFIX="i386"


if [[ "$MACHINE_TYPE" == *"i686"* ]]; then
  MACHINE_TYPE="x86_32"
  LIB_SUFIX="i386"
fi

mkdir -p linuxBinaries/linux-${MACHINE_TYPE}

if [[ "$MACHINE_TYPE" == *"x86_64"* ]]; then
	LIB_SUFIX="amd64"
fi

if [[ "$MACHINE_TYPE" == *"armv"* ]]; then
  LIB_SUFIX="armhf"
fi

mdtool build --configuration:Release YoctoVisualization.csproj

## mkbundle --static --deps -o linuxBinaries/linux-${MACHINE_TYPE}/YoctoVisualization   --config dllmap.config bin/Release/YoctoVisualization.exe
mkbundle -v -o  linuxBinaries/linux-${MACHINE_TYPE}/YoctoVisualization --simple   bin/Release/YoctoVisualization.exe --library libyapi.so,./libyapi-${LIB_SUFIX}.so  --machine-config /etc/mono/4.5/machine.config

echo "binary saved in linuxBinaries/linux-${MACHINE_TYPE}"