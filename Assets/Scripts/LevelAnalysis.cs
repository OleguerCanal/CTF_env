using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class LevelAnalysis : MonoBehaviour
{

    [Range(1.0f, 10.0f)]
    public float time = 10.0f;

    public string path = "Assets/Resources/dummy.txt";

    [System.Serializable]
     public enum Persona
     {
        None,
        Custom, // Use inputed path
        Efficient,
        Completionist,
     }
     public Persona persona;

    void OnDrawGizmos()
    {
        switch (persona)
        {
            case Persona.None:
                return;
            case Persona.Efficient:
                path = "Assets/Resources/efficient.txt";
                break;
            case Persona.Completionist:
                path = "Assets/Resources/completionist.txt";
                break;
            default:
                break;
        }

        // TODO: Load by level id (if(File.exists(....)))
        Logger loggedResult = JsonUtility.FromJson<Logger>(File.ReadAllText(path));
        if (loggedResult != null)
        {
            Gizmos.color = Color.white;
            float eCount = (float) loggedResult.episodes.Count;
            int minVal = (int) Mathf.Max(0, eCount*(time - 1.0f)/10.0f);
            int maxVal = (int) Mathf.Min(eCount, eCount*(time + 1.0f)/10.0f);
            if (time == 10.0f)
            {
                minVal = 0;
                maxVal = (int) eCount;
            }
            for (int e = minVal; e < maxVal; e++)
            {
                var episode = loggedResult.episodes[e];
                for (int i = 1; i < episode.trajectory.Count; i++)
                {
                    Gizmos.DrawLine(episode.trajectory[i - 1], episode.trajectory[i]);
                }
            }
        }
    }
}
