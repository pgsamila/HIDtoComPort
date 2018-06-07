# HIDtoComPort
Human Interface Device to Com port write/ read

## How to build project
Install visual studio 2015 (or compiler).
open project and run for 1st time.
-> you will defenetely get an error for the first run, saying "HidLibrary" is missing. 
in error list section, click on "Warnings" buttons several times, and "HidLibrary" issue will get fixed. (Reason Unknown).

## How to run
You need virtual coupled serial ports.
select 1 serial port from HID to Com port app and run the app.
You can read the HID device data from the other serial port.
-> you will get HEX bytes - seperated.