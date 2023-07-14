using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovementController : MonoBehaviour
{
    public GridGen GridGen;

    private Vector3 _cachedPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfCameraMoved();
    }

    void CheckIfCameraMoved()
    {
        if(GridGen.GetIsGenerating())
            return;

        if (_cachedPos != transform.position)
        {
            // SOMETHING moved the camera.
            _cachedPos = transform.position;

            if (GridGen.TileDictionary == null)
                return;

            foreach (var tile in GridGen.TileDictionary)
            {
                tile.Value.UpdatePosition();
            }
        }
    }
}
