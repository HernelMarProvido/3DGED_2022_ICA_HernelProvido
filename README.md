# 2022 3DGED GDApp & GD Engine

## Overview ##
This repository contains code for the game engine


## Table of Necessary Knowledge (TONK) ##
| Topic | See (Source Code) | Additional Reading |
| :---------------- | :--------------- | :--------------- |
| Encapsulating translation, rotation, and scale of an object|Transform|None|
| Encapsulating View and Projection |Camera|[Basic Matrices](http://rbwhitaker.wikidot.com/monogame-basic-matrices)|
| Representing drawn objects and non-drawn objects |GameObject, Component|None|
| Encapsulating vertex and index data |Mesh, Renderer|[Rendering from Vertex and Index Buffers](https://learn.microsoft.com/en-us/windows/win32/direct3d9/rendering-from-vertex-and-index-buffers)|
| Supporting the addition of components to a GameObject |GameObject, Component|None|
| Storing 3D rendered object's surface properties in a material | Material |None|
| Rendering FBX models and integrating into Mesh hierarchy |Mesh, ModelMesh|None|
| Using AppData to remove hard-coded magic numbers from our code |AppData|(Static Classes & Static Members)[https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members]|
| Adding behaviours (no user-input) and controllers (user-input) as components of a GameObject |RotationBehaviour, FirstPersonController |None|
| Storing drawn GameObjects in a list in the Scene class |Scene |[C# - List](https://www.tutorialsteacher.com/csharp/csharp-list)|
| Storing static (i.e. exist for all of gameplay) and dynamic (i.e. added/removed during gameplay) drawn GameObjects in a GameObjectList |GameObjectList, ObjectType |None|
| Using enums to de-mystify crytic parameters |ObjectType, RenderType |(C# - Enums)[https://www.tutorialsteacher.com/csharp/csharp-enum]|
| Re-factoring Scene to store opaque and transparent GameObjectLists |Scene, RenderType |None|
| Adding utility and extension classes to include useful secondary functionality (e.g. Vector3::Round) |Vector3Extension, PerfUtility, SerializationUtility |None|
| Adding support for multiple scenes and multiple cameras | CameraManager, SceneManager |None|
| Create behaviours (i.e. GameObject components) to add simple camera types | SecurityCameraBehaviour, CurveBehaviour, Curve|None|
| Add Integer2 to demonstrate operator overloading | Integer2|[C# - Operator Overloading](https://www.tutorialspoint.com/csharp/csharp_operator_overloading.htm)|


### Further Reading
- Generating [Gitignore](https://www.toptal.com/developers/gitignore/) files
- Using Gitignore to [remove files/folders](https://linuxize.com/post/gitignore-ignoring-files-in-git/) from upload
- Common screen [resolutions](https://en.wikipedia.org/wiki/Display_resolution#/media/File:Vector_Video_Standards8.svg)
- Using the [out](https://www.c-sharpcorner.com/article/out-parameter-in-c-sharp-7/) keyword
- Using an [Action](https://www.tutorialsteacher.com/csharp/csharp-action-delegate), [Predicate](https://www.tutorialsteacher.com/csharp/csharp-predicate) and [Delegate](https://www.tutorialsteacher.com/csharp/csharp-delegates)
- Using a [pragma](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-pragma) 
- Added [IEnumerable](https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/make-class-foreach-statement) support to a class with a container to support foreach()
- Iterating through a [dictionary](https://robertgreiner.com/iterating-through-a-dictionary-in-csharp/) (e.g., used in SoundManager\:\:Dispose() and with simple/crude implementation in ContentDictionary\:\:Dispose())
- Creating a [sealed](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed) class (e.g., used in Cue used by SoundManager)
- Understanding [Platonic Solids](http://www.technologyuk.net/mathematics/geometry/platonic-solids.shtml) when defining your primitives from scratch
![sdfsdf](http://www.technologyuk.net/mathematics/geometry/images/geometry_0185.gif)


### Add Content To Table of Necessary Knowledge
- None

### Class Exercises
- [x] Add Integer2 to demo operator overloading in C# and add [expression bodied members](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members)
- [ ] Add ThirdPersonController to also demo use of FirstPersonController

### Demo/Comment
- [x] Demo [git emojis](https://gitmoji.dev/) for begin, feature, bug, fix, refactor, remove
- [x] Relationship between world scale and movement speed
- [x] Demo PerfUtility updates re SpriteBatchInfo
- [x] Demo FirstPersonController
- [x] Demo SoundManager
- [ ] Demo EventDispatcher
- [ ] [Sound Assets - Big Sound Bank](https://bigsoundbank.com/)

### Bugs (MUST)
- [x] Fix depth problem on draw
- [x] Fix FirstPersonController rotation
- [x] Fix GameObject::GetComponent<>() - see FOVCameraController
- [x] PerfUtility is not updating FPS - Fixed - base.Update() was accidentally removed
- [ ] Fix CycledTranslationBehaviour

### Optimize (SHOULD)
- [ ] Add event notification on Transform change

### Possible Improvements (COULD)
- [ ] Add InputDevice to separate input from keyboard or gamepad and allow mappings

### To Do - Week 5
- [x] Explain two project setup (one app, one engine)
- [x] Explain Camera, Transform, Component
- [x] Explain abstract classes and methods
- [x] Explain Mesh
- [x] Explain GameObject, Component
- [x] Create QuadMesh
- [x] Explain Material, IEffect, Renderer
- [x] Add RotationBehaviour on Quad
- [x] Add simple (first pass) FirstPersonCameraController for camera
- [x] Added AppData to abstract and centralise hard-coded magic numbers and improve readability
- [x] Added skybox and grass plane
- [x] Tidied Main::Initialize
- [x] Added SamplerState setting to remove artifacts between skybox planes
- [x] Explain how to extend a class (see Extensions)
- [x] Finish CubeMesh

### To Do - Week 6
- [x] Added Scene to store List<GameObject>
- [x] Added GameObjectList to support splitting GameObjects into two lists - static (persists for game duration) and dynamic (add/remove during gameplay)
- [x] Refactored Scene to support opaque and transparent GameObjectLists
- [x] Added ObjectType and RenderType enums to GameObject to prevent passing in two hard-to-understand booleans
- [x] Added Add, Remove, Find, Size, Clear to Scene and GameObjectList
- [x] Added ModelMesh to support loading FBX meshes
- [x] Refactored Mesh, CubeMesh, QuadMesh and ModelMesh to solve constructor problem with model == null
- [x] Added SceneManager to support switching between scenes
- [x] Added Color and Viewport extensions for future use
- [x] Added Perf, Serialization, and String -Utility classes
- [x] Added ContentDictionary
- [x] Added methods in Scene to support transparent game objects
- [x] Added Curve classes for camera controller

### To Do - Week 7
- [x] Practice extension of a class (see Extensions)
- [x] Add Integer2 for use with screen resolution to prevent need to typecast - see InitializeGraphics()
- [x] Refactor RotationBehaviour 
- [x] Added pre-processor directives (i.e. #if...) to set HIRES and DEMO in Main and AppData
- [x] Re-added GameObject::RemoveComponent<T>() to support removal of components by type
- [x] Added Resolutions to clarify and standardize setting screen resolutions
- [x] Added Rail for RailBehaviour
- [x] Added CameraManager::ActiveCameraName for use in PerfUtility
- [x] Added ILoadLevel interface in preparation for moving level specific code out of Main
- [x] Added TestUpdateableOnlyComponent and TestUpdateableDrawableComponent to demo Component concept
- [x] Re-factor SecurityCameraBehaviour to add axis and re-name since it can be applied to any GameObject
- [x] Add CameraFOVController with validation on Camera::FieldOfView setter to ensure its never set <= zero
- [x] Demo SoundManager
- [x] Add camera types
- [x] Added GetPerfStats to Scene and GameObjectList
- [x] Improved PerfUtility to support component based information elements
- [x] Reduced font size on PerfUtility - it was a bit big - do this by opening the spritefont file in MGCB, setting Size in the XML file and re-building in MGCB
- [x] Refactor SecurityCameraBehaviour
- [x] Changed boom sound to play on B and not S key as it clashed with WASD for camera

### To Do - Week 8
- [x] Finish 1st Person Camera
- [x] Add CameraProjectionType and Viewport to Camera
- [x] Add ThirdPersonController
- [x] Add RenderManager to render Scene data and remove Draw from Scene
- [x] Convert SceneManager and CameraManager to inherit from DrawableGameComponent and GameComponent 
- [x] Add Camera::Viewport
- [x] Add support for Material alpha and diffuse color
- [x] Increase decimal precision on Camera::rotation in perf utility to 2 decimal places to support accurate curve creation
- [x] Added GDEvent
- [x] Add basic DriveController

### To Do - Week 9
- [x] Add RailController
- [x] Add MathUtility with Sin related functions
- [x] Re-factor CurveBehaviour to support generic Action
- [x] Add AudioEmitter and AudioListener behaviours

### To Do - Week 10
- [x] Add CurveRecorderController
- [x] Add support code for physics
- [x] Re-factor Transform to conform with Physics Collider class
- [x] Create renderers list in GameObjectList to optimize obtaining renderer in SceneRenderer::Draw()
- [x] Add clean/dirty flag to Transform on change
- [x] Add camera smoothing using Vector2.Lerp in FPC
- [x] Add CD/CR demo for box, sphere, trianglemesh
- [x] Add StatusType enum to support turning Update and Draw on/off during gameplay (hint: use when menu is shown) for specific GameObject and Scene
- [x] Add core code for Menu (Scene2D, SpriteRenderer, SpriteMaterial, TextSpriteMaterial) for UI and menu
- [ ] Give ICA document - Wk 10

### To Do - Week 11
- [x] Collidable Camera

### To Do - Week 12
- [x] Add UI behaviours and controllers - UIProgressBarController, UIColorFlipOnTimeBehaviour, UIScaleBehaviour
- [ ] Collect object => Value => Inventory
- [ ] Pick objects with mouse
- [ ] Add fully working Audio 3D demo
- [ ] Add support for cloning GameObjects, Camera
- [ ] Move level-specific methods to Level class
- [ ] Add clean/dirty flag to Camera on change
- [ ] Change List to SortedList in GameObjectList to sort by material and reduce rendertime
- [ ] Lighting deferred (large numbers of point/spot)
- [ ] Other? Water? Normal mapping?


