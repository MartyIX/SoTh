using NUnit.Framework;

namespace Sokoban
{

    [TestFixture]
    public class EventTests
    {
        [Test]
        public void EventTest_1()
        {
            Assert.That(Event.IsEventOfType(EventCategory.goXXX, EventType.goDown) == true);
            Assert.That(Event.IsEventOfType(EventCategory.goXXX, EventType.guardColumn) == false);
        }

        [Test]
        public void EventTest_2()
        {
            Assert.That(Event.IsEventOfType(EventCategory.movingXXX, EventType.movingUp) == true);
            Assert.That(Event.IsEventOfType(EventCategory.movingXXX, EventType.none) == false);
        }

        [Test]
        public void EventTest_3()
        {
            Assert.That(Event.IsEventOfType(EventCategory.movement, EventType.goLeft) == true &&
                        Event.IsEventOfType(EventCategory.movement, EventType.movingLeft) == true);
        }

    }
}