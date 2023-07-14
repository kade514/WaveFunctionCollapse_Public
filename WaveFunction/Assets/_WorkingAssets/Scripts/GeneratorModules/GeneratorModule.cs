using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneratorModule : MonoBehaviour
{
    public GridGen GridGen;
    public string name => this.ToString();

    public virtual void OnValidate()
    {
        if (GridGen == null)
        {
            GridGen = GetComponent<GridGen>();
        }
    }

    public abstract IEnumerator Run();
}
