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

    // Extension for Vector3. 
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

}