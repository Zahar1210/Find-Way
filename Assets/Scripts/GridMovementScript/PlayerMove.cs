using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed;
    public List<PointRotation> PointRotations = new();
    public Surface[] surfaces;
    public int index { get; set; }
    public bool isMove{ get; set; }
    private bool findTarget{ get; set; }
    public bool isMoving{ get; set; }
    
    public void StartMove()
    {
        if (isMoving) {
            isMoving = false;
            findTarget = true;
        }
    }
    public void Move()
    {
        isMoving = true;
        findTarget = false;
        StartCoroutine(MoveCoroutine());
    }
    private IEnumerator MoveCoroutine()
    {
        while (index < PointRotations.Count)
        {
            if (!isMove && !findTarget)
                yield return StartCoroutine(Roll((surfaces[index].tile == surfaces[index + 1].tile) ? 180 : 90,
                    PointRotations[index]));
            else
                yield return new WaitForSeconds(0.1f);
        }
        findTarget = true;
        isMoving = false;
    }
    private IEnumerator Roll(float remainingAngle, PointRotation point)
    {
        index++;
        isMove = true;
        if (point.isIgnore) {
            isMove = false;
        }
        else {
            while (remainingAngle > 0) {
                float rotAngle = Mathf.Min(Time.fixedDeltaTime * speed, remainingAngle);
                transform.RotateAround(point.Position, Vector3.Cross(point.Rotation_1, point.Rotation_2), rotAngle);
                remainingAngle -= rotAngle;
                yield return null;
            }
            isMove = false;
        }
    }
    public class PointRotation
    {
        public Vector3 Position { get; }
        public Vector3 Rotation_1 { get; } 
        public Vector3 Rotation_2 { get; }
        public bool isIgnore { get; }

        public PointRotation(Vector3 rotation1, Vector3 rotation2, Tile tile, bool ignore)
        {
            Rotation_1 = rotation1;
            Rotation_2 = rotation2;
            isIgnore = ignore;
            Position = tile.Pos + Rotation_1 / 2 + Rotation_2 / 2;
        }
    }
}