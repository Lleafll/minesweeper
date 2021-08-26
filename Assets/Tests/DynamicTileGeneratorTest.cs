using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DynamicTileGeneratorTest
{
    [Test]
    public void Generate()
    {
        var mines = new bool[2, 2] {
            { true, false },
            { false, false } };
        var flags = new bool[2, 2]{
            { false, false},
            { false, false} };
        var generator = new DynamicTileGenerator(mines, flags);
        var expected = new Tile[2, 2] {
            { Tile.Mine, Tile.Proximity1 },
            { Tile.Proximity1, Tile.Proximity1 } };
        Assert.AreEqual(expected, generator.Generate());
    }
}
