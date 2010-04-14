#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System;
using NUnit.Framework;
#endregion
/*
namespace Sokoban
{
    [TestFixture]
    public class SceneTests
    {
        bool sceneTest_1_callback_called = false;

        public void SceneTest_1_callback()
        {
            sceneTest_1_callback_called = true;
        }

        [Test]
        public void SceneTest_1() // test of: calling scenes and test of their callbacks (rely on implemenation of given scene)
        {
            View.GameDesk gameDesk = new View.GameDesk();

            Scene_DummyOne scene_DummyOne = new Scene_DummyOne(gameDesk);

            gameDesk.RegisterScene(scene_DummyOne, SceneTest_1_callback);
            gameDesk.DrawScenes();

            Assert.That(sceneTest_1_callback_called == true);
            Assert.That(scene_DummyOne.wasDrawCalled == true);
        }
    }

    public class Scene_DummyOne : Scene
    {
        public bool wasDrawCalled = false;

        public override void Draw()
        {
            wasDrawCalled = true;
            CallCallback();
        }

        public Scene_DummyOne(View.GameDesk gameDesk) : base(gameDesk)
        {

        }

    }
}*/