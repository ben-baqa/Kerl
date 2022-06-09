using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementGrid
{
    public bool isValid => tokens.Count <= 2;
    public bool isEmpty => tokens.Count == 0;

    List<TeamMenuToken> tokens;
    Vector2 centre, hOff, vOff;
    bool vertical;

    public PlacementGrid(Vector2 pos, float offset, bool vert = false)
    {
        tokens = new List<TeamMenuToken>();
        centre = pos;
        vertical = vert;
        hOff = Vector2.right * offset / 2;
        vOff = Vector2.up * offset / 2;
    }

    public void Add(TeamMenuToken item)
    {
        tokens.Add(item);
        Arrange();
    }
    public void Add(List<TeamMenuToken> toAdd)
    {
        foreach (TeamMenuToken item in toAdd)
            tokens.Add(item);
        Arrange();
    }
    public void Remove(TeamMenuToken item)
    {
        tokens.Remove(item);
        Arrange();
    }

    void Arrange()
    {
        int count = tokens.Count;
        for (int i = 0; i < count; i++)
            tokens[i].targetPosition = centre + GetOffset(i, count, vertical);
    }

    Vector2 GetOffset(int n, int count, bool vertical = false)
    {
        Vector2 offset = Vector2.zero;
        if (count == 1)
            return offset;

        int columns = (count + 1) / 2;
        if (count < 4)
            columns = count;

        int indexOffset = 2;
        if (count == 2 || count == 4)
            indexOffset = 1;
        else if (count >= 7)
            indexOffset = 3;

        if (vertical)
        {

            if (count < 4)
                offset = vOff * (indexOffset - 2 * n);
            else
            {
                offset = vOff * (indexOffset - 2 * (n / 2));

                offset += n % 2 == 0 ? -hOff : hOff;
            }
        }
        else
        {
            offset = hOff * (2 * (n % columns) - indexOffset);

            if (count > 3)
                offset += n >= columns ? -vOff : vOff;
        }
        return offset;
    }

    public List<int> GetPlayers()
    {
        List<int> res = new List<int>();
        foreach (TeamMenuToken token in tokens)
            res.Add(token.playerIndex);
        return res;
    }
}
