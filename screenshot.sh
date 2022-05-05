#!/bin/sh

SCREEN_WIDTH=320
SCREEN_HEIGHT=240
BITS_PER_PIXEL=4
BYTES_PER_LINE=$((SCREEN_WIDTH*BITS_PER_PIXEL))

COUNTER=0

while :
do
	read -n 1 -r -p $'Press q to exit.\n' answer
	if [ "$answer" != "${answer#[q]}" ]
	then
		break
	fi
	FILENAME=$PWD/$1_$COUNTER.data
	dd bs=$BYTES_PER_LINE count=$SCREEN_HEIGHT if=/dev/fb0 of=$FILENAME
	COUNTER=$((COUNTER+1))
	echo "Saved screenshot as " $FILENAME
done
