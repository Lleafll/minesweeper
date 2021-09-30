using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StaticTileGeneratorTest
{
    [Test]
    public void Generate()
    {
        var mines = new Tile[2, 2] {
            { Tile.Mine, Tile.Empty },
            { Tile.Empty, Tile.Empty } };
        var generator = new StaticTileGenerator(mines);
        var expected = new Tile[2, 2] {
            { Tile.Mine, Tile.Proximity1 },
            { Tile.Proximity1, Tile.Proximity1 } };
        Assert.AreEqual(expected, generator.Generate());
    }
}
