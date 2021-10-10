using System.Collections.Generic;
using UnityEngine;

public class SquadController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Min(1)] private int unitsCount = 10;
    [SerializeField] private Vector2 placerScale;

    [Header("References")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Dreamteck.Splines.SplineFollower follower;

    private List<UnitController> _units = new List<UnitController>();
    private bool _gameIsStarted = false;

    #region Unity
    private void Start()
    {
        CreateFirstUnits();
        DrawLine.OnEndTouch += PlaceUnits;
        UnitController.UnitDeath += UnitDeath;
    }

    private void OnDestroy()
    {
        DrawLine.OnEndTouch -= PlaceUnits;
        UnitController.UnitDeath -= UnitDeath;
    }
    #endregion

    private void CreateFirstUnits()
    {
        for (int i = 0; i < unitsCount; i++)
        {
            UnitController unit = KAP.Pool.Pool.Spawn(unitPrefab, transform).GetComponent<UnitController>();
            _units.Add(unit);
        }

        GameIsStarted(false);
    }

    #region Actions
    private void UnitDeath(UnitController unit)
    {
        _units.Remove(unit);
    }

    private void PlaceUnits(List<Vector2> localPositions)
    {
        if (!_gameIsStarted)
            GameIsStarted(true);

        //PlaceUnitsFromPositionsCount(localPositions);
        PlaceUnitsFromPositionsLength(localPositions);
    }
    #endregion


    #region PlaceUnitsFromPositionsLength
    private void PlaceUnitsFromPositionsLength(List<Vector2> localPositions)
    {
        float lengthToUnit = LineLength(localPositions) / _units.Count;
        float currentLength = 0;

        foreach (var unit in _units)
        {

            Vector2 unitPosition = GetUnitPosition(localPositions, currentLength);
            unit.MoveLocalPosition(new Vector3(unitPosition.x, 0, unitPosition.y));

            currentLength += lengthToUnit;
        }
    }

    private Vector2 GetUnitPosition(List<Vector2> localPositions, float neededLength)
    {
        Vector2 result = Vector2.zero;
        float currentLength = 0;
        float distance = 0;

        for (int i = 0; i < localPositions.Count - 2; i++)
        {
            distance = Vector2.Distance(localPositions[i], localPositions[i + 1]);

            if (currentLength + distance < neededLength)
            {
                currentLength += distance;
                continue;
            }
            else
            {
                float percent = 0;
                if(currentLength != 0)
                    percent = (neededLength - currentLength) / distance;
                result = Vector2.Lerp(localPositions[i], localPositions[i + 1], percent);
                break;
            }
        }
        result *= placerScale;
        return result;
    }

    private float LineLength(List<Vector2> vectors)
    {
        float result = 0;
        for (int i = 0; i < vectors.Count - 1; i++)
            result += Vector2.Distance(vectors[i], vectors[i + 1]);
        return result;
    }
    #endregion

    #region PlaceUnitsFromPositionsCount
    private void PlaceUnitsFromPositionsCount(List<Vector2> localPositions)
    {
        float pointsToUnit = (float)localPositions.Count / _units.Count;
        float unitIndexPos = 0;
        int positionIndex;
        float surplus;
        foreach (var unit in _units)
        {
            // найти целое число
            positionIndex = (int)unitIndexPos;

            // найти остаток
            surplus = unitIndexPos - positionIndex;

            // высчитать вектор
            Vector2 unitPosition = GetUnitPosition(localPositions, positionIndex, surplus);

            // выдвинуть туда юнита
            unit.MoveLocalPosition(new Vector3(unitPosition.x, 0, unitPosition.y));

            unitIndexPos += pointsToUnit;
        }
    }

    private Vector2 GetUnitPosition(List<Vector2> localPositions, int index, float surplus)
    {
        Vector2 result;

        if (localPositions.Count - 1 > index)
            result = Vector2.Lerp(localPositions[index], localPositions[index + 1], surplus);
        else
            result = localPositions[localPositions.Count - 1];

        result *= placerScale;

        return result;
    }
    #endregion

    private void GameIsStarted(bool value)
    {
        _gameIsStarted = value;
        follower.follow = value;
    }

    public void IsFinised()
    {
        foreach (var unit in _units)
        {
            unit.IsFinised();
        }
    }
}
