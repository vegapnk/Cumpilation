# 1.0.0

First version. Released on an unsuspecting world.

## Mission Statement 

This mod aims to unite some Fluid mechanics from other popular RJW mods in one place. 

There were two main reasons for this: 

1) The new Fluid System allowed for more nuanced approaches, e.g. by having new effects for the new fluids. 
2) For [RJW-Genes](https://github.com/vegapnk/RJW-Genes) I often depended on other mods for single features around cum. This is a bit notorious, because the `GatheredCum` from Sexperience is a wanted item for RJW users. 

The goal is to have the mechanics on a `per-fluid`-basis, with different fluids resulting in different effects.
The cum-stuffing will make a different effect depending on the fluid, and the cum-bucket can only gather cum. Other advanced storage items can gather more fluids, resulting in different items. 

## Content Summary

- Cumflation (originally from [LicentiaLabs](https://gitgud.io/John-the-Anabaptist/licentia-labs))
- Cum-Stuffed (originally from [LicentiaLabs](https://gitgud.io/John-the-Anabaptist/licentia-labs))
- Cum-Item (originally from [Ameravashi-Sexperience](https://gitgud.io/amevarashi/rjw-sexperience))
- Cum-bucket (originally from [Ameravashi-Sexperience](https://gitgud.io/amevarashi/rjw-sexperience))
- Passive Cum-Cleaning (idea from [rjw-bucket](https://gitgud.io/Thomas404/rjw-bucket))
- Progressive Eat-((Fluid))-Thoughts (originally / partly from [Ameravashi-Sexperience](https://gitgud.io/amevarashi/rjw-sexperience))
- Gathering (originally from [Tory](https://gitgud.io/Tory/gathered-rjw), updated [by Nil](https://gitgud.io/MimiNil/gathering)
- Bukkake (Originally from [Ed86-Cum](https://gitgud.io/Ed86/rjw-cum))
- 2 Items originally from [Semen Processor](https://gitgud.io/Nalzurin/semen-processor)
- Cleansweeper Mechs slowly gather Cum and Insect Jelly
- Penis-equipped pawns will stack-up blue balls until release. Consequent releases make pawns `drained`. Vaginas get `wet`ter on orgasms.
- {Biotech} Slug, a Wastepack based, negative Fluid.

Detailed Content & Mechanics: 

### Cumflation 

- Happens on vaginal sex (and vaginal sex only)
- Adds a Hediff and thoughts when fluids are added above certain thresholds
- How much Fluid is needed for a full cumflation is defined in XML, per Fluid. 
- The Fluid-Amount is multiplied by the receivers body-size (Bodysize 2 will need 2x fluid, Bodysize 0.25 will need 1/4 of Fluid, etc)
- Over-Cumflation will result in the pawn spraying the respective fluids out
- Spraying will reduce the cumflation to severity 1.0

### Cumstuffing 

- Happens on penetrative oral and anal sex
- The receiver will get a hediff based on the fluid-type with different effects
- There is a total of "1" cum-stuffig, which can consist of different fluids. A pawn can have 0.5 of Cum-stuffing and 0.5 of Insect-jelly-Stuffing
- When a pawn is "full", and new fluid enters, the existing hediffs will be proportionally reduced. 
- How much fluid for a full stuffing is needed is defined per fluid in XML 
- The Fluid-Amount is multiplied by the receivers body-size (Bodysize 2 will need 2x fluid, Bodysize 0.25 will need 1/4 of Fluid, etc)

### Likes Cumflation 

- Will like pawns with a lot of fluid in their penetrative organs
- Will receive positive thoughts on cumflation and stuffing

### Biosculptor Cycles 

- 1 Cycle that increases ALL Fluids by 10%
- 1 Cycle that _resets_ the Fluids, this means they are re-rolled to the pawns standard.
- This will also reset all existing genes etc., i.E. if you have the very much fluid gene it will be _inactive_ and the pawn will have normal fluids.

### Fluid-Drugs

- 1 Temporary buff to milk-production
- 1 Temporary buff to cum-production
- 2 Examples for Slug (special section)

These drugs are meant to provide examples for the targetting 
and ingestion-outcome doers that maybe other modders want to use. 
I do not intent to flood this mod with drugs etc. but if you want you can copy-paste them and make them fit whatever you want. 

### Cum Gathering 

- 1 Building "Cum-Bucket" that collects Cum when pawns have non-penetrative sex nearby
- Non Penetrative Sex: Masturbation, Breastjob, Footjob, Handjob
- 1 Building "Advanced Cum Bucket" that can collect Cum & Insect Jelly
- Buckets must be in the pawns room
- Pawns will prefer to masturbate into buckets
- Location-Finding for couples is not modified
- Distance of collection can be adjusted in XML
- Amount of Fluid for one item is XML Adjustable per Fluid

This is done via xml and it's easy to extend the list of cum-gathering buildings. 
Each fluid will need 1 `FluidGatheringDef` that defines how much fluid is required for one item. 
The buildings will need 1 `Cumpilation.Gathering.PassiveFluidGathererCompProperties` that specifies the supported fluids, range, etc. 

### Cleaning Cum Gathering 

- There is a chance to receive the Cum or Insect-Jelly item upon cleaning the respective filth. 
- For 1.0.0: 1/10 Chance for Cum, 1/15 for Insect Jelly

### Passive Cum Gathering Buildings 

- The advanced cum-bucket will clean nearby cum-filth in its room
- This happens twice every hour and removes only one filth. 
- Works for Insect-Jelly and Cum
- Known Limitation: The Gatherers will loose their _progress_ upon load and save. 

This is also done via XML and can be added to any building. The xml can be configured to only have a _chance_ to clean, 
if it stops after one filth and things like range, tick-ratio, etc. 

### Cum Sweeper

- The Cleansweeper Mechs behave like "Passive Cum Gathering Buildings" but with better stats
- They still perform "normal" cleaning of cum, which might be a bit counter-intuitive to watch
- The normal cleaning follows logic from "Cleaning Cum Gathering"
- The passive cleaning does not cost any extra power for the sweeper

### Oscillation 

- Pawns with a natural penis will receive BlueBalls Hediff
- BlueBalls increase sex need, give slight pain and increase the fluid-amount until the next orgasm
- Upon orgasm, Blue Balls vanishes
- Penis-Pawns will be _drained_ after having orgasm (unless they were still blueballed). This reduces their fluid-output and sex-need until recovered. 
- Vagina-Pawns will get _wet_ after having an orgasm, increasing their fluid output and sex need. 

### Cum Items 

- Item: Cum
    - Cum can be used for cooking or eaten raw
    - As of 1.0.0: 25 Cum -> 1 Meal 
    - Cum will spoil, etc. 
- Item: Samenstein (if Semen-Processing is not loaded!)
    - Can be "cooked" at campfires and kitchens using cum
    - "Normal" building material as stone
- Item: Spermatuch (if Semen-Processing is not loaded!)
    - Can be made at Biofuel Refinery and Breweries
    - "Normal" Cloth Material 

If you happen to have Semen-Processor, these items still exist and can be made with the Mods Semen-Processor Building.
I just liked the items too much and they are too useful for my play-through. If you hate it you can just cherry pick them out :) 

### Consumption Thoughts

- Upon having Oral Sex, or eating food with the fitting ingredients, Pawns will increase two records and get thoughts
- The records are always increased, even if the setting is turned off
- Based on how much the Pawn has already eaten, it will like or dislike the consumption
- Cum Improves at 0 -> 10 -> 60 -> 120 Consumptions 
- Insect Jelly improves at 0 -> 30 -> 50 -> 90 Consumptions
- The amount is not important here, only the number how often it was consumed (e.g. 10 oral sexxes)
- Certain Quirks and Traits will automatically like the consumption, giving it the best outcome 
- These numbers, quirks and traits are configurable in XML

### Bukkake 

- Upon having non-penetrative sex, pawns might get _creamed_ by their partner
- They will get 2 types of hediffs: Bodypart "Splashes" and a central "Controller". 
- The splashes do nothing for now, but they are counted how severe the controller is. 
- The controller adds all effects, based on how much splashes are on the pawn
- Default 1.0.0: At 4.5 "Full" Splashes of Cum, the Cum-Controller is at Max
- At 3.5 Girl-Cum Splashes and at 5.5 the Insect-Spunk one
- Unlike Cumstuffing and Cumflation, a pawn can have multiple Bukkakes simultaneously
- How much fluid is required for a "full" splash is defined in the splashs hediff. Default: 10 Cum, 15 Insect Spunk, 6 Girl Cum
- The fluid-requirement scales with the targets body-size. 
- A random (fitting) body part is chosen to cream. E.g. for a footjob the feet or legs. For boob job breasts, etc.  
- To be easy on performance, the controller self-updates only every ingame hour. it can be that a pawn cleaned itself, but the controller still stays for a short time. 
- Pawns can clean splashes from themselves
- Pawns will try to find a fitting Cum-Gathering Building nearby to put their bukkake into. It must support the fluid type of the bukkake (e.g. the normal cum-bucket cant gather jelly)

### Slug 

Special fluid available when Biotech is loaded. 
Allows to transform waste-packs into drugs that change the fluid-type of a male pawn into toxic _Slug_. 
Failure to apply this transformation (e.g. Women injecting, or already slugged pawns) will result in severy toxic buildup. 
Slug-drugs still are toxic-waste and need to be treated more carefully than their input-waste. 

Slug-Effects:
- Slug-Kake makes pawns very weak against fire
- Slug-Stuffing allows for pawns to explode on death, scaling with severity and body-size
- Slug-Stuffing and Kake will slowly build up toxic buildup on the targets
- Animals can be slug-stuffed

Intended Playstyle: 

- Get Rid of Waste-packs by making drugs
- Give it to prisoners / people you kill anyway
- Give it to your pawns and build living bombs 
- Give it to your pawns if you don't have other ways of getting rid of the waste-packs
- A pawn "Slugstuffed" can be re-stuffed to remove the effects. 

See [Wiki](https://github.com/vegapnk/Cumpilation/wiki/%5BWIP%5D-Slug) for motivation and some details. 

Slug is also meant to be an example for how people can design their own fluids, apply them, and configure different follow up effects like stuffing. 