#!/bin/bash
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

mkdir --mode=755 yoctovisualization/usr/share/icons
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/16x16
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/16x16/apps
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/32x32
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/32x32/apps
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/48x48
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/48x48/apps
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor//128x128
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor//128x128/apps
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/256x256
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/256x256/apps
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/scalable
mkdir --mode=755 yoctovisualization/usr/share/icons/hicolor/scalable/apps

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
cp ../artwork/icon.svg yoctovisualization/usr/share/icons/hicolor/scalable/apps/YoctoVisualization.svg
chmod 644 yoctovisualization/usr/share/icons/hicolor/scalable/apps/YoctoVisualization.svg
cp icon_16.png yoctovisualization/usr/share/icons/hicolor/16x16/apps/YoctoVisualization.png
chmod 644 yoctovisualization/usr/share/icons/hicolor/16x16/apps/YoctoVisualization.png
cp icon_32.png yoctovisualization/usr/share/icons/hicolor/32x32/apps/YoctoVisualization.png
chmod 644 yoctovisualization/usr/share/icons/hicolor/32x32/apps/YoctoVisualization.png
cp icon_48.png yoctovisualization/usr/share/icons/hicolor/48x48/apps/YoctoVisualization.png
chmod 644 yoctovisualization/usr/share/icons/hicolor/48x48/apps/YoctoVisualization.png
cp icon_128.png yoctovisualization/usr/share/icons/hicolor//128x128/apps/YoctoVisualization.png
chmod 644 yoctovisualization/usr/share/icons/hicolor//128x128/apps/YoctoVisualization.png
cp icon_256.png yoctovisualization/usr/share/icons/hicolor/256x256/apps/YoctoVisualization.png
chmod 644 yoctovisualization/usr/share/icons/hicolor/256x256/apps/YoctoVisualization.png
cp icon_48.png yoctovisualization/usr/share/pixmaps/YoctoVisualization.png
chmod 644 yoctovisualization/usr/share/pixmaps/YoctoVisualization.png



# copy shell script in the path
cp Yocto-Visualization yoctovisualization/usr/bin
chmod 755 yoctovisualization/usr/bin/Yocto-Visualization

#copy linux libs
cp ../libyapi-amd64.so yoctovisualization/usr/lib/Yocto-Visualization
chmod 0644 yoctovisualization/usr/lib/Yocto-Visualization/libyapi-amd64.so
cp ../libyapi-armhf.so yoctovisualization/usr/lib/Yocto-Visualization/libyapi-armhf.so
chmod 0644 yoctovisualization/usr/lib/Yocto-Visualization/libyapi-armhf.so
cp ../libyapi-i386.so yoctovisualization/usr/lib/Yocto-Visualization
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