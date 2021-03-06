using UnityEngine;

// All public enums of the project and their extensions

public enum Direction { UP = 0, 
                        RIGHT = 1,
                        DOWN = 2,
                        LEFT = 3,
                        Count = 4}

// Extension methods for Direction enum
public static class DirectionExtension
{
    // Get reversed direction. Using: Direction dirRev = dirVar.GetReversed();
    public static Direction GetReversed(this Direction dir)
    {
        return (Direction)(((int)dir + 2) % 4);
    }


    // Get unit vector for a direction
    public static Vector3 GetUnitVector3(this Direction dir)
    {
        Vector3 vector = Vector3.zero;
        switch (dir)
        {
            case Direction.UP:
                vector = Vector3.up;
                break;
            case Direction.RIGHT:
                vector = Vector3.right;
                break;
            case Direction.DOWN:
                vector = Vector3.down;
                break;
            case Direction.LEFT:
                vector = Vector3.left;
                break;
            case Direction.Count:
            default:
                Debug.LogError("Direction enum error!");
                break;
        }
        return vector;
    }

    // Get unit vector for a direction
    public static Vector2Int GetUnitVector2Int(this Direction dir)
    {
        Vector2Int vector = Vector2Int.zero;
        switch (dir)
        {
            case Direction.UP:
                vector = Vector2Int.up;
                break;
            case Direction.RIGHT:
                vector = Vector2Int.right;
                break;
            case Direction.DOWN:
                vector = Vector2Int.down;
                break;
            case Direction.LEFT:
                vector = Vector2Int.left;
                break;
            case Direction.Count:
            default:
                Debug.LogError("Direction enum error!");
                break;
        }
        return vector;
    }

    // Extension for Vector3. 
    /* UNUSED
    public static Direction ComputeDirectionFromVector3(this Vector3 vector3)
    {
        float angle = Vector3.SignedAngle(Vector3.up, vector3, Vector3.back);
        if (angle > -45f && angle <= 45f)
        {
            return Direction.UP;
        }
        if (angle > 45f && angle <= 135f)
        {
            return Direction.RIGHT;
        }
        if (angle > -135 && angle <= -45f)
        {
            return Direction.LEFT;
        }
        return Direction.DOWN;
    }
    */

    // Extension for Vector2Int
    public static Direction ConvertToDirection(this Vector2Int vector2Int)
    {
        if (Mathf.Abs(vector2Int.x) > Mathf.Abs(vector2Int.y)) // horiz direction
        {
            return vector2Int.x > 0 ? Direction.RIGHT : Direction.LEFT;
        }
        else
        {
            return vector2Int.y > 0 ? Direction.UP : Direction.DOWN;
        }
    }
}