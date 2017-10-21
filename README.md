# osu-HitsoundGenerator
Hitsound generator from a template tool for mappers

Sometimes hitsounding is repetetive and tedious. Like, in dnb you often want to add Drum+Percussion on every 2 adjancent downbeats in the whole map. Going one-by-one is time consuming, not fun and thus prone to misses. The program allows to add hitsounds based on a template to a specified time segment on the existing map.

How to use:  
1) Open the app
2) File -> Open .osu
3) Hitsounds -> Add a hitsounding meta
4) Encode hitsound template sequence with F for Finish, W for Whistle, C for Clap, any other symbol for skip (for example, _)
5) Specify the segment with offsets. You can copy objects from the editor and click "Paste" to paste their timing into the textbox.
6) Input the new difficulty name into the textfield. **NOTE!** It is not safe to ovveride your own .osu file with the generated one, always do backup or save the generated map in a new file. I am not responsible for your map corruption.
Example: "FC" with 1/1 fraction will do your Drum+Percussion on every 2 adjancent downbeats.
7) Beatmap -> Generate beatmap
8) Profit!