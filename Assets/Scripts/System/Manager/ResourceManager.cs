using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Resource Manager. Doing load resource, destroy object, save load resource handle
public class ResourceManager
{
    // Save Resource Handle with string Dictionary
    private Dictionary<string, AsyncOperationHandle> LoadResources = new();

    // Load Object with object path
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }

    // Load Game Object by GameObject Instance, and Setting position and rotation
    public GameObject Instantiate(GameObject gameObject, Vector3 pos = default, Quaternion rotation = default)
    {
        GameObject prefab = UnityEngine.Object.Instantiate(gameObject, pos, rotation);
        if (prefab != null)
        {
            return prefab;
        }
        else
        {
            Debug.Log($"Failed to load Prefab : {gameObject.name}");
            return null;
        }
    }

    // Load Game Object by Addressables path, and Setting position and rotation
    public GameObject InstantiateAsync(string path, Vector3 pos = default, Quaternion rotation = default)
    {

        if (LoadResources.ContainsKey(path))
        {
            GameObject go = Instantiate((GameObject)LoadResources[path].Result, pos, rotation);
            return go;
        }
        else
        {
            var op = LoadAssetAsync<GameObject>(path);
            // Object Load Successfully, Save Load Resources Handle
            if (!op.Equals(default))
            {
                LoadResources[path] = op;
            }
            // Load resource saved successfully, Instantiate Object
            if (LoadResources.ContainsKey(path))
            {
                GameObject go = Instantiate((GameObject)LoadResources[path].Result, pos, rotation);
                return go;

            }
            // Load resource save failed, print debug log
            else
            {
                Debug.Log($"Failed to load GameObject : {path}");
                return null;
            }
        }
    }

    // Load Addressables Asset by Addressables path
    public AsyncOperationHandle LoadAssetAsync<T>(string path) where T : UnityEngine.Object
    {
        // Resource is already saved, return save resource handle
        if (LoadResources.ContainsKey(path))
        {
            return LoadResources[path];
        }
        else
        {
            // Try Addressable Asset Load with string path. if path is not exists, return default handle
            try
            {
                var op = Addressables.LoadAssetAsync<T>(path);
                op.WaitForCompletion();
                LoadResources[path] = op;
                return op;
            }
            catch
            {
                return default;
            }
        }
    }

    // Load Addressables Asset by AssetReference
    public AsyncOperationHandle LoadAssetAsync<T>(AssetReference assetReference) where T : UnityEngine.Object
    {
        var op = Addressables.LoadAssetAsync<T>(assetReference);
        op.WaitForCompletion();
        return op;
    }

    // Load Addressables Asset by Tag name
    public List<AsyncOperationHandle> LoadAssetAsyncWithTag<T>(string label) where T : UnityEngine.Object
    {
        var ops = Addressables.LoadResourceLocationsAsync(label);
        List<AsyncOperationHandle> ret = new();
        foreach (var location in ops.Result)
        {
            AsyncOperationHandle op = LoadAssetAsync<UnityEngine.Object>(location.PrimaryKey);
            ret.Add(op);
        }

        return ret;
    }

    // Load Sprite by Adressables Path
    public Sprite LoadSprite(string path)
    {
        if (LoadResources.ContainsKey(path))
        {
            Sprite spr = (Sprite)LoadResources[path].Result;
            return spr;
        }
        else
        {
            var op = LoadAssetAsync<Sprite>(path);
            if (!op.Equals(default))
            {
                LoadResources[path] = op;
            }
            if (LoadResources.ContainsKey(path))
            {
                Sprite spr = (Sprite)LoadResources[path].Result;
                return spr;

            }
            else
            {
                Debug.Log($"Failed to load Sprite : {path}");
                return null;
            }
        }
    }

    // Loading XML Datas in path
    public XmlDocument LoadXML(string path)
    {
        XmlDocument xml = new XmlDocument();
        TextAsset txtAsset = Load<TextAsset>($"XML/{path}");
        xml.LoadXml(txtAsset.text);

        if (xml == null)
        {
            Debug.Log($"Failed to load XML : {path}");
            return null;
        }

        return xml;
    }

    // Destroy game Object, if object loaded by addressables, Unload object
    public void Destroy(GameObject obj)
    {
        if (obj == null) return;
        if (!Addressables.ReleaseInstance(obj))
        {
            UnityEngine.Object.Destroy(obj);
        }
        else
        {
            Debug.Log("Destroy by Release");
        }
    }

    // Destroy game Objects, if objects loaded by addressables, Unload object
    public void Destroy(GameObject[] objs)
    {
        foreach (GameObject go in objs)
        {
            if (go != null)
                Destroy(go);
        }
    }

}