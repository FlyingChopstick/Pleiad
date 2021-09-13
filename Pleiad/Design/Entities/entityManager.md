# On creation, storage and management of entities

## Terminology
- **Entity** - a structure containing an ID, used as a label for organisation of component data;
- **Component** - a structure containing some user-defined fields;
- **System** - a structure that executes user-defined action every cycle. It can be an action on entities with a specified *component*(s) or not;
- **Data** - *component* data of some type;
- **[Chunk](./entityChunk.md)** - structure that contains entity IDs and entity *data*;
- **Open chunk** - a chunk with at least 1 empty slot for entity;
- **Entity data** - *data*, associated with the specified *entity*;
- **Entity chunk** - *chunk* that contains *entity data*;
- **Entity template**, **template** - a collection of *components* and component data, that is used to create a new *entity*.

## Assumptions
- *Entities* are just IDs - they don't know about their *components*, no need to update them;
- *Chunks* are never deleted - no need to maintain lookups, any stored *chunk* index leads to a valid *chunk*;
- *Chunks* are never reorganised - if you place an *entity* in *chunk* N, you should be able to retrieve it from *chunk* N at any moment in the future;
- *Chunk* size is constant - can use fixed arrays internally, possible future low-level optimisation.

## Basic required functionality and workflows
- **Entity creation**
	1. **FOR EACH** *component* in *template*:
		1. **FIND OR ADD** a *chunk* for every type in *template*
		2. **SET** *entity data* in  *entity chunk* to *template* data		
- **Entity removal**
	1. **FOR EACH** *entity chunk*:
		1.  **REMOVE** *entity data*
- **Entity component addition**
	1. **FIND OR ADD** a *chunk* for new *component*
	2. **SET** *entity data*
- **Entity component removal**
	1. **FIND** *entity chunk* of *component* type
	2. **REMOVE** *entity data*
- **Entity component data retrieval**
	1. **FIND** *entity chunk* of *component* type
	2. **GET** *entity data* from *entity chunk*
- **Entity component data update**
	1. **FIND** *entity chunk* of *component* type
	2. **SET** *entity data*

## Possible optimisation
- Map *entity* -> *component* list. Simplifies **entity removal** process;
- Map Type -> *chunk* index. Simplifies queries;
- Store *open chunks* indices in queue to avoid linear checks.
