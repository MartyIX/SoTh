using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sokoban;
using Sokoban.Solvers;

namespace DummyObjectImplementations
{
    public enum DummySolverProviderEnum
    {
        TestOne,
        TestTwo,
        TestThree
    }

    public class DummySolverProvider : ISolverProvider
    {
        private DummySolverProviderEnum en;

        public DummySolverProvider(DummySolverProviderEnum en)
        {
            this.en = en;
        }
        
        
        #region ISolverProvider Members

        public uint GetMazeWidth()
        {
            if (en == DummySolverProviderEnum.TestOne)
            {
                return 19;
            }
            else if (en == DummySolverProviderEnum.TestTwo)
            {
                return 30;
            }
            else if (en == DummySolverProviderEnum.TestThree)
            {
                return 11;
            }

            throw new Exception("Unknown test: " + en.ToString());
        }

        public uint GetMazeHeight()
        {
            if (en == DummySolverProviderEnum.TestOne)
            {
                return 17;
            }
            else if (en == DummySolverProviderEnum.TestTwo)
            {
                return 18;
            }
            else if (en == DummySolverProviderEnum.TestThree)
            {
                return 11;
            }

            throw new Exception("Unknown test: " + en.ToString());
        }

        public string SerializeMaze()
        {
            if (en == DummySolverProviderEnum.TestOne)
            {
                return "##############################################################   ################$  ################  $##############  $ $ ############# # ## ###########   # ## #####  ..## $  $          ..###### ### #@##  ..######     ########################################################################################################";
            }
            else if (en == DummySolverProviderEnum.TestTwo)
            {
                return "############################### .$ ## $. ## .$ ## $. ## .$ ##$  .  .  $##$  .  .  $##$  .##.  $##$  .  .  $##$  .  .  $## $. ## .$ ## $. ## .$ ## $. #### #  # ##  ## #  # ##  ## ##### #  # ## #  . # # ##  ## ### $. ## .$ #   $  # .$ ## $. ##.  $##$  . .$##  #$  .  .  $##$  .  .  $#  @#$. .  $##$  .## .$ ## $. #  $   # $. ## .$ ### ##  ## # # .  # ## #  # ##### ##  ## #  # ##  ## #  # #### .$ ## $. ## .$ ## $. ## .$ ##$  .  .  $##$  .  .  $##$  .##.  $##$  .  .  $##$  .  .  $## $. ## .$ ## $. ## .$ ## $. ###############################";
            }
            else if (en == DummySolverProviderEnum.TestThree)
            {
                return "############    *    ## $$ ## $ ##  $..#$$ ## ##*.*.  ##*#..@..#*##  .*.*## ## $$#..$  ## $ ## $$ ##    *    ############";
            }


            throw new Exception("Unknown test: " + en.ToString());
        }

        public event Sokoban.Lib.GameObjectMovedDel SokobanMoved;

        #endregion
    }
}
