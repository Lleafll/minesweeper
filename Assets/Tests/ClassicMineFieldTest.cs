using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class ClassicMineFieldTest
{
    [Test]
    public void TestTileAtReturnsNullInitially()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Mine;
        var mineField = new ClassicMineField(mines);
        Assert.AreEqual(Tile.Fog, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestTileAtReturnsProperTileAfterClicking()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Mine;
        var mineField = new ClassicMineField(mines);
        mineField.RevealAt(0, 0);
        Assert.AreEqual(Tile.Mine, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestSetFlagOnFog()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Mine;
        var mineField = new ClassicMineField(mines);
        mineField.SetFlag(0, 0);
        Assert.AreEqual(Tile.Flag, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestSetFlagOnRevealedTile()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Empty;
        var mineField = new ClassicMineField(mines);
        mineField.RevealAt(0, 0);
        mineField.SetFlag(0, 0);
        Assert.AreEqual(Tile.Empty, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestFlagPreventsReveal()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Mine;
        var mineField = new ClassicMineField(mines);
        mineField.SetFlag(0, 0);
        mineField.RevealAt(0, 0);
        mineField.SetFlag(0, 0);
        Assert.AreEqual(Tile.Fog, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestUnsetFlag()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Mine;
        var mineField = new ClassicMineField(mines);
        mineField.SetFlag(0, 0);
        mineField.SetFlag(0, 0);
        Assert.AreEqual(Tile.Fog, mineField.TileAt(0, 0));
    }

    [Test]
    public void TestAutomaticReveal()
    {
        var mines = new Tile[2, 2];
        mines[0, 0] = Tile.Empty;
        mines[0, 1] = Tile.Empty;
        mines[1, 0] = Tile.Empty;
        mines[1, 1] = Tile.Empty;
        var mineField = new ClassicMineField(mines);
        mineField.RevealAt(0, 0);
        Assert.AreEqual(Tile.Empty, mineField.TileAt(0, 0));
        Assert.AreEqual(Tile.Empty, mineField.TileAt(0, 1));
        Assert.AreEqual(Tile.Empty, mineField.TileAt(1, 0));
        Assert.AreEqual(Tile.Empty, mineField.TileAt(1, 1));
    }

    [Test]
    public void TestRevealWhenClickingOnNumbers()
    {
        var mines = new Tile[2, 2];
        mines[0, 0] = Tile.Mine;
        mines[0, 1] = Tile.Empty;
        mines[1, 0] = Tile.Empty;
        mines[1, 1] = Tile.Empty;
        var mineField = new ClassicMineField(mines);
        mineField.SetFlag(0, 0);
        mineField.RevealAt(0, 1);
        mineField.RevealAt(0, 1);
        Assert.AreEqual(Tile.Flag, mineField.TileAt(0, 0));
        Assert.AreEqual(Tile.Proximity1, mineField.TileAt(0, 1));
        Assert.AreEqual(Tile.Proximity1, mineField.TileAt(1, 0));
        Assert.AreEqual(Tile.Proximity1, mineField.TileAt(1, 1));
    }

    [Test]
    public void TestRevealWhenSettingFlagOnNumber()
    {
        var mines = new Tile[2, 2];
        mines[0, 0] = Tile.Mine;
        mines[0, 1] = Tile.Empty;
        mines[1, 0] = Tile.Empty;
        mines[1, 1] = Tile.Empty;
        var mineField = new ClassicMineField(mines);
        mineField.SetFlag(0, 0);
        mineField.RevealAt(0, 1);
        mineField.SetFlag(0, 1);
        Assert.AreEqual(Tile.Flag, mineField.TileAt(0, 0));
        Assert.AreEqual(Tile.Proximity1, mineField.TileAt(0, 1));
        Assert.AreEqual(Tile.Proximity1, mineField.TileAt(1, 0));
        Assert.AreEqual(Tile.Proximity1, mineField.TileAt(1, 1));
    }

    [Test]
    public void TestRevealWhenSettingFlagOnNumberWhenDynamic()
    {
        var mines = new Tile[2, 2] {
            { Tile.Mine, Tile.Empty},
            { Tile.Empty, Tile.Empty} };
        var mineField = new ClassicMineField(mines, false);
        mineField.SetFlag(0, 0);
        mineField.RevealAt(0, 1);
        mineField.SetFlag(0, 1);
        Assert.AreEqual(Tile.Flag, mineField.TileAt(0, 0));
        Assert.AreEqual(Tile.Proximity0, mineField.TileAt(0, 1));
        Assert.AreEqual(Tile.Proximity0, mineField.TileAt(1, 0));
        Assert.AreEqual(Tile.Proximity0, mineField.TileAt(1, 1));
    }

    [Test]
    public void TestCheckGameStatusRunning()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Empty;
        var mineField = new ClassicMineField(mines);
        Assert.AreEqual(GameStatus.Running, mineField.CheckGameStatus());
    }

    [Test]
    public void TestCheckGameStatusLostWhenMineIsRevealed()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Mine;
        var mineField = new ClassicMineField(mines);
        mineField.RevealAt(0, 0);
        Assert.AreEqual(GameStatus.Lost, mineField.CheckGameStatus());
    }

    [Test]
    public void TestCheckGameStatusWonWhenEverythingIsRevealed()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Empty;
        var mineField = new ClassicMineField(mines);
        mineField.RevealAt(0, 0);
        Assert.AreEqual(GameStatus.Won, mineField.CheckGameStatus());
    }

    [Test]
    public void TestNoChangeWhenRevealingEmptyTile()
    {
        var mines = new Tile[1, 1];
        mines[0, 0] = Tile.Empty;
        var mineField = new ClassicMineField(mines);
        mineField.RevealAt(0, 0);
        var changed = mineField.RevealAt(0, 0);
        Assert.IsFalse(changed);
    }
}
