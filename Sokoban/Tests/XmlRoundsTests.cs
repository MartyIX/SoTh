using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;

namespace Sokoban.Tests
{
    [TestFixture]
    class XmlRoundsTests
    {
        #region roundXML definition
        string roundXml = 
            "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n"
          + "<SokobanAdventure xmlns=\"\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.martinvseticka.eu http://www.martinvseticka.eu/skola/programovani_v_CSharp/sokoban.xsd\">\n"
          + @"<Round>
                <Name>(2) Fun Round</Name>
                <Dimensions>
                  <width>8</width>
                  <height>7</height>
                </Dimensions>
                  <Aim>
                      <posX>7</posX>
                      <posY>1</posY>
                  </Aim>
                  <Aim>
                      <posX>7</posX>
                      <posY>2</posY>
                  </Aim>
                  <Aim>
                      <posX>2</posX>
                      <posY>6</posY>
                  </Aim>
                  <Aim>
                      <posX>3</posX>
                      <posY>6</posY>
                  </Aim>
                  <Box>
                      <posX>6</posX>
                      <posY>2</posY>
                  </Box>
                  <Box>
                      <posX>5</posX>
                      <posY>3</posY>
                  </Box>
                  <Box>
                      <posX>4</posX>
                      <posY>4</posY>
                  </Box>
                  <Box>
                      <posX>3</posX>
                      <posY>5</posY>
                  </Box>
                  <Wall>
                      <posX>1</posX>
                      <posY>1</posY>
                  </Wall>
                  <Wall>
                      <posX>2</posX>
                      <posY>1</posY>
                  </Wall>
                  <Wall>
                      <posX>4</posX>
                      <posY>1</posY>
                  </Wall>
                  <Wall>
                      <posX>5</posX>
                      <posY>1</posY>
                  </Wall>
                  <Wall>
                      <posX>1</posX>
                      <posY>2</posY>
                  </Wall>
                  <Wall>
                      <posX>3</posX>
                      <posY>2</posY>
                  </Wall>
                  <Wall>
                      <posX>4</posX>
                      <posY>2</posY>
                  </Wall>
                  <Wall>
                      <posX>2</posX>
                      <posY>3</posY>
                  </Wall>
                  <Wall>
                      <posX>3</posX>
                      <posY>3</posY>
                  </Wall>
                  <Wall>
                      <posX>1</posX>
                      <posY>4</posY>
                  </Wall>
                  <Wall>
                      <posX>2</posX>
                      <posY>4</posY>
                  </Wall>
                  <Wall>
                      <posX>7</posX>
                      <posY>4</posY>
                  </Wall>
                  <Wall>
                      <posX>8</posX>
                      <posY>4</posY>
                  </Wall>
                  <Wall>
                      <posX>1</posX>
                      <posY>5</posY>
                  </Wall>
                  <Wall>
                      <posX>6</posX>
                      <posY>5</posY>
                  </Wall>
                  <Wall>
                      <posX>7</posX>
                      <posY>5</posY>
                  </Wall>
                  <Wall>
                      <posX>1</posX>
                      <posY>6</posY>
                  </Wall>
                  <Wall>
                      <posX>5</posX>
                      <posY>6</posY>
                  </Wall>
                  <Wall>
                      <posX>6</posX>
                      <posY>6</posY>
                  </Wall>
                  <Wall>
                      <posX>8</posX>
                      <posY>6</posY>
                  </Wall>
                  <Wall>
                      <posX>1</posX>
                      <posY>7</posY>
                  </Wall>
                  <Wall>
                      <posX>5</posX>
                      <posY>7</posY>
                  </Wall>
                  <Wall>
                      <posX>7</posX>
                      <posY>7</posY>
                  </Wall>
                  <Wall>
                      <posX>8</posX>
                      <posY>7</posY>
                  </Wall>      
                  <!--     
                  <Monster>
                      <posX>4</posX>
                      <posY>6</posY>
                      <orientation>Left</orientation>
                      <firstState>guardRow</firstState>
                      <speed>15</speed>
                  </Monster>
                  -->
                  <Sokoban>
                      <posX>5</posX>
                      <posY>4</posY>
                      <twoWayOrientation>Left</twoWayOrientation>
                  </Sokoban>
              </Round>

              <Round>
                <Name>(1) Easy start</Name>
                <Dimensions>
                  <width>8</width>
                  <height>9</height>
                </Dimensions>
                <Aim>
                  <posX>4</posX>
                  <posY>3</posY>
                </Aim>
                <Aim>
                  <posX>4</posX>
                  <posY>4</posY>
                </Aim>
                <Box>
                  <posX>4</posX>
                  <posY>3</posY>    
                </Box>
                <Box>
                  <posX>4</posX>
                  <posY>4</posY>    
                </Box>
                <Wall>
                  <posX>5</posX>
                  <posY>5</posY>    
                </Wall>
                
                <Monster>
                   <posX>1</posX>
                   <posY>1</posY>
                   <orientation>Left</orientation>
                   <firstState>guardRow</firstState>
                   <speed>10</speed>           
                </Monster>
                
                <Sokoban>
                   <posX>2</posX>
                   <posY>7</posY>
                   <twoWayOrientation>Left</twoWayOrientation>            
                </Sokoban>
                
              </Round> 
            </SokobanAdventure>";
        #endregion roundXML definition


        [Test]
        public void Test_1() // test of: validation
        {
            Assert.That(CommonFunc.IsXMLValid(Properties.Resources.sokoban, roundXml, false) == true);
        }
        
        // Test 2 ===================================================================================

        int test_2_loadingGameObjectsCounter = 0;

        void test_2_loadedGameObject(int objectID, string description, int posX, int posY, MovementDirection direction, EventType InitialEvent, int speed)
        {
            if (test_2_loadingGameObjectsCounter == 0)
            {
                Assert.That(description != null);
                Assert.That(description.CompareTo("A") == 0);
                Assert.That(posX == 7);
                Assert.That(posY == 1);
                Assert.That(speed == 0);
                Assert.That(direction == MovementDirection.none);
                Assert.That(InitialEvent == EventType.none);
            }

            test_2_loadingGameObjectsCounter++;

        }

        void test_2_loadedRoundProperties(string roundName, int maxX, int maxY)
        {
            Debug.WriteLine("maxX: '" + maxX.ToString() + "', maxY: '" + maxY.ToString() + "', roundName: '" + roundName + "'");
            Assert.That(roundName != null);
            Assert.That(roundName.CompareTo("(2) Fun Round") == 0);
            Assert.That(maxX == 8);
            Assert.That(maxY == 7);
        }

        [Test]
        public void Test_2() // test of: loadedRoundProperties and loadedGameObject
        {
            XmlRounds xmlRounds = new XmlRounds(roundXml);
            xmlRounds.loadedRoundProperties += new d_RoundProperties(test_2_loadedRoundProperties);
            xmlRounds.loadedGameObject += new d_GameObjectProperties(test_2_loadedGameObject);
            xmlRounds.LoadRound(1);
        }
    }
}
