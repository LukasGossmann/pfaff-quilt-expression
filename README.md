# PFAFF quilt expression 710 & 720
Information about the software running on PFAFF® quilt expression™ sewing machines.

**This readme is still work in progress and a lot of stuff might change in the future!**

## Disclaimer
All the information in this repository should only be used on your own risk.
If you damage or brick your sewing machine in any way i am **not** responsible.
The information below have been extracted from the scripts i was able to find in the UBIFS of the firmware update image.
Opening or modifiying the hardware of the sewing machine is not needed.

## Where to get the firmware

The current firmware for the quilt expression™ 710 and 720 sewing machines can be found on the PFAFF website.  
710 firmware: https://www.pfaff.com/de-DE/support/35148
720 firmware: https://www.pfaff.com/de-DE/support/35149  

## Extracting the firmware file

The zip file that can be downloaded there contains a `.clo` file.
This file is made up of a linux zImage and a UBIFS filesystem.

```
$ binwalk quilt_expression_720.clo

DECIMAL       HEXADECIMAL     DESCRIPTION
--------------------------------------------------------------------------------
0             0x0             Linux kernel ARM boot executable zImage (little-endian)
12725         0x31B5          gzip compressed data, maximum compression, from Unix, last modified: 2018-08-03 13:27:17
5775876       0x582204        UBIFS filesystem superblock node, CRC: 0x42918221, flags: 0x0, min I/O unit size: 2048, erase block size: 126976, erase block count: 178, max erase blocks: 2048, format version: 4, compression type: lzo
5902852       0x5A1204        UBIFS filesystem master node, CRC: 0xA0C8E616, highest inode: 1954, commit number: 0
6029828       0x5C0204        UBIFS filesystem master node, CRC: 0xACF8130B, highest inode: 1954, commit number: 0
other stuff inside the UBIFS image....
```

## Extracting the UBIFS

To extract the UBIFS first download and install ubireader: https://github.com/jrspruitt/ubi_reader

Next the UBIFS can be extracted using the following command:
```
$ ubireader_extract_files -v quilt_expression_720.clo
```
However if this fails with the following error some more steps are required as a workarround.
```
guess_start_offset Found UBIFS magic number at 5775876
guess_filetype Looking for file type at 5775876
guess_filetype File looks like a UBIFS image.
UBI_File Open Path: quilt_expression_720.clo
UBI_File File Size: 28377652
UBI_File Start Offset: 5775876
UBI_File End Offset: 5775876
UBI_File File Tail Size: 22601776
UBI_File Block Size: 3568727028
read Error: Block ends at 5775900 which is greater than file size 5775876
UBIFS Fatal: Super block error: Bad Read Offset Request
```

## Workarround when UBIFS extraction fails

This error occurs due to the start and end offset parameters not working properly.
To get around this error we simply have to split out the UBIFS part of the firmware image before trying to extract it.
Replace `UBI_File Start Offset` with the start offset that was printed in the error message (5775876 in this example).
```
$ dd if=quilt_expression_720.clo of=quilt_expression_720.ubifs skip=<UBI_File Start Offset> iflag=skip_bytes
```

Now the UBIFS can hopefully be extracted without any errors using this command:

```
$ ubireader_extract_files quilt_expression_720.ubifs
```

By default ubireader creates a directory called `ubifs-root` into which the files will be extracted.

## Extracting the zImage

To find the offset where the zImage starts I originally used binwalk. (See command output at the start of this document)
However the start offset can probably also be found by looking for some kind of magic number at the start of the gzip compressed (not tested).

```
$ dd if=quilt_expression_720.clo of=gzipData skip=12725 count=5763151 iflag=skip_bytes,count_bytes

$ file gzipData 
gzipData: gzip compressed data, last modified: Fri Aug  3 13:27:17 2018, max compression, from Unix, original size modulo 2^32 1698195043

gzip -d -k -c gzipData > uncompressedData
```

## Interresting files and directories

- This folder contains most of the custom stuff added by PFAFF: `ubifs-root/usr/share/vsm`
- Main executable: `ubifs-root/usr/share/vsm/mainnodecmp`
```
$ file usr/share/vsm/mainnodecmp 
usr/share/vsm/mainnodecmp: ELF 32-bit LSB executable, ARM, EABI5 version 1 (SYSV), dynamically linked, interpreter /lib/ld-linux.so.3, for GNU/Linux 2.6.16, stripped
```

- Co-processor firmware: `ubifs-root/usr/share/vsm/subnodecmp.bin`

- Fonts (Stitches) in SFX format: `ubifs-root/usr/share/vsm/ProjectDisk_X/System/Fonts`
- Stitch patterns in SPX format: `ubifs-root/usr/share/vsm/ProjectDisk_X/System/Stitches`
  - The names of the folders contain the category and sub-category numbers shown in the stich selection screen
  - The files in the categories are displayed in ascending order of the file name
- Scripts used to automount usb drives and to handle "development requests": `ubifs-root/lib/mdev`
- Kernel modules for network support: `ubifs-root/usr/share/vsm/usbnet.ko` and `ubifs-root/usr/share/vsm/asix.ko`
- Easteregg when the main application crashes in debug mode :) `ubifs-root/usr/share/vsm/crash.png`

Other files prefixed with vms:
```
ubifs-root/lib/mdev/vsm_load_lgpl_libraries.sh
ubifs-root/lib/mdev/vsm_quirks_mounter.sh
ubifs-root/lib/mdev/vsm_automounter.sh
ubifs-root/lib/mdev/vsm_handle_development_request.sh
ubifs-root/usr/bin/vsm_stop
ubifs-root/usr/bin/vsm_ro
ubifs-root/usr/bin/vsm_updateApplication
ubifs-root/usr/bin/vsm_setup_pointercal
ubifs-root/usr/bin/vsm_rw
ubifs-root/usr/bin/vsm_setup_usbnwk_support
ubifs-root/usr/bin/vsm_restart
ubifs-root/usr/bin/vsm_restartSubNode
ubifs-root/usr/bin/vsm_install_subnode
ubifs-root/usr/bin/vsm_start
ubifs-root/usr/bin/vsm_setup_reference_pointercal
ubifs-root/usr/bin/vsm_show_picture.sh
ubifs-root/usr/bin/vsm_install_boot
ubifs-root/usr/share/vsm/vsm_launch_application.sh
ubifs-root/usr/share/vsm/vsm_start_application.sh
ubifs-root/usr/share/vsm/vsm-sewhead-kbd.ko
ubifs-root/usr/share/vsm/vsm_uploadSubNode.sh
ubifs-root/etc/init.d/vsm_setup_system_time
ubifs-root/etc/init.d/vsm_check_development_request
ubifs-root/etc/init.d/vsm_startup
ubifs-root/etc/init.d/vsm_mdev
ubifs-root/etc/profile.d/vsm_profile.sh
ubifs-root/etc/usb_scripts.d/vsm_application-config-overrides.sh
```

## The hardware and software

Main processor: i.MX23 (ARM926EJ-S)  
RAM: 63MB  
Flash: 256MiB NAND flash made by Hynix  

Co-processor: unknown STM32  
 - Attached to the main processor on /dev/ttySP1 @ 115200 baud  

```
Linux version 2.6.31.6-rt19-626-g602af1c-dirty (jenkins@sehvavm004) (gcc version 4.4.1 (Sourcery G++ Lite 2010q1-202) ) #2 PREEMPT RT Fri Aug 3 15:27:14 CEST 2018
```
Network adapters supported by `asix.ko`:
The C version of the AX88772 works as well.
```
ASIX AX88178 USB 2.0 Ethernet
ASIX AX8817x USB 2.0 Ethernet
ASIX AX88772 USB 2.0 Ethernet
ASIX AX88772A USB 2.0 Ethernet
ASIX AX88772B USB 2.0 Ethernet
```

GPIOs used (maybe more):
```
160 LED
161 LED
162 LED
163 LED
164 LED
165 LED
166 STM32 reset
167 STM32 boot mode select
```

## Enabling SSH and dumping log files

To do this a special USB stick needs to be prepared that when automounted triggers the scripts made by PFAFF which starts the SSH daemon and copies the log files.
The folder `usb_drive` contains all files that are needed.
Just copy the contents of that folder to a FAT32 formatted usb drive and everything is ready to go.
The provided files will:
 - Dump the logs
 - Enable SSH (IP and password can be changed in `vsm_request_network_support.txt`, user is root)
 - Restart the application in "Service Lab" mode

Now the machine can be turned on (with nothing plugged into the usb port).
Wait for the machine to boot.
Next connect the network adapter and the usb drive to a usb hub and connect that to the machine.
After a few seconds drive access indicator light on the usb stick will start to flash.
Shortly after the SSH daemon will be started and after that the software will restart in Service Lab mode.

## How does it work ?

When a USB drive is inserted into the sewing machine several scripts will be executed to first mount the drive and then check for certain files on the drive.
Depending on which files are present it will trigger more stuff to happen.

The files involved in detecting this usb drive are:

- `ubifs-root/lib/mdev/vsm_quirks_mounter.sh` and `ubifs-root/lib/mdev/vsm_automounter.sh`
  - These scripts get triggered by mdev after a drive is inserted.
  - They mount the drive, make sure that there isnt currently an update in progress and then trigger more scripts
- `ubifs-root/lib/mdev/vsm_load_lgpl_libraries.sh`
  - Gets triggered by the mounter scripts above
  - Copies all *.so files from the `vsm_opensource_libs` directory on the usb drive to `/mnt/user/vsm_opensource_libs`
  - If `vsm_remove_opensource_libs.txt` is present in the `vsm_opensource_libs` directory on the usb drive all *.so files in `/mnt/user/vsm_opensource_libs` will be deleted first
- `/lib/mdev/vsm_handle_development_request.sh`
  - Gets triggered by the mounter scripts above
  - Checks for multiple files in `vsm_develop_and_debug` on the usb drive and triggers more scripts
  - Also checks for StartIn*.suf files and changes the applications startup mode


### Files used by vsm_load_lgpl_libraries.sh

```
/usb-root/
    vsm_opensource_libs/
        *.so
        vsm_remove_opensource_libs.txt
```

### Files used by vsm_handle_development_request.sh

```
/usb-root/
    vsm_develop_and_debug/
        vsm_request_logs.txt
        vsm_request_network_support.txt
        vsm_request_debug_mode.txt
        vsm_request_rootfs_rw.txt
        logs/
            ApplicationLog.txt
            versions.txt
            system-version.txt
            SystemReport*.dmp
            FailureLog.txt
            dmesg.txt
            uptime.txt
            meminfo.txt
            diskusage.txt
StartInServiceL.suf
StartInServiceP.suf
StartInService.suf
StartInProduction1.suf
StartInProduction2.suf
```

Only one of the StartIn*.suf files can be present at once!
Logs folder is created when vsm_request_logs.txt triggers its associated script.

### vsm_request_network_support.txt and /usr/bin/vsm_setup_usbnwk_support
**Warning!** This will remount the rootfs read write for a short moment.

The .txt file contains 3 lines.  
1: ip address  
2: ssh pasword  
3: yes or no if the application should be restarted or not  

```
192.168.99.123
SomePassword
yes
```
