using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class Logger
{
    public List<Episode> episodes = new List<Episode>();
    public Episode episode = new Episode();

    private string logPath;
    private int logFrequency;
    private bool logInfo;

    public Logger(string path, int logFreq, bool log = true) {
        logPath = path;
        logFrequency = logFreq;
        logInfo = log;
    }

    public void StartEpisode(int episodeCounter)
    {
        if (!logInfo) return;
        if (episodeCounter % logFrequency != 0) return;
        episode = new Episode();
    }

    public void EndEpisode(int episodeCounter)
    {
        if (!logInfo) return;
        if (episodeCounter % logFrequency != 0) return;
        episodes.Add(episode);
        Save();
    }

    public void AddStep(int episodeCounter, int frameCounter, Vector3 position)
    {
        if (!logInfo) return;
        if (episodeCounter % logFrequency != 0) return;
        if (frameCounter % 5 == 0)
        {
            episode.trajectory.Add(position);
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(logPath, json);
        Debug.Log("Log saved to: " + logPath);
    }

    [Serializable]
    public class Episode
    {
        public List<Vector3> trajectory = new List<Vector3>();
    }
}
