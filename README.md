# Nevermind all this, just give me the mod
Grab [`global.ini`](https://github.com/StelardActek/sc-localizer/raw/refs/heads/main/output/global.ini) from the `output` folder in this repository, then read the next section for what to do with it.

# What is this?
This is my take on localization string editing for Star Citizen.

If you aren't familiar, it is possible to change various strings in the game by creating a file called `global.ini` in the `LIVE/Data/Localization/english/` directory, and then adding `g_language = english` to your `USER.cfg` file. The problem with this is that said `global.ini` file must contain ALL strings for the game, or you'll get a UI full of things like **@frontend_LearnToPlay**.

# Why would I want that?
It allows you to include useful information such as the class and grade of a ship component in the name, allowing you to know at a glance if it is worth looting. At time of writing, my intention is to do this for example:

`XL-1` → `XL-1 [Mil A]`

But I might also include a general suggestion on the value of mineables and harvestables. E.g. `Corundum [Low Value]`

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
You'll need to download either `sc-localizer-linux` or `sc-localizer-win.exe` depending on your OS. Then run it from the terminal with these parameters.

| Argument | Description |
| -------- | ----------- |
| -b <path> | (Required) The path to the base strings file. This can be either your Data.p4k file, if you want to start from the latest `global.ini` strings (recommended); Or, the path to an existing `global.ini` file you want to update. |
| -m <path> | (Optional, repeatable) The path to a `.ini` file you wish to replace the values in the the base strings file. You can provide multiple `-m` arguments, each with its own path, and they will be evaluated in order. So put the most important file last. |
| -l <lang> | (Optional, defaults to 'english') I haven't tested other languages, but in theory it should work. You only need to specify this if you are providing your Data.p4k file for the `-b` option. At time of writing, valid options for `-l` include: `polish_(poland)`, `korean_(south_korea)`, `chinese_(traditional)`, `chinese_(simplified)`, `spanish_(latin_america)`, `spanish_(spain)`, `german_(germany)`, `italian_(italy)`, `japanese_(japan)`, `portuguese_(brazil)`, `french_(france)`, and `english`. |
| -o <path> | (Required) The full path including filename of where you would like the output written. WILL OVERWRITE WITHOUT PROMPTING. |

E.g.
`sc-localizer-linux -b ~/Games/star-citizen/StarCitizen/LIVE/Data.p4k -m ./data/components-annotated.ini -m ./data/commodities-annotated.ini -o ./output/global.ini`
