# On entity chunks

## Terminology
- **Entity** - a structure containing an ID, used as a label for organisation of component data;
- **Data** - *component* data of some type;
- **Chunk** - structure that contains entity IDs and entity *data*;
- **Open chunk** - a chunk with at least 1 empty slot for entity;
- **Entity data** - *data*, associated with the specified *entity*;
- **Entity chunk** - *chunk* that contains *entity data*;

## Assumptions
- *Chunk* size is defined on creation and does not change;
- *Chunk* maintains it's bool IsOpen value, but it should not be used for linear search on main chunk list in [Entity Manager](./entityManager.md);

## Basic required functionality and workflows
- **Add entity to the chunk**
	1. **FIND** an empty slot
	2. **SET** *entity* id and *entity data*
	3. **UPDATE AND RETURN** IsOpen
- **Remove entity from the chunk**
	1. **CHECK** that *chunk* contains this *entity*
	2. **REMOVE** *entity* id and *entity data*
	3. **UPDATE AND RETURN** IsOpen
- **Get *entity data***
	1. **CHECK** that *chunk* contains this *entity*
	2. **RETURN** *entity data*
- **Set *entity data***
	1. **CHECK** that *chunk* contains this *entity*
	2. **SET** *entity data*
	
## Possible optimisation
- Maintain a queue of empty slot indices