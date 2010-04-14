using NUnit.Framework;

namespace Sokoban
{

    [TestFixture]
    public class SimulationTests
    {
        [Test]
        public void CalendarFirstMethod_test_1() // test of: "first" method
        {
            Calendar cal = new Calendar();
            GameObject gameObject = new GameObject();

            cal.AddEvent(1, gameObject, EventType.goDown);

            Event ev = cal.First(2);
            Assert.That(ev != null && ev.who == gameObject && ev.when == 1);
        }

        [Test]
        public void CalendarFirstMethod_test_2() 
        {
            // test of: "first" method in combination with time parameter of first method
            Calendar cal = new Calendar();
            GameObject gameObject = new GameObject();
            
            cal.AddEvent(2, gameObject, EventType.goDown);
            cal.AddEvent(3, gameObject, EventType.goDown);

            Event ev = cal.First(2);
            Assert.That(ev != null && ev.who == gameObject && ev.when == 2);
        }

        [Test]
        public void CalendarFirstMethod_test_3()
        {
            Calendar cal = new Calendar();
            GameObject gameObject = new GameObject();

            cal.AddEvent(2, gameObject, EventType.goDown);

            Event ev = cal.First(1);
            Assert.That(ev == null); // no event is supposed to be returned
        }

        [Test]
        public void CalendarFirstMethod_test_4() // test of: removing events from calendar
        {
            Calendar cal = new Calendar();
            GameObject gameObject = new GameObject();

            cal.AddEvent(1, gameObject, EventType.goDown);

            Event ev = cal.First(2);
            Assert.That(cal.CountOfEvents == 0);
        }


        [Test]
        public void CalendarFirstMethod_test_5() // test of: removing events from calendar
        {
            Calendar cal = new Calendar();
            GameObject gameObject = new GameObject();

            cal.AddEvent(2, gameObject, EventType.goDown);
            cal.AddEvent(2, gameObject, EventType.goDown);

            Event ev = cal.First(2);
            Assert.That(cal.CountOfEvents == 1);
            ev = cal.First(2);
            Assert.That(cal.CountOfEvents == 0);
        }

        [Test]
        public void CalendarFirstMethod_test_6() // test of: IsEnabledAddingEvents
        {
            Calendar cal = new Calendar();
            GameObject gameObject = new GameObject();

            cal.IsEnabledAddingEvents = false;
            cal.AddEvent(2, gameObject, EventType.goDown);

            Assert.That(cal.CountOfEvents == 0);
        }

        [Test]
        public void CalendarFirstMethod_test_7() // test of: IsEnabledAddingEvents
        {
            Calendar cal = new Calendar();
            GameObject gameObject = new GameObject();

            cal.IsEnabledAddingMovementEvents = false;
            cal.AddEvent(1, gameObject, EventType.goDown);
            cal.AddEvent(2, gameObject, EventType.none);

            Assert.That(cal.CountOfEvents == 1);
        }    

    }
}