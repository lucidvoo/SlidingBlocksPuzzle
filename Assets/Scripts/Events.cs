using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Generic event system

// list of all events, events created here.
public static class Events
{
    // Player input
    public static readonly Evt<Tile> onTileClicked = new Evt<Tile>();

    // Tiles animation
    public static readonly Evt onTileStartAnyMovement = new Evt();
    public static readonly Evt onTileCompleteAnyMovement = new Evt();
    public static readonly Evt onWrongTileClicked = new Evt();
    public static readonly Evt onTileStartSliding = new Evt();
    public static readonly Evt onTileStopSliding = new Evt();
    public static readonly Evt onHoleTileRemovingStarted = new Evt();
    public static readonly Evt onHoleTileDisappearVfx = new Evt();
    public static readonly Evt onHoleTileAppearVfx = new Evt();

    // Level
    public static readonly Evt onLevelWin = new Evt();
    public static readonly Evt onBoardStartShuffling = new Evt();
    public static readonly Evt onBoardShuffled = new Evt();

    // Game
    public static readonly Evt onGamePaused = new Evt();
    public static readonly Evt onGameUnpaused = new Evt();

    // Event with parameter example
    //public static readonly Evt<Card> onCardClicked = new Evt<Card>();

    // Event without parameters example
    //public static readonly Evt onGameStarted = new Evt();
}

// event class with no parameters
public class Evt
{
    private event Action EventAction = delegate { };

    public void AddListener(Action listenerMethod)
    {
        EventAction += listenerMethod;
    }

    public void RemoveListener(Action listenerMethod)
    {
        EventAction -= listenerMethod;
    }

    public void Invoke()
    {
        EventAction.Invoke();
    }
}

// event with 1 parameter
public class Evt<T>
{
    private event Action<T> EventAction = delegate { };

    public void AddListener(Action<T> listenerMethod)
    {
        EventAction += listenerMethod;
    }

    public void RemoveListener(Action<T> listenerMethod)
    {
        EventAction -= listenerMethod;
    }

    public void Invoke(T param)
    {
        EventAction.Invoke(param);
    }
}

public class Evt<T, V>
{
    private event Action<T, V> EventAction = delegate { };

    public void AddListener(Action<T, V> listenerMethod)
    {
        EventAction += listenerMethod;
    }

    public void RemoveListener(Action<T, V> listenerMethod)
    {
        EventAction -= listenerMethod;
    }

    public void Invoke(T param1, V param2)
    {
        EventAction.Invoke(param1, param2);
    }
}
