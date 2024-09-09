using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
[Serializable]
public class ScriptManager
{
    public List<MapTrigText> mapTexts = new List<MapTrigText>();
    public List<MapScript> scripts = new List<MapScript>();

    string scriptXMLFile = "Scripts";
    string NPCScriptXMLFile = "NPCScripts";

    XmlDocument text;


    // Road Text Script XML
    public void init()
    {

        text = GameManager.Resource.LoadXML(scriptXMLFile);
        // Initialize Trigger Texts
        MapTrigText mapTrigText = new MapTrigText();
        XmlNodeList nodes = text.SelectNodes("//mapID");
        TrigText trigs = new TrigText();
        foreach (XmlNode node in nodes)
        {
            XmlNode trigData = node.SelectSingleNode("trigID");


            // if not Same Trigger Id
            if (!trigData.Attributes["name"].Value.Equals(trigs.trigId))
            {
                // Save Previous Trigger Script Data
                if (trigs.trigId != null)
                {
                    mapTrigText.trigTexts.Add(trigs);
                }
                trigs = new TrigText();
                trigs.trigId = trigData.Attributes["name"].Value;
            }

            Script script = new Script();
            script.npcId = trigData["script"]["NPCID"].InnerText;
            script.script = trigData["script"]["kr"].InnerText;
            if (trigData["script"]["isAwait"] != null)
            {
                script.isAwait = false;
                script.delayT = float.Parse(trigData["script"]["delayT"].InnerText);
            }
            else
            {
                script.isAwait = true;
            }

            if (trigData["script"]["junctionID"] != null)
            {
                script.junctionID = trigData["script"]["junctionID"].InnerText;
            }
            else
            {
                script.junctionID = "";
            }


            if (trigData["script"]["startNPC"] != null)
            {
                script.startNPCId = trigData["script"]["startNPC"].InnerText;

            }
            else
            {
                script.startNPCId = "";
            }

            if (trigData["script"]["junction"] != null)
            {
                script.junction = trigData["script"]["junction"].InnerText;
            }
            else
            {
                script.junction = "";
            }

            if (trigData["script"]["subTrig"] != null)
            {
                script.subTriggerId = trigData["script"]["subTrig"].InnerText;
            }
            else
            {
                script.subTriggerId = "";
            }
            trigs.scripts.Add(script);

            // if not Same Map Id
            if (!node.Attributes["name"].Value.Equals(mapTrigText.mapId))
            {

                // Save Previous Map Script Data
                if (mapTrigText.mapId != null)
                {
                    mapTexts.Add(mapTrigText);
                }
                // Create new MapScript
                mapTrigText = new MapTrigText();
                mapTrigText.mapId = node.Attributes["name"].Value;
            }

        }

        mapTrigText.trigTexts.Add(trigs);
        mapTexts.Add(mapTrigText);

        text = GameManager.Resource.LoadXML(NPCScriptXMLFile);

        MapScript mapScript = new MapScript();
        XmlNodeList mapScriptNode = text.SelectNodes("//mapID");
        TrigScript mapTrigScript = new TrigScript();
        foreach (XmlNode node in mapScriptNode)
        {
            XmlNode trigData = node.SelectSingleNode("trigID");


            // if not Same Trigger Id
            if (!trigData.Attributes["name"].Value.Equals(mapTrigScript.trigId))
            {
                // Save Previous Trigger Script Data
                if (mapTrigScript.trigId != null)
                {
                    mapScript.scripts.Add(mapTrigScript);
                }
                mapTrigScript = new TrigScript();
                mapTrigScript.trigId = trigData.Attributes["name"].Value;
            }

            NPCScript script = new NPCScript();
            script.npcId = trigData["script"]["NPCID"].InnerText;
            script.script = trigData["script"]["kr"].InnerText;


            mapTrigScript.scripts.Add(script);

            // if not Same Map Id
            if (!node.Attributes["name"].Value.Equals(mapScript.mapId))
            {

                // Save Previous Map Script Data
                if (mapScript.mapId != null)
                {
                    scripts.Add(mapScript);
                }
                // Create new MapScript
                mapScript = new MapScript();
                mapScript.mapId = node.Attributes["name"].Value;
            }

        }

        mapScript.scripts.Add(mapTrigScript);
        scripts.Add(mapScript);

    }


    public MapTrigText getMapTrigTextData(string mapId)
    {
        MapTrigText ret = mapTexts.Find(item => item.mapId.Equals(mapId));
        return ret;
    }

    public MapScript getMapScriptsData(string mapId)
    {
        MapScript ret = scripts.Find(item => item.mapId.Equals(mapId));

        return ret;
    }
}

// Use Trigger active Texts
[Serializable]
public class MapTrigText
{
    public string mapId;
    public List<TrigText> trigTexts = new List<TrigText>();
}

[Serializable]
public class TrigText
{
    public string trigId;
    public List<Script> scripts = new List<Script>();
}

[Serializable]
public class Script
{
    public string junctionID;
    public string junction;
    public string npcId;
    public string startNPCId;
    public string script;
    public bool isAwait;
    public float delayT;
    public string subTriggerId;
}

// Use General NPC Texts
[Serializable]
public class MapScript
{
    public string mapId;
    public List<TrigScript> scripts = new List<TrigScript>();
}

[Serializable]
public class TrigScript
{
    public string trigId;
    public List<NPCScript> scripts = new List<NPCScript>();
}

[Serializable]
public class NPCScript
{
    public string npcId;
    public string script;
}