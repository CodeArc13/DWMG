
Hot Key Method 1. (Old Method)

Setup your hotkeys as described in "lochotkeys.JPG",
bind them all to your WASD keys or Arrow keys or what 
ever keys you use to move. Alternatively you can just bind your forward
key and none of the other keys to reduce /loc spam in your chat window.

Hot Key Method 2. (Alternate Method, - Thanks go to Mezr for this)

Simply make a macro that has /loc in each of the 5 macro lines.
Clicking this will give you an accurate position, and heading if you are moving.

Then,

Close EverQuest, go into the EverQuest folder and find 
(by searching if needed) "eqclient.ini"
Then open it in notepad (should be default with double click on the file)
Click Edit > Find and search for log.
change the entry from log=FALSE to log=TRUE
Save the file and close it.

If you get an error while loading the mapper you will need to
download and install .Net Framework 4 minimum from Microsoft at 
http://www.microsoft.com/en-GB/download/details.aspx?id=17851
Although I recommend you get the latest .Net package your windows version 
will support, which I believe at the time of writing this is 4.5.2 for windows 7/vista 
and 4 for XP.
(if the link doesn’t work just Google for it, as it maybe outdated, of course .Net also gets 
updates through windows updates which is always the best way to get and keep it up to date.)

Log back into EverQuest

Once you can load the mapper without the above error,
DWMG will attempt to locate your EverQuest directory in one of four possible places, 
C:\Program Files (x86)\Sony\EverQuest
C:\Program Files\Sony\EverQuest
C:\Program Files (x86)EverQuest
C:\Program Files\EverQuest
If it cannot find it in any of these places it will ask you where it find it with a folder selection box.
If you have multiple installs of EverQuest you can use the "Set EQ Directory" button the on the very left of the tool bar to change it.

DWMG will then look for your log folder, if it cannot find it or any logs in it, check you have done the above edit in "eqclient.ini" 
and of course had at least one character online while EQ logs were enabled so they actually generated in the logs folder!
Now DWMG can see a working log folder it will always use the most recent log EQ has written to, meaning it will switch the log when 
you log in a new character.

When you do a /loc command in EQ DWMG will display your location and direction on the map, the more frequently you do /loc commands 
the more accurate your location and direction.

DWMG can now also switch your maps when you do a /who (or /) in EQ if you have map auto switching enabled in DWMG.  This will also 
move your characters marker in DWMG to the zone you are in if it had your zone incorrectly logged.

NOTE:- sometimes you will need to do a few /loc's before the map marker will appear
in the correct place.

If you find the loc messages are spamming your chat out make a new chat
window, (right click in the chat bar on the main window and click
New chat window, then right click on the new windows chat bar
and click Filters > Other, then move the new window out of the way, maybe
partially off screen. 

Some twisty corridors may not be completely accurate on the map but you
will still have a rough idea of where you are.  It works in Guk!

Enjoy!















