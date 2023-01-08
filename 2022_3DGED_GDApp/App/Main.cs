#region Pre-compiler directives

#define DEMO
#define SHOW_DEBUG_INFO

#endregion

using App.Managers;
using GD.Core;
using GD.Engine;
using GD.Engine.Events;
using GD.Engine.Globals;
using GD.Engine.Inputs;
using GD.Engine.Managers;
using GD.Engine.Parameters;
using GD.Engine.Utilities;
using JigLibX.Collision;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Application = GD.Engine.Globals.Application;
using Cue = GD.Engine.Managers.Cue;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace GD.App
{
    public class Main : Game
    {
        #region Fields

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicEffect unlitEffect;
        private BasicEffect litEffect;

        private CameraManager cameraManager;
        private SceneManager<Scene> sceneManager;
        private SoundManager soundManager;
        private PhysicsManager physicsManager;
        private RenderManager renderManager;
        private EventDispatcher eventDispatcher;
        private GameObject playerGameObject;
        private PickingManager pickingManager;
        private MyStateManager stateManager;
        private SceneManager<Scene2D> uiManager;
        private SceneManager<Scene2D> menuManager;

        public Vector3 rOne = new Vector3(0, 3, -25);
        public Vector3 rTwo = new Vector3(0, 3, -75);
        public Vector3 rThree = new Vector3(0, 3, -125);

#if DEMO

        private event EventHandler OnChanged;

#endif

        #endregion Fields

        #region Constructors

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        #endregion Constructors

        #region Actions - Initialize

#if DEMO

        private void DemoCode()
        {
            //shows how we can create an event, register for it, and raise it in Main::Update() on Keys.E press
            DemoEvent();

            //shows us how to listen to a specific event
            DemoStateManagerEvent();

            Demo3DSoundTree();
        }

        private void Demo3DSoundTree()
        {
            //var camera = Application.CameraManager.ActiveCamera.AudioListener;
            //var audioEmitter = //get tree, get emitterbehaviour, get audio emitter

            //object[] parameters = {"sound name", audioListener, audioEmitter};

            //EventDispatcher.Raise(new EventData(EventCategoryType.Sound,
            //    EventActionType.OnPlay3D, parameters));

            //throw new NotImplementedException();
        }

        private void DemoStateManagerEvent()
        {
            EventDispatcher.Subscribe(EventCategoryType.Menu, HandleEvent);
            EventDispatcher.Subscribe(EventCategoryType.Player, HandleEvent);
            EventDispatcher.Subscribe(EventCategoryType.GameObject, HandleEvent);
        }

        private void HandleEvent(EventData eventData)
        {
            switch (eventData.EventActionType)
            {
                case EventActionType.OnWin:
                    System.Diagnostics.Debug.WriteLine(eventData.Parameters[0] as string);
                    break;

                case EventActionType.OnLose:
                    System.Diagnostics.Debug.WriteLine(eventData.Parameters[2] as string);
                    break;

                default:
                    break;
            }
        }

        private void DemoEvent()
        {
            OnChanged += HandleOnChanged;
        }

        private void HandleOnChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{e} was sent by {sender}");
        }

#endif

        protected override void Initialize()
        {
            //moved spritebatch initialization here because we need it in InitializeDebug() below
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //core engine - common across any game
            InitializeEngine(AppData.APP_RESOLUTION, true, true);

            //game specific content
            InitializeLevel("My Amazing Game", AppData.SKYBOX_WORLD_SCALE);

#if SHOW_DEBUG_INFO
            InitializeDebug();
#endif

#if DEMO
            DemoCode();
#endif

            base.Initialize();
        }

        #endregion Actions - Initialize

        #region Actions - Level Specific

        protected override void LoadContent()
        {
            //moved spritebatch initialization to Main::Initialize() because we need it in InitializeDebug()
            //_spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        private void InitializeLevel(string title, float worldScale)
        {
            //set game title
            SetTitle(title);

            //load sounds, textures, models etc
            LoadMediaAssets();

            //initialize curves used by cameras
            InitializeCurves();

            //initialize rails used by cameras
            InitializeRails();

            //add scene manager and starting scenes
            InitializeScenes();

            //add collidable drawn stuff
            InitializeCollidableContent(worldScale);

            //add non-collidable drawn stuff
            InitializeNonCollidableContent(worldScale);

            //add the player
            //InitializePlayer();

            //add UI and menu
            InitializeUI();
            InitializeMenu();

            //send all initial events

            #region Start Events - Menu etc

            //start the game paused
            EventDispatcher.Raise(new EventData(EventCategoryType.Menu, EventActionType.OnPause));

            #endregion
        }

        private void InitializeMenu()
        {
            GameObject menuGameObject = null;
            Material2D material = null;
            Renderer2D renderer2D = null;
            Texture2D btnTexture = Content.Load<Texture2D>("Assets/Textures/Menu/Controls/genericbtn");
            Texture2D backGroundtexture = Content.Load<Texture2D>("Assets/Textures/Menu/Backgrounds/BG");
            SpriteFont spriteFont = Content.Load<SpriteFont>("Assets/Fonts/menu");
            Vector2 btnScale = new Vector2(0.8f, 0.8f);

            #region Create new menu scene

            //add new main menu scene
            var mainMenuScene = new Scene2D("main menu");

            #endregion

            #region Add Background Texture

            menuGameObject = new GameObject("background");
            var scaleToWindow = _graphics.GetScaleFactorForResolution(backGroundtexture, Vector2.Zero);
            //set transform
            menuGameObject.Transform = new Transform(
                new Vector3(0.43f, 0.44f, 0.43f), //s
                new Vector3(0, 0, 0), //r
                new Vector3(0, 0, 0)); //t

            #region texture

            //material and renderer
            material = new TextureMaterial2D(backGroundtexture, Color.Gray, 1);
            menuGameObject.AddComponent(new Renderer2D(material));

            #endregion

            //add to scene2D
            mainMenuScene.Add(menuGameObject);

            #endregion

            #region Add Play button and text

            menuGameObject = new GameObject("play");
            menuGameObject.Transform = new Transform(
            new Vector3(btnScale, 1), //s
            new Vector3(0, 0, 0), //r
            new Vector3(Application.Screen.ScreenCentre - btnScale * btnTexture.GetCenter() - new Vector2(0, 30), 0)); //t

            #region texture

            //material and renderer
            material = new TextureMaterial2D(btnTexture, Color.Green, 0.9f);
            //add renderer to draw the texture
            renderer2D = new Renderer2D(material);
            //add renderer as a component
            menuGameObject.AddComponent(renderer2D);

            #endregion

            #region collider

            //add bounding box for mouse collisions using the renderer for the texture (which will automatically correctly size the bounding box for mouse interactions)
            var buttonCollider2D = new ButtonCollider2D(menuGameObject, renderer2D);
            //add any events on MouseButton (e.g. Left, Right, Hover)
            buttonCollider2D.AddEvent(MouseButton.Left, new EventData(EventCategoryType.Menu, EventActionType.OnPlay));
            menuGameObject.AddComponent(buttonCollider2D);

            #endregion

            #region text

            //material and renderer
            material = new TextMaterial2D(spriteFont, "Let's Play", new Vector2(30, 5), Color.Silver, 0.8f);
            //add renderer to draw the text
            renderer2D = new Renderer2D(material);
            menuGameObject.AddComponent(renderer2D);

            #endregion

            //add to scene2D
            mainMenuScene.Add(menuGameObject);

            #endregion

         
            #region Add Scene to Manager and Set Active

            //add scene2D to menu manager
            menuManager.Add(mainMenuScene.ID, mainMenuScene);

            //what menu do i see first?
            menuManager.SetActiveScene(mainMenuScene.ID);

            #endregion
        }

        private void InitializeUI()
        {
            GameObject uiGameObject = null;
            Material2D material = null;
            Texture2D texture = Content.Load<Texture2D>("Assets/Textures/Menu/Controls/progress_white");

            var mainHUD = new Scene2D("game HUD");

            #region Add UI Element

            uiGameObject = new GameObject("progress bar - health - 1");
            uiGameObject.Transform = new Transform(
                new Vector3(1, 1, 0), //s
                new Vector3(0, 0, 0), //r
                new Vector3(_graphics.PreferredBackBufferWidth - texture.Width - 20,
                20, 0)); //t

            #region texture

            //material and renderer
            material = new TextureMaterial2D(texture, Color.White);
            uiGameObject.AddComponent(new Renderer2D(material));

            #endregion

            #region progress controller

            uiGameObject.AddComponent(new UIProgressBarController(0, 10));

            #endregion

            #region color change behaviour

            uiGameObject.AddComponent(new UIColorFlipOnTimeBehaviour(Color.White, Color.Green, 500));

            #endregion

            //add to scene2D
            mainHUD.Add(uiGameObject);

            #endregion

            #region Add Scene to Manager and Set Active

            //add scene2D to manager
            uiManager.Add(mainHUD.ID, mainHUD);

            //what ui do i see first?
            uiManager.SetActiveScene(mainHUD.ID);

            #endregion
        }

        private void SetTitle(string title)
        {
            Window.Title = title.Trim();
        }

        private void LoadMediaAssets()
        {
            //sounds, models, textures
            LoadSounds();
            LoadTextures();
            LoadModels();
        }

        // Code for the sounds here
        private void LoadSounds()
        {
            // Walking sounds effects 
            var soundEffect =
                Content.Load<SoundEffect>("Assets/Audio/Diegetic/Wood_Running_1.1_-2-2");

            soundManager.Add(new Cue(
                "walk",
                soundEffect,
                SoundCategoryType.Alarm,
                new Vector3(0.25f, 0.5f, 0),
                true));
            soundManager.Play2D("walk");
            Application.SoundManager.Pause("walk");

            // Background music for my game..
            soundEffect =
              Content.Load<SoundEffect>("Assets/Audio/Diegetic/BgMusic");

            soundManager.Add(new Cue(
                "BG-Music",
                soundEffect,
                SoundCategoryType.Alarm,
                new Vector3(0.25f, 0, 0),
                false));

            soundManager.Play2D("BG-Music");

            // First riddle in the game.............
            soundEffect =
              Content.Load<SoundEffect>("Assets/Audio/Placeholder/Entering-1");

            soundManager.Add(new Cue(
                "Riddle1",
                soundEffect,
                SoundCategoryType.Alarm,
                new Vector3(0.25f, 0, 0),
                false));
                soundManager.Play2D("Riddle1");
            Application.SoundManager.Pause("Riddle1");


            //Second riddle in the game..........
            soundEffect =
               Content.Load<SoundEffect>("Assets/Audio/Placeholder/Entering-Kitchen");

            soundManager.Add(new Cue(
                "Riddle2",
                soundEffect,
                SoundCategoryType.Alarm,
                new Vector3(0.25f, 0, 0),
                false));
                soundManager.Play2D("Riddle2");
            Application.SoundManager.Pause("Riddle2");


            // Third riddle in the game........
            soundEffect =
               Content.Load<SoundEffect>("Assets/Audio/Placeholder/Grandfather-Note");

            soundManager.Add(new Cue(
                "Riddle3",
                soundEffect,
                SoundCategoryType.Alarm,
                new Vector3(0.25f, 0, 0),
                false));
                soundManager.Play2D("Riddle3");
            Application.SoundManager.Pause("Riddle3");

        }

        private void LoadTextures()
        {
            //load and add to dictionary
            //Content.Load<Texture>
        }

        private void LoadModels()
        {
            //load and add to dictionary
        }

        private void InitializeCurves()
        {
            //load and add to dictionary
        }

        private void InitializeRails()
        {
            //load and add to dictionary
        }

        private void InitializeScenes()
        {
            //initialize a scene
            var scene = new Scene("labyrinth");

            //add scene to the scene manager
            sceneManager.Add(scene.ID, scene);

            //don't forget to set active scene
            sceneManager.SetActiveScene("labyrinth");
        }

        private void InitializeEffects()
        {
            //only for skybox with lighting disabled
            unlitEffect = new BasicEffect(_graphics.GraphicsDevice);
            unlitEffect.TextureEnabled = true;

            //all other drawn objects
            litEffect = new BasicEffect(_graphics.GraphicsDevice);
            litEffect.TextureEnabled = true;
            litEffect.LightingEnabled = true;
            litEffect.EnableDefaultLighting();
        }

        private void InitializeCameras()
        {
            //camera
            GameObject cameraGameObject = null;

            #region Third Person

            cameraGameObject = new GameObject(AppData.THIRD_PERSON_CAMERA_NAME);
            cameraGameObject.Transform = new Transform(null, new Vector3(-35, 0, 0), null);
            cameraGameObject.AddComponent(new Camera(
                AppData.FIRST_PERSON_HALF_FOV, //MathHelper.PiOver2 / 2,
                (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight,
                AppData.FIRST_PERSON_CAMERA_NCP, //0.1f,
                AppData.FIRST_PERSON_CAMERA_FCP,
                new Viewport(0, 0, _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight))); // 3000

            cameraGameObject.AddComponent(new ThirdPersonController());

            cameraManager.Add(cameraGameObject.Name, cameraGameObject);

            #endregion

            #region First Person

            //camera 1
            cameraGameObject = new GameObject(AppData.FIRST_PERSON_CAMERA_NAME);
            cameraGameObject.Transform = new Transform(null, null, AppData.FIRST_PERSON_DEFAULT_CAMERA_POSITION);
                

            #region Camera - View & Projection

            cameraGameObject.AddComponent(
             new Camera(
             AppData.FIRST_PERSON_HALF_FOV, //MathHelper.PiOver2 / 2,
             (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight,
             AppData.FIRST_PERSON_CAMERA_NCP, //0.1f,
             AppData.FIRST_PERSON_CAMERA_FCP,
             new Viewport(0, 0, _graphics.PreferredBackBufferWidth,
             _graphics.PreferredBackBufferHeight))); // 3000

            #endregion

            #region Collision - Add capsule

            //adding a collidable surface that enables acceleration, jumping
            var characterCollider = new CharacterCollider(cameraGameObject, true);

            cameraGameObject.AddComponent(characterCollider);
            characterCollider.AddPrimitive(new Capsule(
                cameraGameObject.Transform.Translation,
                Matrix.CreateRotationX(MathHelper.PiOver2),
                1, 3.6f),
                new MaterialProperties(0.2f, 0.8f, 0.7f));
            characterCollider.Enable(cameraGameObject, false, 1);

            #endregion

            #region Collision - Add Controller for movement (now with collision)

            cameraGameObject.AddComponent(new CollidableFirstPersonController(cameraGameObject,
                characterCollider,
                AppData.FIRST_PERSON_MOVE_SPEED, AppData.FIRST_PERSON_STRAFE_SPEED,
                AppData.PLAYER_ROTATE_SPEED_VECTOR2, AppData.FIRST_PERSON_CAMERA_SMOOTH_FACTOR, true,
                AppData.PLAYER_COLLIDABLE_JUMP_HEIGHT));

            #endregion

            #region 3D Sound

            //added ability for camera to listen to 3D sounds
            cameraGameObject.AddComponent(new AudioListenerBehaviour());

            #endregion

           


            cameraManager.Add(cameraGameObject.Name, cameraGameObject);

            #endregion First Person

            #region Security

            //camera 2
            cameraGameObject = new GameObject(AppData.SECURITY_CAMERA_NAME);

            cameraGameObject.Transform
                = new Transform(null,
                null,
                new Vector3(0, 2, 25));

            //add camera (view, projection)
            cameraGameObject.AddComponent(new Camera(
                MathHelper.PiOver2 / 2,
                (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight,
                0.1f, 3500,
                new Viewport(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight)));

            //add rotation
            cameraGameObject.AddComponent(new CycledRotationBehaviour(
                AppData.SECURITY_CAMERA_ROTATION_AXIS,
                AppData.SECURITY_CAMERA_MAX_ANGLE,
                AppData.SECURITY_CAMERA_ANGULAR_SPEED_MUL,
                TurnDirectionType.Right));

            //adds FOV change on mouse scroll
            cameraGameObject.AddComponent(new CameraFOVController(AppData.CAMERA_FOV_INCREMENT_LOW));

            cameraManager.Add(cameraGameObject.Name, cameraGameObject);

            #endregion Security

            // Curve camera to show an overview of my game space!
            #region Curve

            Curve3D curve3D = new Curve3D(CurveLoopType.Oscillate);
            curve3D.Add(new Vector3(0, 25, 0), 0);
            curve3D.Add(new Vector3(0, 25, -150), 10000);
            curve3D.Add(new Vector3(0, 25, 5), 10000);
            //curve3D.Add(new Vector3(0, 25, 100), 10000);

            cameraGameObject = new GameObject(AppData.CURVE_CAMERA_NAME);

            cameraGameObject.Transform = new Transform(null, new Vector3(-35, 0, 0), null);
               
            cameraGameObject.AddComponent(new Camera(
                MathHelper.PiOver2 / 2,
                (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight,
                -100, 1500,
                  new Viewport(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight)));

            //define what action the curve will apply to the target game object
            var curveAction = (Curve3D curve, GameObject target, GameTime gameTime) =>
            {
                target.Transform.SetTranslation(curve.Evaluate(gameTime.TotalGameTime.TotalMilliseconds, 4));
            };

            cameraGameObject.AddComponent(new CurveBehaviour(curve3D, curveAction));

            cameraManager.Add(cameraGameObject.Name, cameraGameObject);

            #endregion Curve

            cameraManager.SetActiveCamera(AppData.FIRST_PERSON_CAMERA_NAME);
        }


        // To Initialize my stuff >>><<<
        private void InitializeCollidableContent(float worldScale)
        {
            InitializeCollidableGround(worldScale);
            //InitializeCollidableBox();
            InitializeCollidableHighDetailMonkey();

            //My Content
            InitializeWalls();
            InitializeSpheres1();
            InitializeSpheres2();
            InitializeSpheres3();

            InitializeRiddle1();
            InitializeRiddle2();
            InitializeRiddle3();
        }

        private void InitializeNonCollidableContent(float worldScale)
        {
            //InitializeXYZ();

            //create sky
            InitializeSkyBox(worldScale);

            //quad with crate texture
            //InitializeDemoQuad();

            //load an FBX and draw
            //InitializeDemoModel();

            //TODO - remove these test methods later
            //test for one team
           // InitializeRadarModel();
            //test for another team
           // InitializeDemoButton();

            //quad with a tree texture
            //InitializeTreeQuad();
        }

        private void InitializeXYZ()
        {
            //  throw new NotImplementedException();
        }

        #region My Object Models 

        // Creating a simple Level Design for my Game

        #region Walls
        private void InitializeWalls()
        {

            var gameObject = new GameObject("Walls",ObjectType.Static, RenderType.Opaque);
            var model = Content.Load<Model>("Assets/Models/Wall/CastleWall_Fixed");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);
            var texture = Content.Load<Texture2D>("Assets/Textures/Walls/Castle Towers UV New");
            var collider = new Collider(gameObject);

            for (int i = 0; i < 11; i++)
            {
                gameObject = new GameObject("Wall Left Side" + i, ObjectType.Static, RenderType.Opaque);
                gameObject.Transform = new Transform(
                                   new Vector3(20, 39, 19),
                                   new Vector3(0, 630.25f, 0),
                                   new Vector3(20, 6.9f, -10.8f - (18.9f * i)));
                gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f),
                mesh));

                //add Collision Surface(s)
                collider = new Collider(gameObject, true);
                collider.AddPrimitive(new Box(
                    gameObject.Transform.Translation,
                    gameObject.Transform.Rotation,
                    new Vector3(19.4f, 8, 3.4f)),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
                collider.Enable(gameObject, true, 50);
                gameObject.AddComponent(collider);
                sceneManager.ActiveScene.Add(gameObject);
            }

            //Right side Wall
            for (int i = 0; i < 11; i++)
            {
                gameObject = new GameObject("Wall Right Side" + i, ObjectType.Static, RenderType.Opaque);
                gameObject.Transform = new Transform(
                    new Vector3(20, 39, 19),
                    new Vector3(0, 630.25f, 0),
                    new Vector3(-20, 6.9f, -10.8f - (18.9f * i)));
                gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f),
                mesh));

                //add Collision Surface(s)
                collider = new Collider(gameObject, true);
                collider.AddPrimitive(new Box(
                    gameObject.Transform.Translation,
                    gameObject.Transform.Rotation,
                    new Vector3(19.4f, 8, 3.4f)),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
                collider.Enable(gameObject, true, 50);
                gameObject.AddComponent(collider);
                sceneManager.ActiveScene.Add(gameObject);
            }

            // Middle Walls level 1
            for (int i = 0; i < 2; i++)
            {
                gameObject = new GameObject("Wall For Level 1" + i, ObjectType.Static, RenderType.Opaque);
                gameObject.Transform = new Transform(
                    new Vector3(20, 39, 19),
                    Vector3.Zero,
                    new Vector3(10 - (19.3f * i), 6.9f, -50));
                gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f),
                mesh));
                //add Collision Surface(s)
                collider = new Collider(gameObject, true);
                collider.AddPrimitive(new Box(
                    gameObject.Transform.Translation,
                    gameObject.Transform.Rotation,
                    new Vector3(19.4f, 8, 3.4f)),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
                collider.Enable(gameObject, true, 50);
                gameObject.AddComponent(collider);
                sceneManager.ActiveScene.Add(gameObject);
            }

            // Middle Walls level 2
            for (int i = 0; i < 2; i++)
            {
                gameObject = new GameObject("Wall For Level 2" + i, ObjectType.Static, RenderType.Opaque);
                gameObject.Transform = new Transform(
                    new Vector3(20, 39, 19),
                    Vector3.Zero,
                    new Vector3(10 - (19.3f * i), 6.9f, -100));
                gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f),
                mesh));
                //add Collision Surface(s)
                collider = new Collider(gameObject, true);
                collider.AddPrimitive(new Box(
                    gameObject.Transform.Translation,
                    gameObject.Transform.Rotation,
                    new Vector3(19.4f, 8, 3.4f)),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
                collider.Enable(gameObject, true, 50);
                gameObject.AddComponent(collider);
                sceneManager.ActiveScene.Add(gameObject);
            }

            // Middle Walls level 3
            for (int i = 0; i < 2; i++)
            {
                gameObject = new GameObject("Wall For Level 3" + i, ObjectType.Static, RenderType.Opaque);
                gameObject.Transform = new Transform(
                    new Vector3(20, 39, 19),
                    Vector3.Zero,
                    new Vector3(10 - (19.3f * i), 6.9f, -150));
                gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f),
                mesh));
                //add Collision Surface(s)
                collider = new Collider(gameObject, true);
                collider.AddPrimitive(new Box(
                    gameObject.Transform.Translation,
                    gameObject.Transform.Rotation,
                    new Vector3(19.4f, 8, 3.4f)),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
                collider.Enable(gameObject, true, 50);
                gameObject.AddComponent(collider);
                sceneManager.ActiveScene.Add(gameObject);
            }


            // Middle Walls End
            for (int i = 0; i < 2; i++)
            {
                gameObject = new GameObject("Wall For Level 3" + i, ObjectType.Static, RenderType.Opaque);
                gameObject.Transform = new Transform(
                    new Vector3(20, 39, 19),
                    Vector3.Zero,
                    new Vector3(10 - (19.3f * i), 6.9f, -205));
                gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f),
                mesh));
                //add Collision Surface(s)
                collider = new Collider(gameObject, true);
                collider.AddPrimitive(new Box(
                    gameObject.Transform.Translation,
                    gameObject.Transform.Rotation,
                    new Vector3(19.4f, 8, 3.4f)),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
                collider.Enable(gameObject, true, 50);
                gameObject.AddComponent(collider);
                sceneManager.ActiveScene.Add(gameObject);
            }

            // Middle Walls Top
            for (int i = 0; i < 2; i++)
            {
                gameObject = new GameObject("Wall For Level 3" + i, ObjectType.Static, RenderType.Opaque);
                gameObject.Transform = new Transform(
                    new Vector3(20, 39, 19),
                    Vector3.Zero,
                    new Vector3(10 - (19.3f * i), 6.9f, 0));
                gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f),
                mesh));
                //add Collision Surface(s)
                collider = new Collider(gameObject, true);
                collider.AddPrimitive(new Box(
                    gameObject.Transform.Translation,
                    gameObject.Transform.Rotation,
                    new Vector3(19.4f, 8, 3.4f)),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
                collider.Enable(gameObject, true, 50);
                gameObject.AddComponent(collider);
                sceneManager.ActiveScene.Add(gameObject);
            }

        }
         #endregion


        #region Answer Spheres

        private void InitializeSpheres1()
        {
            //answer 1
            var gameObject = new GameObject("Answer Sphere 1", ObjectType.Static, RenderType.Opaque);


            //Object size, rotation, position
            gameObject.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(-10, 2, -45));

            //Assest path of object
            var texture = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model = Content.Load<Model>("Assets/Models/sphere");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            //put object into game..
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Yellow),
                mesh));


            sceneManager.ActiveScene.Add(gameObject);


            // Asnwer 2
            var gameObject2 = new GameObject("Answer Sphere 2", ObjectType.Static, RenderType.Opaque);

            //Object size, rotation, position
            gameObject2.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(0, 2, -45));

            //Assest path of object
            var texture2 = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model2 = Content.Load<Model>("Assets/Models/sphere");
            var mesh2 = new Engine.ModelMesh(_graphics.GraphicsDevice, model2);

            //put object into game..
            gameObject2.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Green),
                mesh));

            sceneManager.ActiveScene.Add(gameObject2);


            //answer 3
            var gameObject3 = new GameObject("Answer Sphere 1", ObjectType.Static, RenderType.Opaque);


            //Object size, rotation, position
            gameObject3.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(10, 2, -45));

            //Assest path of object
            var texture3 = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model3 = Content.Load<Model>("Assets/Models/sphere");
            var mesh3 = new Engine.ModelMesh(_graphics.GraphicsDevice, model3);

            //put object into game..
            gameObject3.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Red),
                mesh));


            sceneManager.ActiveScene.Add(gameObject3);

        }

        private void InitializeSpheres2()
        {
            //answer 1
            var gameObject = new GameObject("Answer Sphere 1", ObjectType.Static, RenderType.Opaque);


            //Object size, rotation, position
            gameObject.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(-10, 2, -95));

            //Assest path of object
            var texture = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model = Content.Load<Model>("Assets/Models/sphere");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            //put object into game..
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Yellow),
                mesh));


            sceneManager.ActiveScene.Add(gameObject);


            // Asnwer 2
            var gameObject2 = new GameObject("Answer Sphere 2", ObjectType.Static, RenderType.Opaque);

            //Object size, rotation, position
            gameObject2.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(0, 2, -95));

            //Assest path of object
            var texture2 = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model2 = Content.Load<Model>("Assets/Models/sphere");
            var mesh2 = new Engine.ModelMesh(_graphics.GraphicsDevice, model2);

            //put object into game..
            gameObject2.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Green),
                mesh));

            sceneManager.ActiveScene.Add(gameObject2);


            //answer 3
            var gameObject3 = new GameObject("Answer Sphere 1", ObjectType.Static, RenderType.Opaque);


            //Object size, rotation, position
            gameObject3.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(10, 2, -95));

            //Assest path of object
            var texture3 = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model3 = Content.Load<Model>("Assets/Models/sphere");
            var mesh3 = new Engine.ModelMesh(_graphics.GraphicsDevice, model3);

            //put object into game..
            gameObject3.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Red),
                mesh));


            sceneManager.ActiveScene.Add(gameObject3);

        }

        private void InitializeSpheres3()
        {
            //answer 1
            var gameObject = new GameObject("Answer Sphere 1", ObjectType.Static, RenderType.Opaque);


            //Object size, rotation, position
            gameObject.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(-10, 2, -145));

            //Assest path of object
            var texture = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model = Content.Load<Model>("Assets/Models/sphere");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            //put object into game..
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Yellow),
                mesh));


            sceneManager.ActiveScene.Add(gameObject);


            // Asnwer 2
            var gameObject2 = new GameObject("Answer Sphere 2", ObjectType.Static, RenderType.Opaque);

            //Object size, rotation, position
            gameObject2.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(0, 2, -145));

            //Assest path of object
            var texture2 = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model2 = Content.Load<Model>("Assets/Models/sphere");
            var mesh2 = new Engine.ModelMesh(_graphics.GraphicsDevice, model2);

            //put object into game..
            gameObject2.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Green),
                mesh));

            sceneManager.ActiveScene.Add(gameObject2);


            //answer 3
            var gameObject3 = new GameObject("Answer Sphere 1", ObjectType.Static, RenderType.Opaque);


            //Object size, rotation, position
            gameObject3.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(10, 2, -145));

            //Assest path of object
            var texture3 = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model3 = Content.Load<Model>("Assets/Models/sphere");
            var mesh3 = new Engine.ModelMesh(_graphics.GraphicsDevice, model3);

            //put object into game..
            gameObject3.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Red),
                mesh));


            sceneManager.ActiveScene.Add(gameObject3);

        }


        #endregion


        #region Riddle Platform 

        private void InitializeRiddle1()
        {
            //game object
            var gameObject = new GameObject("Spawn 1", ObjectType.Static, RenderType.Opaque);
            gameObject.GameObjectType = GameObjectType.Consumable;

            //Object size, rotation, position
            gameObject.Transform = new Transform(
                new Vector3(5, 0.1f, 5),
                new Vector3(0, 0, 0),
                new Vector3(0, 0.1f, -25));

            //Assest path of object
            var texture = Content.Load<Texture2D>("Assets/Textures/Walls/Castle Towers UV New");
            var model = Content.Load<Model>("Assets/Models/cube");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            //put object into game..
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Black),
                mesh));


            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializeRiddle2()
        {
            //game object
            var gameObject = new GameObject("Spawn 1", ObjectType.Static, RenderType.Opaque);
            gameObject.GameObjectType = GameObjectType.Consumable;

            //Object size, rotation, position
            gameObject.Transform = new Transform(
                new Vector3(5, 0.1f, 5),
                new Vector3(0, 0, 0),
                new Vector3(0, 0.1f, -75));

            //Assest path of object
            var texture = Content.Load<Texture2D>("Assets/Textures/Walls/Castle Towers UV New");
            var model = Content.Load<Model>("Assets/Models/cube");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            //put object into game..
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Black),
                mesh));


            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializeRiddle3()
        {
            //game object
            var gameObject = new GameObject("Spawn 1", ObjectType.Static, RenderType.Opaque);
            gameObject.GameObjectType = GameObjectType.Consumable;

            //Object size, rotation, position
            gameObject.Transform = new Transform(
                new Vector3(5, 0.1f, 5),
                new Vector3(0, 0, 0),
                new Vector3(0, 0.1f, -125));

            //Assest path of object
            var texture = Content.Load<Texture2D>("Assets/Textures/Walls/Castle Towers UV New");
            var model = Content.Load<Model>("Assets/Models/cube");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            //put object into game..
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Black),
                mesh));


            sceneManager.ActiveScene.Add(gameObject);
        }

        #endregion

        #endregion

        private void InitializeCollidableGround(float worldScale)
        {
            var gdBasicEffect = new GDBasicEffect(unlitEffect);
            var quadMesh = new QuadMesh(_graphics.GraphicsDevice);

            //ground
            var ground = new GameObject("ground");
            ground.Transform = new Transform(new Vector3(worldScale, worldScale, 1),
                new Vector3(-90, 0, 0), new Vector3(0, 0, 0));
            var texture = Content.Load<Texture2D>("Assets/Textures/Foliage/Ground/grass1");
            ground.AddComponent(new Renderer(gdBasicEffect, new Material(texture, 1), quadMesh));

            //add Collision Surface(s)
            var collider = new Collider(ground);
            collider.AddPrimitive(new Box(
                    ground.Transform.Translation,
                    ground.Transform.Rotation,
                    ground.Transform.Scale),
                    new MaterialProperties(0.8f, 0.8f, 0.7f));
            collider.Enable(ground, true, 1);
            ground.AddComponent(collider);

            sceneManager.ActiveScene.Add(ground);
        }

        private void InitializeCollidableBox()
        {
            //game object
            var gameObject = new GameObject("my first collidable box!", ObjectType.Dynamic, RenderType.Opaque);
            gameObject.GameObjectType = GameObjectType.Collectible;

            gameObject.Transform = new Transform(
                new Vector3(1, 1, 1),
                new Vector3(45, 45, 0),
                new Vector3(0, 15, 0));
            var texture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate2");
            var model = Content.Load<Model>("Assets/Models/cube");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1, Color.White),
                mesh));

            var collider = new Collider(gameObject, true);
            collider.AddPrimitive(new Box(
                gameObject.Transform.Translation,
                gameObject.Transform.Rotation,
                gameObject.Transform.Scale), //make the colliders a fraction larger so that transparent boxes dont sit exactly on the ground and we end up with flicker or z-fighting
                new MaterialProperties(0.8f, 0.8f, 0.7f));
            collider.Enable(gameObject, false, 10);
            gameObject.AddComponent(collider);

            //var collider = new Collider(gameObject);
            //collider.AddPrimitive(new Sphere(
            //    gameObject.Transform.Translation, 1), //make the colliders a fraction larger so that transparent boxes dont sit exactly on the ground and we end up with flicker or z-fighting
            //    new MaterialProperties(0.2f, 0.8f, 0.7f));
            //collider.Enable(gameObject, true, 10);
            //gameObject.AddComponent(collider);

            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializeCollidableHighDetailMonkey()
        {
            //game object
            var gameObject = new GameObject("Going to be the Big boss!", ObjectType.Static, RenderType.Opaque);
            gameObject.GameObjectType = GameObjectType.Consumable;

            //Object size, rotation, position
            gameObject.Transform = new Transform(
                new Vector3(12, 12, 12),
                new Vector3(0, 0, 0),
                new Vector3(0, 5, -190));

            //Assest path of object
            var texture = Content.Load<Texture2D>("Assets/Textures/Walls/sphere_txt");
            var model = Content.Load<Model>("Assets/Models/monkey");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            //put object into game..
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.Gold),
                mesh));


            // Show the collider lines
            
            var model_medium = Content.Load<Model>("Assets/Models/monkey_medium");
            var collider = new Collider(gameObject);
            collider.AddPrimitive(CollisionUtility.GetTriangleMesh(model_medium,
                gameObject.Transform), new MaterialProperties(0.8f, 0.8f, 0.7f));

            //NOTE - TriangleMesh colliders MUST be marked as IMMOVABLE=TRUE
            collider.Enable(gameObject, true, 1);
            gameObject.AddComponent(collider);
            

            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializeDemoModel()
        {
            //game object
            var gameObject = new GameObject("my first bottle!",
                ObjectType.Static, RenderType.Opaque);

            gameObject.Transform = new Transform(0.0005f * Vector3.One,
                new Vector3(-90, 0, 0), new Vector3(2, 0, 0));
            var texture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate2");

            var model = Content.Load<Model>("Assets/Models/bottle2");

            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.White),
                mesh));

            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializeRadarModel()
        {
            //game object
            var gameObject = new GameObject("radar",
                ObjectType.Static, RenderType.Opaque);

            gameObject.Transform = new Transform(0.005f * Vector3.One,
                new Vector3(0, 0, 0), new Vector3(8, 0, 0));
            var texture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate2");

            var model = Content.Load<Model>("Assets/Models/radar-display");

            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.White),
                mesh));

            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializeDemoButton()
        {
            //game object
            var gameObject = new GameObject("my first button!",
                ObjectType.Static, RenderType.Opaque);

            gameObject.Transform = new Transform(6 * Vector3.One,
                new Vector3(0, 0, 0), new Vector3(-10, -5, 0));
            var texture = Content.Load<Texture2D>("Assets/Textures/Button/button_DefaultMaterial_Base_color");

            var model = Content.Load<Model>("Assets/Models/button");

            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(litEffect),
                new Material(texture, 1f, Color.White),
                mesh));

            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializeDemoQuad()
        {
            //game object
            var gameObject = new GameObject("Box",
                ObjectType.Dynamic, RenderType.Transparent);

            gameObject.Transform = new Transform(null, null,
                new Vector3(4, 4, 4));  //World

            var texture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate1");

            gameObject.AddComponent(new Renderer(new GDBasicEffect(litEffect),
                new Material(texture, 1), new QuadMesh(_graphics.GraphicsDevice)));

           // gameObject.AddComponent(new SimpleRotationBehaviour(new Vector3(1, 0, 0), 5 / 60.0f));

            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializeTreeQuad()
        {
            //game object
            var gameObject = new GameObject("my first tree", ObjectType.Static,
                RenderType.Transparent);
            gameObject.Transform = new Transform(new Vector3(3, 3, 1), null, new Vector3(-6, 1.5f, 1));  //World
            var texture = Content.Load<Texture2D>("Assets/Textures/Foliage/Trees/tree1");
            gameObject.AddComponent(new Renderer(
                new GDBasicEffect(unlitEffect),
                new Material(texture, 1),
                new QuadMesh(_graphics.GraphicsDevice)));

            //a weird tree that makes sounds
            gameObject.AddComponent(new AudioEmitterBehaviour());

            sceneManager.ActiveScene.Add(gameObject);
        }

        private void InitializePlayer()
        {
            playerGameObject = new GameObject("player 1", ObjectType.Static, RenderType.Opaque);

            playerGameObject.Transform = new Transform(new Vector3(0.4f, 0.4f, 1),
                null, new Vector3(0, 0.2f, -2));
            var texture = Content.Load<Texture2D>("Assets/Textures/Props/Crates/crate2");
            var model = Content.Load<Model>("Assets/Models/sphere");
            var mesh = new Engine.ModelMesh(_graphics.GraphicsDevice, model);

            playerGameObject.AddComponent(new Renderer(new GDBasicEffect(litEffect),
                new Material(texture, 1),
                mesh));

            playerGameObject.AddComponent(new PlayerController(AppData.FIRST_PERSON_MOVE_SPEED, AppData.FIRST_PERSON_STRAFE_SPEED,
                AppData.PLAYER_ROTATE_SPEED_VECTOR2, true));

            sceneManager.ActiveScene.Add(playerGameObject);

            //set this as active player
            Application.Player = playerGameObject;
        }

        private void InitializeSkyBox(float worldScale)
        {
            float halfWorldScale = worldScale / 2.0f;

            GameObject quad = null;
            var gdBasicEffect = new GDBasicEffect(unlitEffect);
            var quadMesh = new QuadMesh(_graphics.GraphicsDevice);

            //skybox - back face
            quad = new GameObject("skybox back face");
            quad.Transform = new Transform(new Vector3(worldScale, worldScale, 1), null, new Vector3(0, 0, -halfWorldScale));
            var texture = Content.Load<Texture2D>("Assets/Textures/Skybox/back");
            quad.AddComponent(new Renderer(gdBasicEffect, new Material(texture, 1), quadMesh));
            sceneManager.ActiveScene.Add(quad);

            //skybox - left face
            quad = new GameObject("skybox left face");
            quad.Transform = new Transform(new Vector3(worldScale, worldScale, 1),
                new Vector3(0, 90, 0), new Vector3(-halfWorldScale, 0, 0));
            texture = Content.Load<Texture2D>("Assets/Textures/Skybox/left");
            quad.AddComponent(new Renderer(gdBasicEffect, new Material(texture, 1), quadMesh));
            sceneManager.ActiveScene.Add(quad);

            //skybox - right face
            quad = new GameObject("skybox right face");
            quad.Transform = new Transform(new Vector3(worldScale, worldScale, 1),
                new Vector3(0, -90, 0), new Vector3(halfWorldScale, 0, 0));
            texture = Content.Load<Texture2D>("Assets/Textures/Skybox/right");
            quad.AddComponent(new Renderer(gdBasicEffect, new Material(texture, 1), quadMesh));
            sceneManager.ActiveScene.Add(quad);

            //skybox - top face
            quad = new GameObject("skybox top face");
            quad.Transform = new Transform(new Vector3(worldScale, worldScale, 1),
                new Vector3(90, -90, 0), new Vector3(0, halfWorldScale, 0));
            texture = Content.Load<Texture2D>("Assets/Textures/Skybox/sky");
            quad.AddComponent(new Renderer(gdBasicEffect, new Material(texture, 1), quadMesh));
            sceneManager.ActiveScene.Add(quad);

            //skybox - front face
            quad = new GameObject("skybox front face");
            quad.Transform = new Transform(new Vector3(worldScale, worldScale, 1),
                new Vector3(0, -180, 0), new Vector3(0, 0, halfWorldScale));
            texture = Content.Load<Texture2D>("Assets/Textures/Skybox/front");
            quad.AddComponent(new Renderer(gdBasicEffect, new Material(texture, 1), quadMesh));
            sceneManager.ActiveScene.Add(quad);
        }

        #endregion Actions - Level Specific

        #region Actions - Engine Specific

        private void InitializeEngine(Vector2 resolution, bool isMouseVisible, bool isCursorLocked)
        {
            //add support for mouse etc
            InitializeInput();

            //add game effects
            InitializeEffects();

            //add dictionaries to store and access content
            InitializeDictionaries();

            //add camera, scene manager
            InitializeManagers();

            //share some core references
            InitializeGlobals();

            //set screen properties (incl mouse)
            InitializeScreen(resolution, isMouseVisible, isCursorLocked);

            //add game cameras
            InitializeCameras();
        }

        private void InitializeGlobals()
        {
            //Globally shared commonly accessed variables
            Application.Main = this;
            Application.GraphicsDeviceManager = _graphics;
            Application.GraphicsDevice = _graphics.GraphicsDevice;
            Application.Content = Content;

            //Add access to managers from anywhere in the code
            Application.CameraManager = cameraManager;
            Application.SceneManager = sceneManager;
            Application.SoundManager = soundManager;
            Application.PhysicsManager = physicsManager;

            Application.UISceneManager = uiManager;
            Application.MenuSceneManager = menuManager;
        }

        private void InitializeInput()
        {
            //Globally accessible inputs
            Input.Keys = new KeyboardComponent(this);
            Components.Add(Input.Keys);
            Input.Mouse = new MouseComponent(this);
            Components.Add(Input.Mouse);
            Input.Gamepad = new GamepadComponent(this);
            Components.Add(Input.Gamepad);
        }

        /// <summary>
        /// Sets game window dimensions and shows/hides the mouse
        /// </summary>
        /// <param name="resolution"></param>
        /// <param name="isMouseVisible"></param>
        /// <param name="isCursorLocked"></param>
        private void InitializeScreen(Vector2 resolution, bool isMouseVisible, bool isCursorLocked)
        {
            Screen screen = new Screen();

            //set resolution
            screen.Set(resolution, isMouseVisible, isCursorLocked);

            //set global for re-use by other entities
            Application.Screen = screen;

            //set starting mouse position i.e. set mouse in centre at startup
            Input.Mouse.Position = screen.ScreenCentre;

            ////calling set property
            //_graphics.PreferredBackBufferWidth = (int)resolution.X;
            //_graphics.PreferredBackBufferHeight = (int)resolution.Y;
            //IsMouseVisible = isMouseVisible;
            //_graphics.ApplyChanges();
        }

        private void InitializeManagers()
        {
            //add event dispatcher for system events - the most important element!!!!!!
            eventDispatcher = new EventDispatcher(this);
            //add to Components otherwise no Update() called
            Components.Add(eventDispatcher);

            //add support for multiple cameras and camera switching
            cameraManager = new CameraManager(this);
            //add to Components otherwise no Update() called
            Components.Add(cameraManager);

            //big kahuna nr 1! this adds support to store, switch and Update() scene contents
            sceneManager = new SceneManager<Scene>(this);
            //add to Components otherwise no Update()
            Components.Add(sceneManager);

            //big kahuna nr 2! this renders the ActiveScene from the ActiveCamera perspective
            renderManager = new RenderManager(this, new ForwardSceneRenderer(_graphics.GraphicsDevice));
            renderManager.DrawOrder = 1;
            Components.Add(renderManager);

            //add support for playing sounds
            soundManager = new SoundManager();
            //why don't we add SoundManager to Components? Because it has no Update()
            //wait...SoundManager has no update? Yes, playing sounds is handled by an internal MonoGame thread - so we're off the hook!

            //add the physics manager update thread
            physicsManager = new PhysicsManager(this, AppData.GRAVITY);
            Components.Add(physicsManager);

            #region Collision - Picking

            //picking support using physics engine
            //this predicate lets us say ignore all the other collidable objects except interactables and consumables
            Predicate<GameObject> collisionPredicate =
                (collidableObject) =>
                {
                    if (collidableObject != null)
                        return collidableObject.GameObjectType
                        == GameObjectType.Interactable
                        || collidableObject.GameObjectType == GameObjectType.Consumable
                        || collidableObject.GameObjectType == GameObjectType.Collectible;
                    return false;
                };

            pickingManager = new PickingManager(this,
                AppData.PICKING_MIN_PICK_DISTANCE,
                AppData.PICKING_MAX_PICK_DISTANCE,
                collisionPredicate);
            Components.Add(pickingManager);

            #endregion

            #region Game State

            //add state manager for inventory and countdown
            stateManager = new MyStateManager(this, AppData.MAX_GAME_TIME_IN_MSECS);
            Components.Add(stateManager);

            #endregion

            #region UI

            uiManager = new SceneManager<Scene2D>(this);
            uiManager.StatusType = StatusType.Off;
            uiManager.IsPausedOnPlay = false;
            Components.Add(uiManager);

            var uiRenderManager = new Render2DManager(this, _spriteBatch, uiManager);
            uiRenderManager.StatusType = StatusType.Off;
            uiRenderManager.DrawOrder = 2;
            uiRenderManager.IsPausedOnPlay = false;
            Components.Add(uiRenderManager);

            #endregion

            #region Menu

            menuManager = new SceneManager<Scene2D>(this);
            menuManager.StatusType = StatusType.Updated;
            menuManager.IsPausedOnPlay = true;
            Components.Add(menuManager);

            var menuRenderManager = new Render2DManager(this, _spriteBatch, menuManager);
            menuRenderManager.StatusType = StatusType.Drawn;
            menuRenderManager.DrawOrder = 3;
            menuRenderManager.IsPausedOnPlay = true;
            Components.Add(menuRenderManager);

            #endregion
        }

        private void InitializeDictionaries()
        {
            //TODO - add texture dictionary, soundeffect dictionary, model dictionary
        }

        private void InitializeDebug(bool showCollisionSkins = true)
        {
            //intialize the utility component
            var perfUtility = new PerfUtility(this, _spriteBatch,
                new Vector2(10, 10),
                new Vector2(0, 22));

            //set the font to be used
            var spriteFont = Content.Load<SpriteFont>("Assets/Fonts/Perf");

            //add components to the info list to add UI information
            float headingScale = 1f;
            float contentScale = 0.9f;
            perfUtility.infoList.Add(new TextInfo(_spriteBatch, spriteFont, "Performance ------------------------------", Color.Yellow, headingScale * Vector2.One));
            perfUtility.infoList.Add(new FPSInfo(_spriteBatch, spriteFont, "FPS:", Color.White, contentScale * Vector2.One));
            perfUtility.infoList.Add(new TextInfo(_spriteBatch, spriteFont, "Camera -----------------------------------", Color.Yellow, headingScale * Vector2.One));
            perfUtility.infoList.Add(new CameraNameInfo(_spriteBatch, spriteFont, "Name:", Color.White, contentScale * Vector2.One));

            var infoFunction = (Transform transform) =>
            {
                return transform.Translation.GetNewRounded(1).ToString();
            };

            perfUtility.infoList.Add(new TransformInfo(_spriteBatch, spriteFont, "Pos:", Color.White, contentScale * Vector2.One,
                ref Application.CameraManager.ActiveCamera.transform, infoFunction));

            infoFunction = (Transform transform) =>
            {
                return transform.Rotation.GetNewRounded(1).ToString();
            };

            perfUtility.infoList.Add(new TransformInfo(_spriteBatch, spriteFont, "Rot:", Color.White, contentScale * Vector2.One,
                ref Application.CameraManager.ActiveCamera.transform, infoFunction));

            perfUtility.infoList.Add(new TextInfo(_spriteBatch, spriteFont, "Object -----------------------------------", Color.Yellow, headingScale * Vector2.One));
            perfUtility.infoList.Add(new ObjectInfo(_spriteBatch, spriteFont, "Objects:", Color.White, contentScale * Vector2.One));
            perfUtility.infoList.Add(new TextInfo(_spriteBatch, spriteFont, "Hints -----------------------------------", Color.Yellow, headingScale * Vector2.One));
            perfUtility.infoList.Add(new TextInfo(_spriteBatch, spriteFont, "Use mouse scroll wheel to change security camera FOV, F1-F4 for camera switch", Color.White, contentScale * Vector2.One));
            perfUtility.infoList.Add(new TextInfo(_spriteBatch, spriteFont, "Use Up and Down arrow to see progress bar change", Color.White, contentScale * Vector2.One));

            //add to the component list otherwise it wont have its Update or Draw called!
            // perfUtility.StatusType = StatusType.Drawn | StatusType.Updated;
            perfUtility.DrawOrder = 3;
            Components.Add(perfUtility);

            if (showCollisionSkins)
            {
                var physicsDebugDrawer = new PhysicsDebugDrawer(this);
                physicsDebugDrawer.DrawOrder = 4;
                Components.Add(physicsDebugDrawer);
            }
        }

        #endregion Actions - Engine Specific

        #region Actions - Update, Draw

        #region triggers for Riddles
        public Vector3 rTriggers(Vector3 temp, string sound)
        {
            if (temp != Vector3.Zero)
                if (cameraManager.activeCamera.transform.translation.X <= temp.X + 2 && cameraManager.activeCamera.transform.translation.X >= temp.X - 2)
                    if (cameraManager.activeCamera.transform.translation.Z <= temp.Z + 2 && cameraManager.activeCamera.transform.translation.Z >= temp.Z - 2)
                    {
                        temp = Vector3.Zero;
                        Application.SoundManager.Stop("rOne");
                        Application.SoundManager.Stop("rTwo");
                        Application.SoundManager.Stop("rThree");
          
                        Application.SoundManager.Play2D(sound);

                    }
            return temp;

        }
        #endregion

        protected override void Update(GameTime gameTime)
        {
            #region Menu On/Off with U/P

            if (Input.Keys.WasJustPressed(Keys.P))
            {
                EventDispatcher.Raise(new EventData(EventCategoryType.Menu,
                    EventActionType.OnPause));
            }
            else if (Input.Keys.WasJustPressed(Keys.U))
            {
                EventDispatcher.Raise(new EventData(EventCategoryType.Menu,
                   EventActionType.OnPlay));
            }

            #endregion

            // My Code..............

            #region Walking Sounds

            if (Input.Keys.IsPressed(Keys.W) || Input.Keys.IsPressed(Keys.A) || Input.Keys.IsPressed(Keys.S) || Input.Keys.IsPressed(Keys.D))
            {
             
                Application.SoundManager.Resume("walk");
                Application.SoundManager.Resume("BG-Music");
            }
            else
            {
                Application.SoundManager.Pause("walk");
            }

            #endregion


            #region Riddles  

            // Trigger spots for riddles to start playing 
            rOne = rTriggers(rOne, "Riddle1");
            rTwo = rTriggers(rTwo, "Riddle2");
            rThree = rTriggers(rThree, "Riddle3");

            // Testing if Riddles are connected
            if (Input.Keys.IsPressed(Keys.NumPad1))
            {
                Application.SoundManager.Resume("Riddle1");
                Application.SoundManager.Pause("Riddle2");
                Application.SoundManager.Pause("Riddle3");

            }
            else if(Input.Keys.IsPressed(Keys.NumPad2))
            {
                Application.SoundManager.Pause("Riddle1");
                Application.SoundManager.Resume("Riddle2");
                Application.SoundManager.Pause("Riddle3");
            }
            else if(Input.Keys.IsPressed(Keys.NumPad3))
            {
                Application.SoundManager.Pause("Riddle1");
                Application.SoundManager.Pause("Riddle2");
                Application.SoundManager.Resume("Riddle3");
            }
           

            #endregion




#if DEMO

            #region Demo - UI - progress bar

            if (Input.Keys.WasJustPressed(Keys.Up))
            {
                object[] parameters = { "progress bar - health - 1", 1 };
                EventDispatcher.Raise(new EventData(EventCategoryType.UI, EventActionType.OnHealthDelta, parameters));
            }
            else if (Input.Keys.WasJustPressed(Keys.Down))
            {
                object[] parameters = { "progress bar - health - 1", -1 };
                EventDispatcher.Raise(new EventData(EventCategoryType.UI, EventActionType.OnHealthDelta, parameters));
            }

            #endregion

            #region Demo - Camera switching

            if (Input.Keys.IsPressed(Keys.F1))
                cameraManager.SetActiveCamera(AppData.FIRST_PERSON_CAMERA_NAME);
            else if (Input.Keys.IsPressed(Keys.F2))
                cameraManager.SetActiveCamera(AppData.SECURITY_CAMERA_NAME);
            else if (Input.Keys.IsPressed(Keys.F3))
                cameraManager.SetActiveCamera(AppData.CURVE_CAMERA_NAME);
            else if (Input.Keys.IsPressed(Keys.F4))
                cameraManager.SetActiveCamera(AppData.THIRD_PERSON_CAMERA_NAME);

            #endregion Demo - Camera switching

            #region Demo - Gamepad

            var thumbsL = Input.Gamepad.ThumbSticks(false);
            //   System.Diagnostics.Debug.WriteLine(thumbsL);

            var thumbsR = Input.Gamepad.ThumbSticks(false);
            //     System.Diagnostics.Debug.WriteLine(thumbsR);

            //    System.Diagnostics.Debug.WriteLine($"A: {Input.Gamepad.IsPressed(Buttons.A)}");

            #endregion Demo - Gamepad

            #region Demo - Raising events using GDEvent

            if (Input.Keys.WasJustPressed(Keys.E))
                OnChanged.Invoke(this, null); //passing null for EventArgs but we'll make our own class MyEventArgs::EventArgs later

            #endregion

#endif
            //fixed a bug with components not getting Update called
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        #endregion Actions - Update, Draw
    }
}