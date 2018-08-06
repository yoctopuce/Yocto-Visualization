## builds linux binary for local platform
## requires installation of
## mono-devel   (sudo apt-get install mono-devel)
## monodevelop  (sudo apt-get install monodevelop)


sudo rm -rf yoctovisualization

mkdir --mode=755 yoctovisualization/
mkdir --mode=755 yoctovisualization/usr/
mkdir --mode=755 yoctovisualization/usr/lib/
mkdir --mode=755 yoctovisualization/usr/lib/Yocto-Visualization

mkdir --mode=755 yoctovisualization/usr/bin/

mkdir --mode=755 yoctovisualization/usr/share/
mkdir --mode=755 yoctovisualization/usr/share/applications
mkdir --mode=755 yoctovisualization/usr/share/pixmaps
mkdir --mode=755 yoctovisualization/DEBIAN


mkdir --mode=755 yoctovisualization/usr/share/doc
mkdir --mode=755 yoctovisualization/usr/share/doc/yoctovisualization


#mkdir yoctovisualization/etc
#mkdir yocto-visualization/etc/udev
#mkdir yocto-visualization/etc/rules.d

#cp 51-yoctopuce_all.rules yocto-visualization/etc/udev/rules.d
#chmod -R 644 yocto-visualization/etc


#copy copyright
cp copyright yoctovisualization/usr/share/doc/yoctovisualization
chmod 644 yoctovisualization/usr/share/doc/yoctovisualization/copyright
cp changelog yoctovisualization/usr/share/doc/yoctovisualization
gzip -n -9 yoctovisualization/usr/share/doc/yoctovisualization/changelog
chmod 644 yoctovisualization/usr/share/doc/yoctovisualization/changelog.gz

#copy debian control filE
cp control yoctovisualization/DEBIAN
#cp conffiles yoctovisualization/DEBIAN
chmod 644 yoctovisualization/DEBIAN/*

#copy freedesktop stuff
cp YoctoVisualization.desktop yoctovisualization/usr/share/applications
chmod 644 yoctovisualization/usr/share/applications/YoctoVisualization.desktop
cp YoctoVisualization.png yoctovisualization/usr/share/pixmaps
chmod 644 yoctovisualization/usr/share/pixmaps/YoctoVisualization.png


# copy shell script in the path
cp Yocto-Visualization yoctovisualization/usr/bin
chmod 755 yoctovisualization/usr/bin/Yocto-Visualization

#copy linux libs
#strip -S -o yoctovisualization/usr/lib/Yocto-Visualization/libyapi-amd64.so libyapi-amd64.so
cp libyapi-amd64.so yoctovisualization/usr/lib/Yocto-Visualization
chmod 0644 yoctovisualization/usr/lib/Yocto-Visualization/libyapi-amd64.so
chmod 0644 libyapi-armhf.so
#strip -S -o yoctovisualization/usr/lib/libyapi-armhf.so libyapi-armhf.so
cp libyapi-armhf.so yoctovisualization/usr/lib/Yocto-Visualization/libyapi-armhf.so
#strip -S -o yoctovisualization/usr/lib/Yocto-Visualization/libyapi-i386.so libyapi-i386.so
cp libyapi-i386.so yoctovisualization/usr/lib/Yocto-Visualization
chmod 0644 yoctovisualization/usr/lib/Yocto-Visualization/libyapi-i386.so

#copy binary
cp  ../bin/Release/YoctoVisualization.exe yoctovisualization/usr/lib/Yocto-Visualization
cp  YoctoVisualization.exe.config yoctovisualization/usr/lib/Yocto-Visualization
chmod 755 yoctovisualization/usr/lib/Yocto-Visualization/YoctoVisualization.exe
chmod 644 yoctovisualization/usr/lib/Yocto-Visualization/YoctoVisualization.exe.config

#set all file to root user
sudo chown -R root:root yoctovisualization


dpkg-deb --build yoctovisualization


lintian yoctovisualization.deb