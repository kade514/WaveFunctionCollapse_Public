# WaveFunctionCollapse_Public

A public-facing repo for my wave function collapse implementation.

This was an experiment in implementing a wave function collapse algorithm into a Hex-based world that got a little out of hand. I initially just wanted to see if I could do it but then started adding features inspired by Sid Mier's Alpha Centauri. While this implementation is a purely 2D hex-based thing and uses grid coordinates to work, the method could be adapted to something like an FPS with less effort than you'd think. 

## Minimum Unity Version 2021.3
Note: The actual wave function collapse code and 99% of the other scripts can run on earlier versions of Unity. The only thing that requires a later version of Unity is Unity's Mathematics package that I used for altitude manipulation

## Version 1.0.0


## Features

- Hex Grid Generation
	- Not an amazing implementation of this since a few values are hardcoded, but it's a good starting place for anyone who wants to make hex-grid maps in Unity.
	- Includes West-East world wrapping.

- Wave Function Collapse terrain Generation
	- Uses the methodology showcased in [this video by Martin Donald](https://www.youtube.com/watch?v=2SuvO4Gi7uY) to collapse the possible states of tiles until they reach a final state.
	- Probabilities are controlled using weighted-random percentages to give you fine control over what percentage of the hexes are what type of terrain.
	- Can support an arbitrary number of tile types so long as the relationships between the types are defined.

- Tile Modifiers
	- Tiles track temperature, elevation, and moisture.
	- Tiles also have flags tracking what modifications have been made to them (I.E if they're rainy, if they have a river in them, if they're a mountain tile, etc).


- Terrain generation post-processing
	- Set tile altitude using worley noise.
	- Generate mountain chains.
	- Set tile temperature based on distance from the equator.
	- Generate rivers starting from the mountains and flowing towards lowland areas.
	- Basic rainfall/rainshadow simulation using West-East wind.


## TODO/Desired Features

- Visualise tile altitude
	- Either by manipulating vertices via code or using some kind of distance field, IDK.
- Triplanar shader for terrain
- Adjust tile visuals depending on neighbouring tiles
- Adjust tile visuals based on their modifiers (E.G cold forest tiles should show different trees/trees with snow on them)
