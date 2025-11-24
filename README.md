# What is this?
This is my take on localization string editing for Star Citizen.

If you aren't familiar, it is possible to change various strings in the game by creating a file called `global.ini` in the `LIVE/Data/Localization/english/` directory, and then adding `g_language = english` to your USER.cfg file. The problem with this is that said `global.ini` file must contain ALL strings for the game, or you'll get a UI full of things like **@frontend_LearnToPlay**.

# Why not just use...?
I took inspiration for this tool from existing solutions:
* https://github.com/MrKraken/SCLocalizationMergeTool
* https://github.com/ExoAE/ScCompLangPack

So why not just use them? Well for a start, I play on linux so running their tools could prove difficult. I was also not 100% happy with their method, as it seemed quite possible to either carry mistakes forwards or leave strings out.

Instead, I decided to create a tool I could use that would pull the original localization file from the Data.p4k file, using code adapted from [unp4k](https://github.com/dolkensp/unp4k). It can then merge zero or more alteration files on top of this to create a `global.ini` file that is up-to-date and error free.

# Data sources
* Components list manually curated from the original global.ini as of Star Citizen v4.4
* Class and grade information pulled from [Erkul](https://erkul.games/) -- Do please visit them and toss them a small donation, they're doing an incredible service to the community with their ship loadout calculator.

# How do I use it?
TBA - I'll make a release that can be run from linux and windows later.

If you just want the `global.ini` file, which is up-to-date as of 4.4.0-live10753606, grab `global-annotated.ini` from the data directory of this repository and rename it.