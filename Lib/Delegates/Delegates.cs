namespace Sokoban.Lib
{
    public delegate void GameObjectMovedDel(int newX, int newY, char direction);
    public delegate void VoidChangeDelegate();
    public delegate void VoidStringDelegate(string s);
    public delegate void VoidObjectStringDelegate(object o, string s);
    public delegate void VoidObjectStringStringDelegate(object o, string s1, string s2);
    public delegate void GameChangeDelegate(GameChange gameChange);
}