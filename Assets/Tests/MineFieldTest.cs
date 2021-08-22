using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class MineFieldTest
{
    [Test]
    public void TestTileAtReturnsNullInitially()
    {
        var mines = new bool[1, 1];
        mines[0, 0] = true;
        var mineField = new MineField(mines);
        Assert.AreEqual(Tile.Fog, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestTileAtReturnsProperTileAfterClicking()
    {
        var mines = new bool[1, 1];
        mines[0, 0] = true;
        var mineField = new MineField(mines);
        mineField.RevealAt(0, 0);
        Assert.AreEqual(Tile.Mine, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestSetFlagOnFog()
    {
        var mines = new bool[1, 1];
        mines[0, 0] = true;
        var mineField = new MineField(mines);
        mineField.SetFlag(0, 0);
        Assert.AreEqual(Tile.Flag, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestSetFlagOnRevealedTile()
    {
        var mines = new bool[1, 1];
        mines[0, 0] = false;
        var mineField = new MineField(mines);
        mineField.RevealAt(0, 0);
        mineField.SetFlag(0, 0);
        Assert.AreEqual(Tile.Empty, mineField.TileAt(0, 0));
    }
}
