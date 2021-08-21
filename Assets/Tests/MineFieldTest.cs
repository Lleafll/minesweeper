using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class MineFieldTest
{
    [Test]
    public void TestTileAtReturnsNullInitially()
    {
        var tiles = new Tile[1, 1];
        tiles[0, 0] = Tile.Mine;
        var mineField = new MineField(tiles);
        Assert.IsNull(mineField.TileAt(0, 0));
    }

    [Test]
    public void TestTileAtReturnsProperTileAfterClicking()
    {
        var tiles = new Tile[1, 1];
        tiles[0, 0] = Tile.Mine;
        var mineField = new MineField(tiles);
        mineField.RevealAt(0, 0);
        Assert.AreEqual(Tile.Mine, mineField.TileAt(0, 0));
    }
}
